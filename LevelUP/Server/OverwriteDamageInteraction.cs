using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

[HarmonyPatchCategory("levelup_damageinteraction")]
class OverwriteDamageInteraction
{
    public readonly Harmony patch = new("levelup_damageinteraction");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_damageinteraction"))
        {
            patch.PatchCategory("levelup_damageinteraction");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_damageinteraction"))
        {
            patch.UnpatchCategory("levelup_damageinteraction");
        }
    }

    // Overwrite Damage Interaction
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Entity), "ReceiveDamage")]
    internal static void ReceiveDamageStart(Entity __instance, DamageSource damageSource, ref float damage)
    {
        if (__instance.World.Side != EnumAppSide.Server) return;
        if (damage <= 0) return;
        if (!__instance.Alive) return;
        if (!__instance.ShouldReceiveDamage(damageSource, damage)) return;

        // Player Does Damage
        if (damageSource.SourceEntity is EntityPlayer || damageSource.GetCauseEntity() is EntityPlayer)
        {
            if (damageSource.SourceEntity is EntityPlayer)
                Debug.LogDebug($"{(damageSource.SourceEntity as EntityPlayer).GetName() ?? "PlayerProjectile"} previous damage: {damage}");

            // Check if source entity is a projectile
            bool SourceEntityIsProjectile()
            {
                return
                // Native Game
                damageSource.SourceEntity is EntityProjectile ||
                damageSource.SourceEntity is EntityThrownStone ||
                // Combat Overhaul compatibility
                damageSource.SourceEntity.GetType().ToString() == "CombatOverhaul.RangedSystems.ProjectileEntity";
            }

            // Melee Action
            if (damageSource.SourceEntity is EntityPlayer)
            {
                // Get player source
                EntityPlayer playerEntity = damageSource.SourceEntity as EntityPlayer;
                // Get player instance
                IPlayer player = playerEntity.Player;

                // Integration
                damage = OverwriteDamageInteractionEvents.GetExternalMeleeDamageStart(player, damageSource, damage);

                // Integration
                damage = OverwriteDamageInteractionEvents.GetExternalMeleeDamageFinish(player, damageSource, damage);
            }
            // Ranged Action
            else if (damageSource.GetCauseEntity() is EntityPlayer && SourceEntityIsProjectile())
            {
                // Get player entity
                EntityPlayer playerEntity = damageSource.GetCauseEntity() as EntityPlayer;

                // Get player instance
                IPlayer player = playerEntity.Player;

                // Integration
                damage = OverwriteDamageInteractionEvents.GetExternalRangedDamageStart(player, damageSource, damage);

                // Integration
                damage = OverwriteDamageInteractionEvents.GetExternalRangedDamageFinish(player, damageSource, damage);
            }
            // Invalid
            else if (Configuration.enableExtendedLog)
                Debug.LogWarn($"WARNING: Invalid damage type in OverwriteDamageInteraction, cause entity is unhandled: {damageSource.GetCauseEntity()} or source entity is unhandled: {damageSource.SourceEntity}");

            if (damageSource.SourceEntity is EntityPlayer)
                Debug.LogDebug($"{(damageSource.SourceEntity as EntityPlayer).GetName() ?? "PlayerProjectile"} final damage: {damage}");
        }

        bool ValidPlayerReceivedDamage()
        {
            return damageSource.Type switch
            {
                EnumDamageType.Hunger => false,
                EnumDamageType.Heal => false,
                EnumDamageType.Suffocation => false,
                _ => true,
            };
        }

        // Player Receive Damage
        if (__instance is EntityPlayer && ValidPlayerReceivedDamage())
        {
            Debug.LogDebug($"{(__instance as EntityPlayer)?.GetName()} received damage: {damage} from: {damageSource.Type}");

            // Get player source
            EntityPlayer playerEntity = __instance as EntityPlayer;
            // Get player instance
            IPlayer player = playerEntity.Player;

            // Integration
            damage = OverwriteDamageInteractionEvents.GetExternalReceiveDamageStart(player, damageSource, damage);

            // Integration
            damage = OverwriteDamageInteractionEvents.GetExternalReceiveDamageFinish(player, damageSource, damage);

            Debug.LogDebug($"{player.PlayerName} received final damage: {damage}");
        }
        // Unkown Type
        else if (__instance is EntityPlayer)
        {
            // Get player source
            EntityPlayer playerEntity = __instance as EntityPlayer;
            // Get player instance
            IPlayer player = playerEntity.Player;

            damage = OverwriteDamageInteractionEvents.GetExternalReceiveDamageUnkown(player, damageSource, damage);
        }

        if (damage < 0) damage = 0;
    }

    // Handle Status: 
    // ProtectionModifiers
    // FlatDamageReduction
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModSystemWearableStats), "handleDamaged")]
    [HarmonyPriority(Priority.VeryLow)]
    internal static void HandleDamagedStart(ModSystemWearableStats __instance, IPlayer player, ref float damage, DamageSource dmgSource)
    {
        IInventory inv = player.InventoryManager.GetOwnInventory("character");

        // The inventory codes: 12,13,14 is reserved to store armor
        List<ItemSlot> armorSlots = [];
        for (int i = 12; i <= 14; i++)
        {
            ItemSlot armorSlot = inv[i];
            if (armorSlot.Itemstack?.Item is ItemWearable armorWearable)
            {
                if (armorWearable.ProtectionModifiers != null)
                {
                    if (armorSlot.Itemstack.Attributes.TryGetFloat("FlatDamageReduction") != null)
                    {
                        armorWearable.ProtectionModifiers.FlatDamageReduction = armorSlot.Itemstack.Attributes.GetFloat("BaseFlatDamageReduction");
                    }
                    if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseRelativeProtection") != null)
                    {
                        armorWearable.ProtectionModifiers.RelativeProtection = armorSlot.Itemstack.Attributes.GetFloat("BaseRelativeProtection");
                    }
                }

                if (armorWearable.StatModifers != null)
                {
                    if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseHealingEffectivness") != null)
                    {
                        armorWearable.StatModifers.healingeffectivness = armorSlot.Itemstack.Attributes.GetFloat("BaseHealingEffectivness");
                    }
                    if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseHungerRate") != null)
                    {
                        armorWearable.StatModifers.hungerrate = armorSlot.Itemstack.Attributes.GetFloat("BaseHungerRate");
                    }
                    if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseRangedWeaponsAccuracy") != null)
                    {
                        armorWearable.StatModifers.rangedWeaponsAcc = armorSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsAccuracy");
                    }
                    if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseRangedWeaponsSpeed") != null)
                    {
                        armorWearable.StatModifers.rangedWeaponsSpeed = armorSlot.Itemstack.Attributes.GetFloat("BaseRangedWeaponsSpeed");
                    }
                    if (armorSlot.Itemstack.Attributes.TryGetFloat("BaseWalkSpeed") != null)
                    {
                        armorWearable.StatModifers.walkSpeed = armorSlot.Itemstack.Attributes.GetFloat("BaseWalkSpeed");
                    }
                }

                armorSlots.Add(armorSlot);
            }
        }

        if (armorSlots.Count > 0)
        {
            damage = OverwriteDamageInteractionEvents.GetExternalArmorReceiveDamageStat(player, armorSlots, damage);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ModSystemWearableStats), "handleDamaged")]
    [HarmonyPriority(Priority.VeryLow)]
    internal static void HandleDamagedFinish(ModSystemWearableStats __instance, IPlayer player, ref float damage, DamageSource dmgSource)
    {
        IInventory inv = player.InventoryManager.GetOwnInventory("character");

        // The inventory codes: 12,13,14 is reserved to store armor
        List<ItemSlot> armorSlots = [];
        for (int i = 12; i <= 14; i++)
        {
            ItemSlot armorSlot = inv[i];
            if (armorSlot.Itemstack?.Item is ItemWearable)
            {
                armorSlots.Add(armorSlot);
            }
        }

        if (armorSlots.Count > 0)
        {
            damage = OverwriteDamageInteractionEvents.GetExternalArmorReceiveDamageStatPos(player, armorSlots, damage);
        }
    }


    // Handle Status: 
    // healingeffectivness
    // hungerrate
    // rangedWeaponsAcc
    // rangedWeaponsSpeed
    // walkSpeed
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModSystemWearableStats), "updateWearableStats")]
    [HarmonyPriority(Priority.VeryLow)]
    internal static void UpdateWearableStatsStart(ModSystemWearableStats __instance, InventoryBase inv, IServerPlayer player)
    {
        // The inventory codes: 12,13,14 is reserved to store armor
        List<ItemSlot> armorSlots = [];
        for (int i = 12; i <= 14; i++)
        {
            ItemSlot armorSlot = inv[i];
            if (armorSlot.Itemstack?.Item is ItemWearable)
            {
                armorSlots.Add(armorSlot);
            }
        }

        if (armorSlots.Count > 0)
        {
            OverwriteDamageInteractionEvents.ExecuteArmorReceiveHandleStat(player, armorSlots);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ModSystemWearableStats), "updateWearableStats")]
    [HarmonyPriority(Priority.VeryLow)]
    internal static void UpdateWearableStatsFinish(ModSystemWearableStats __instance, InventoryBase inv, IServerPlayer player)
    {
        // The inventory codes: 12,13,14 is reserved to store armor
        List<ItemSlot> armorSlots = [];
        for (int i = 12; i <= 14; i++)
        {
            ItemSlot armorSlot = inv[i];
            if (armorSlot.Itemstack?.Item is ItemWearable)
            {
                armorSlots.Add(armorSlot);
            }
        }

        if (armorSlots.Count > 0)
        {
            OverwriteDamageInteractionEvents.ExecuteArmorReceiveHandleStatPos(player, armorSlots);
        }
    }

}


#region Compatibility
public static class OverwriteDamageInteractionEvents
{
    public delegate void DamageModifierHandler(IPlayer player, DamageSource damageSource, ref float damage);
    public delegate void ArmorStatusModifierHandler(IPlayer player, List<ItemSlot> item);
    public delegate void ArmorDamageModifierHandler(IPlayer player, List<ItemSlot> item, ref float damage);

    public static event DamageModifierHandler OnPlayerMeleeDoDamageStart;
    public static event DamageModifierHandler OnPlayerMeleeDoDamageFinish;
    public static event DamageModifierHandler OnPlayerRangedDoDamageStart;
    public static event DamageModifierHandler OnPlayerRangedDoDamageFinish;
    public static event DamageModifierHandler OnPlayerReceiveDamageStart;
    public static event DamageModifierHandler OnPlayerReceiveDamageFinish;
    public static event DamageModifierHandler OnPlayerReceiveDamageUnkown;
    public static event ArmorStatusModifierHandler OnPlayerArmorReceiveHandleStats;
    public static event ArmorDamageModifierHandler OnPlayerArmorReceiveDamageStat;
    public static event ArmorStatusModifierHandler OnPlayerArmorReceiveHandleStatsPos;
    public static event ArmorDamageModifierHandler OnPlayerArmorReceiveDamageStatPos;

    internal static float GetExternalMeleeDamageStart(IPlayer player, DamageSource damageSource, float damage)
    {
        OnPlayerMeleeDoDamageStart?.Invoke(player, damageSource, ref damage);
        return damage;
    }

    internal static float GetExternalMeleeDamageFinish(IPlayer player, DamageSource damageSource, float damage)
    {
        OnPlayerMeleeDoDamageFinish?.Invoke(player, damageSource, ref damage);
        return damage;
    }

    internal static float GetExternalRangedDamageStart(IPlayer player, DamageSource damageSource, float damage)
    {
        OnPlayerRangedDoDamageStart?.Invoke(player, damageSource, ref damage);
        return damage;
    }

    internal static float GetExternalRangedDamageFinish(IPlayer player, DamageSource damageSource, float damage)
    {
        OnPlayerRangedDoDamageFinish?.Invoke(player, damageSource, ref damage);
        return damage;
    }

    internal static float GetExternalReceiveDamageStart(IPlayer player, DamageSource damageSource, float damage)
    {
        OnPlayerReceiveDamageStart?.Invoke(player, damageSource, ref damage);
        return damage;
    }

    internal static float GetExternalReceiveDamageFinish(IPlayer player, DamageSource damageSource, float damage)
    {
        OnPlayerReceiveDamageFinish?.Invoke(player, damageSource, ref damage);
        return damage;
    }

    internal static float GetExternalReceiveDamageUnkown(IPlayer player, DamageSource damageSource, float damage)
    {
        OnPlayerReceiveDamageUnkown?.Invoke(player, damageSource, ref damage);
        return damage;
    }

    internal static void ExecuteArmorReceiveHandleStat(IPlayer player, List<ItemSlot> item)
    {
        OnPlayerArmorReceiveHandleStats?.Invoke(player, item);
    }

    internal static float GetExternalArmorReceiveDamageStat(IPlayer player, List<ItemSlot> item, float damage)
    {
        OnPlayerArmorReceiveDamageStat?.Invoke(player, item, ref damage);
        return damage;
    }

    internal static void ExecuteArmorReceiveHandleStatPos(IPlayer player, List<ItemSlot> item)
    {
        OnPlayerArmorReceiveHandleStats?.Invoke(player, item);
    }

    internal static float GetExternalArmorReceiveDamageStatPos(IPlayer player, List<ItemSlot> item, float damage)
    {
        OnPlayerArmorReceiveDamageStat?.Invoke(player, item, ref damage);
        return damage;
    }
}
#endregion