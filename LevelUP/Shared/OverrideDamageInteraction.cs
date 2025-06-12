using System;
using System.Collections.Generic;
using HarmonyLib;
using LevelUP.Server;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace LevelUP.Shared;

#pragma warning disable IDE0060
[HarmonyPatchCategory("levelup_damageinteraction")]
class OverwriteDamageInteraction
{
    private static Instance instance;
    public Harmony overwriter;

    public void OverwriteNativeFunctions(Instance _instance)
    {
        instance = _instance;
        if (!Harmony.HasAnyPatches("levelup_damageinteraction"))
        {
            overwriter = new Harmony("levelup_damageinteraction");
            overwriter.PatchCategory("levelup_damageinteraction");
            Debug.Log("Damage interaction has been overwrited");
        }
        else
        {
            if (instance.side == EnumAppSide.Client) Debug.Log("Damage interaction overwriter has already patched, probably by the singleplayer server");
            else Debug.LogError("ERROR: Damage interaction overwriter has already patched, did some mod already has levelup_damageinteraction in harmony?");
        }
    }

    // Overwrite Damage Interaction
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Entity), "ReceiveDamage")]
    public static void ReceiveDamageStart(Entity __instance, DamageSource damageSource, ref float damage)
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

                #region hunter
                // Increase the damage
                damage *= Configuration.HunterGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Hunter"));
                #endregion

                switch (player.InventoryManager.ActiveTool)
                {
                    #region knife
                    case EnumTool.Knife:
                        if (Configuration.enableLevelKnife)
                        {
                            damage *= Configuration.KnifeGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Knife"));
                            Experience.IncreaseExperience(player, "Knife", "Hit");
                        }
                        break;
                    #endregion
                    #region axe
                    case EnumTool.Axe:
                        if (Configuration.enableLevelAxe)
                        {
                            damage *= Configuration.AxeGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Axe"));
                            Experience.IncreaseExperience(player, "Axe", "Hit");
                        }
                        break;
                    #endregion
                    #region pickaxe
                    case EnumTool.Pickaxe:
                        if (Configuration.enableLevelPickaxe)
                        {
                            damage *= Configuration.PickaxeGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe"));
                            Experience.IncreaseExperience(player, "Pickaxe", "Hit");
                        }
                        break;
                    #endregion
                    #region shovel
                    case EnumTool.Shovel:
                        if (Configuration.enableLevelShovel)
                        {
                            damage *= Configuration.ShovelGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Shovel"));
                            Experience.IncreaseExperience(player, "Shovel", "Hit");
                        }
                        break;
                    #endregion
                    #region spear
                    case EnumTool.Spear:
                        if (Configuration.enableLevelSpear)
                        {
                            damage *= Configuration.SpearGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Spear"));
                            Experience.IncreaseExperience(player, "Spear", "Hit");
                        }
                        break;
                    #endregion
                    #region hammer
                    case EnumTool.Hammer:
                        if (Configuration.enableLevelHammer)
                        {
                            damage *= Configuration.HammerGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Hammer"));
                            Experience.IncreaseExperience(player, "Hammer", "Hit");
                        }
                        break;
                    #endregion
                    #region sword
                    case EnumTool.Sword:
                        if (Configuration.enableLevelSword)
                        {
                            damage *= Configuration.SwordGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Sword"));
                            Experience.IncreaseExperience(player, "Sword", "Hit");
                        }
                        break;
                    #endregion
                    default:
                        #region hand
                        if (Configuration.enableLevelHand && player.InventoryManager.ActiveHotbarSlot != null)
                        {
                            // Check if the active slot is empty
                            if (player.InventoryManager.ActiveHotbarSlot.Itemstack == null)
                            {
                                damage *= Configuration.HandGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Hand"));
                                Experience.IncreaseExperience(player, "Hand", "Hit");
                            }
                        }
                        #endregion
                        break;
                }

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

                #region bow
                // Increase the damage if the damage source is from any arrow
                if (Configuration.enableLevelBow && damageSource.SourceEntity.GetName().Contains("arrow"))
                {
                    damage *= Configuration.BowGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow"));
                    Experience.IncreaseExperience(player, "Bow", "Hit");
                }
                #endregion

                #region spear
                // Increase the damage if the damage source is from any spear
                if (Configuration.enableLevelSpear && damageSource.SourceEntity.GetName().Contains("spear"))
                {
                    damage *= Configuration.SpearGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Spear"));
                    Experience.IncreaseExperience(player, "Spear", "Hit");
                }
                #endregion

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
            Debug.LogDebug($"{(damageSource.SourceEntity as EntityPlayer)?.GetName()} received damage: {damage} from: {damageSource.Type}");

            // Get player source
            EntityPlayer playerEntity = __instance as EntityPlayer;
            // Get player instance
            IPlayer player = playerEntity.Player;

            // Integration
            damage = OverwriteDamageInteractionEvents.GetExternalReceiveDamageStart(player, damageSource, damage);

            #region vitality
            if (Configuration.enableLevelVitality && damage < Configuration.DamageLimitVitality)
            {
                // Check if damage is bigger than player max health
                float damageCalculation = damage;
                float playerMaxHealth = playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                // If is set the damage experience limit to the player max health
                if (playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < damage) damageCalculation = playerMaxHealth;

                Experience.IncreaseExperience(player, "Vitality", (ulong)Configuration.VitalityEXPEarnedByDAMAGE(damageCalculation));
            }
            #endregion

            // Check if the damage received is from a valid entity source damage
            // in others cases the armor shouldn't reduce damage
            List<string> equippedArmors = null;
            if (damageSource.GetCauseEntity() != null || damageSource.SourceEntity != null)
            {
                equippedArmors = [];
                { // Getting all player equipped armors
                    foreach (IInventory playerInventory in player.InventoryManager.Inventories.Values)
                    {
                        // Get inventory type
                        string inventoryType = playerInventory.GetType().ToString();
                        // Check if is armor inventory
                        if (inventoryType.Contains("InventoryCharacter"))
                        {
                            int index = 0;
                            // Swipe all items in this inventory
                            foreach (ItemSlot item in playerInventory)
                            {
                                // Check if slot contains item
                                if (item.Itemstack == null || item.Itemstack.Item == null)
                                {
                                    index++;
                                    continue;
                                }

                                equippedArmors.Add(item.Itemstack.Item.Code.ToString());
                            }
                            break;
                        }
                    }
                }

                // Check if damage is bigger than player max health
                float damageCalculation = damage;
                float playerMaxHealth = playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                // If is set the damage experience limit to the player max health
                if (playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < damage) damageCalculation = playerMaxHealth;

                // Swipe all inventorys to receive armor multiply
                double multiplyLeatherArmor = 1.0;
                double multiplyChainArmor = 1.0;
                double multiplyBrigandineArmor = 1.0;
                double multiplyPlateArmor = 1.0;
                double multiplyScaleArmor = 1.0;

                // Getting all multiply
                foreach (string armorCode in equippedArmors)
                {
                    if (Configuration.enableLevelLeatherArmor && damage < Configuration.DamageLimitLeatherArmor)
                    {
                        // Check if the armor contains experience
                        double value = Configuration.expMultiplyHitLeatherArmor.GetValueOrDefault(armorCode, 0.0);
                        multiplyLeatherArmor += value;

                        if (value != 0.0)
                            Debug.LogDebug($"{player.PlayerName} received damage using: {armorCode} as armor");
                    }

                    if (Configuration.enableLevelChainArmor && damage < Configuration.DamageLimitChainArmor)
                    {
                        // Check if the armor contains experience
                        double value = Configuration.expMultiplyHitChainArmor.GetValueOrDefault(armorCode, 0.0);
                        multiplyChainArmor += value;

                        if (value != 0.0)
                            Debug.LogDebug($"{player.PlayerName} received damage using: {armorCode} as armor");
                    }

                    if (Configuration.enableLevelBrigandineArmor && damage < Configuration.DamageLimitBrigandineArmor)
                    {
                        // Check if the armor contains experience
                        double value = Configuration.expMultiplyHitBrigandineArmor.GetValueOrDefault(armorCode, 0.0);
                        multiplyBrigandineArmor += value;

                        if (value != 0.0)
                            Debug.LogDebug($"{player.PlayerName} received damage using: {armorCode} as armor");
                    }

                    if (Configuration.enableLevelPlateArmor && damage < Configuration.DamageLimitPlateArmor)
                    {
                        // Check if the armor contains experience
                        double value = Configuration.expMultiplyHitPlateArmor.GetValueOrDefault(armorCode, 0.0);
                        multiplyPlateArmor += value;

                        if (value != 0.0)
                            Debug.LogDebug($"{player.PlayerName} received damage using: {armorCode} as armor");
                    }

                    if (Configuration.enableLevelScaleArmor && damage < Configuration.DamageLimitScaleArmor)
                    {
                        // Check if the armor contains experience
                        double value = Configuration.expMultiplyHitScaleArmor.GetValueOrDefault(armorCode, 0.0);
                        multiplyScaleArmor += value;

                        if (value != 0.0)
                            Debug.LogDebug($"{player.PlayerName} received damage using: {armorCode} as armor");
                    }
                }

                { // Experience Armor
                    // Leather Armor
                    if (multiplyLeatherArmor > 1.0)
                    {
                        Experience.IncreaseExperience(player, "LeatherArmor", (ulong)Math.Round(Configuration.LeatherArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiplyLeatherArmor));

                        float damageReduction = Configuration.LeatherArmorDamageReductionByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor")) * (float)multiplyLeatherArmor;
                        damage -= damageReduction;
                        Debug.LogDebug($"{player.PlayerName} reduced {damageReduction} damage by leather armor level");
                    }

                    // Chain Armor
                    if (multiplyChainArmor > 1.0)
                    {
                        Experience.IncreaseExperience(player, "ChainArmor", (ulong)Math.Round(Configuration.ChainArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiplyChainArmor));

                        float damageReduction = Configuration.ChainArmorDamageReductionByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor")) * (float)multiplyChainArmor;
                        damage -= damageReduction;
                        Debug.LogDebug($"{player.PlayerName} reduced {damageReduction} damage by chain armor level");
                    }

                    // Brigandine Armor
                    if (multiplyBrigandineArmor > 1.0)
                    {
                        Experience.IncreaseExperience(player, "BrigandineArmor", (ulong)Math.Round(Configuration.BrigandineArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiplyBrigandineArmor));

                        float damageReduction = Configuration.BrigandineArmorDamageReductionByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_BrigandineArmor")) * (float)multiplyBrigandineArmor;
                        damage -= damageReduction;
                        Debug.LogDebug($"{player.PlayerName} reduced {damageReduction} damage by brigandine armor level");
                    }

                    // Plate Armor
                    if (multiplyPlateArmor > 1.0)
                    {
                        Experience.IncreaseExperience(player, "PlateArmor", (ulong)Math.Round(Configuration.PlateArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiplyPlateArmor));

                        float damageReduction = Configuration.PlateArmorDamageReductionByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor")) * (float)multiplyPlateArmor;
                        damage -= damageReduction;
                        Debug.LogDebug($"{player.PlayerName} reduced {damageReduction} damage by plate armor level");
                    }

                    // Scale Armor
                    if (multiplyScaleArmor > 1.0)
                    {
                        Experience.IncreaseExperience(player, "ScaleArmor", (ulong)Math.Round(Configuration.ScaleArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiplyScaleArmor));

                        float damageReduction = Configuration.ScaleArmorDamageReductionByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor")) * (float)multiplyScaleArmor;
                        damage -= damageReduction;
                        Debug.LogDebug($"{player.PlayerName} reduced {damageReduction} damage by plate armor level");
                    }
                }
            }

            // Integration
            damage = OverwriteDamageInteractionEvents.GetExternalReceiveDamageFinish(player, damageSource, equippedArmors, damage);

            Debug.LogDebug($"{player.PlayerName} received final damage: {damage}");
        }

        // If the armor reduces less than 0, just change to 0
        if (damage < 0) damage = 0;
    }

    #region bow
    // Overwrite Projectile impact
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EntityProjectile), "impactOnEntity")]
    public static void ImpactOnEntity(EntityProjectile __instance, Entity entity)
    {
        if (!Configuration.enableLevelBow) return;
        if (__instance.World.Side != EnumAppSide.Server) return;

        // Check if is a arrow
        if (__instance.Code.ToString().Contains("arrow"))
        {
            // Check if arrow is shotted by a player
            if (__instance.FiredBy is EntityPlayer)
            {
                EntityPlayer playerEntity = __instance.FiredBy as EntityPlayer;

                float chance = Configuration.BowGetChanceToNotLoseArrowByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow"));

                // Integration
                chance = OverwriteDamageInteractionEvents.GetExternalBowDropChance(playerEntity.Player, chance);

                // Change the change based on level
                __instance.DropOnImpactChance = chance;
            }
        }
    }
    // Overwrite Bow shot start
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ItemBow), "OnHeldInteractStop")]
    public static void OnHeldInteractBowStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
    {
        if (!Configuration.enableLevelBow) return;
        if (byEntity.Api.Side != EnumAppSide.Server) return;

        if (byEntity is EntityPlayer)
        {
            float chance = Configuration.BowGetAimAccuracyByLevel(byEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow", 0));

            // Integration
            chance = OverwriteDamageInteractionEvents.GetExternalBowAiming((byEntity as EntityPlayer).Player, chance);

            // Setting new aim accuracy
            byEntity.Attributes.SetFloat("aimingAccuracy", chance);

            Debug.LogDebug($"Bow Accuracy: {chance}");
        }
    }
    #endregion
    #region spear
    // Overwrite Spear shot start
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ItemSpear), "OnHeldInteractStop")]
    public static void OnHeldInteractSpearStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
    {
        if (!Configuration.enableLevelSpear) return;
        if (byEntity.Api.Side != EnumAppSide.Server) return;

        if (byEntity is EntityPlayer)
        {
            float chance = Configuration.SpearGetAimAccuracyByLevel(byEntity.WatchedAttributes.GetInt("LevelUP_Level_Spear", 0));

            // Integration
            chance = OverwriteDamageInteractionEvents.GetExternalSpearAiming((byEntity as EntityPlayer).Player, chance);

            // Setting new aim accuracy
            byEntity.Attributes.SetFloat("aimingAccuracy", chance);
            Debug.LogDebug($"Spear Accuracy: {chance}");
        }
    }
    #endregion
    #region shield
    // Overwrite the Shield function end
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModSystemWearableStats), "applyShieldProtection")]
    public static void ApplyShieldProtectionStart(ModSystemWearableStats __instance, IPlayer player, ref float damage, DamageSource dmgSource)
    {
        if (!Configuration.enableLevelShield) return;
        if (player.Entity.Api.Side != EnumAppSide.Server) return;

        ItemSlot[] shieldSlots =
        [
            player.Entity.LeftHandItemSlot,
            player.Entity.RightHandItemSlot
        ];
        // Swipe all shields from player hands
        for (int i = 0; i < shieldSlots.Length; i++)
        {
            ItemSlot shieldSlot = shieldSlots[i];
            JsonObject attr = shieldSlot.Itemstack?.ItemAttributes?["shield"];
            // Checking if is a shield if not continues
            if (attr == null || !attr.Exists)
                continue;

            // Integration
            damage = OverwriteDamageInteractionEvents.GetExternalShieldReceiveDamageStart(player, dmgSource, damage);

            // Reduces the damage received more than normal based on shield level
            float damageReduced = damage * Configuration.ShieldGetReductionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield"));
            damage -= damageReduced;

            // Integration
            damage = OverwriteDamageInteractionEvents.GetExternalShieldReceiveDamageFinish(player, dmgSource, damage);

            if (damage < 0) damage = 0;
            Debug.LogDebug($"{player.PlayerName} reduced: {damageReduced} in shield damage");

            Experience.IncreaseExperience(player, "Shield", "Hit");
        }
    }
    #endregion
}

#region Compatibility
public static class OverwriteDamageInteractionEvents
{
    public delegate void DamageModifierHandler(IPlayer player, DamageSource damageSource, ref float damage);
    public delegate void DamageArmorModifierHandler(IPlayer player, DamageSource damageSource, List<string> equippedArmors, ref float damage);
    public delegate void PlayerFloatModifierHandler(IPlayer player, ref float number);

    public static event DamageModifierHandler OnPlayerMeleeDoDamageStart;
    public static event DamageModifierHandler OnPlayerMeleeDoDamageFinish;
    public static event DamageModifierHandler OnPlayerRangedDoDamageStart;
    public static event DamageModifierHandler OnPlayerRangedDoDamageFinish;
    public static event DamageModifierHandler OnPlayerReceiveDamageStart;
    public static event DamageArmorModifierHandler OnPlayerReceiveDamageFinish;
    public static event PlayerFloatModifierHandler OnBowDropChanceRefresh;
    public static event PlayerFloatModifierHandler OnBowAimingRefresh;
    public static event PlayerFloatModifierHandler OnSpearAimingRefresh;
    public static event DamageModifierHandler OnPlayerShieldReceiveDamageStart;
    public static event DamageModifierHandler OnPlayerShieldReceiveDamageFinish;

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

    internal static float GetExternalReceiveDamageFinish(IPlayer player, DamageSource damageSource, List<string> equippedArmors, float damage)
    {
        OnPlayerReceiveDamageFinish?.Invoke(player, damageSource, equippedArmors, ref damage);
        return damage;
    }

    internal static float GetExternalBowDropChance(IPlayer player, float chance)
    {
        OnBowDropChanceRefresh?.Invoke(player, ref chance);
        return chance;
    }

    internal static float GetExternalBowAiming(IPlayer player, float chance)
    {
        OnBowAimingRefresh?.Invoke(player, ref chance);
        return chance;
    }

    internal static float GetExternalSpearAiming(IPlayer player, float chance)
    {
        OnSpearAimingRefresh?.Invoke(player, ref chance);
        return chance;
    }

    internal static float GetExternalShieldReceiveDamageStart(IPlayer player, DamageSource damageSource, float damage)
    {
        OnPlayerShieldReceiveDamageStart?.Invoke(player, damageSource, ref damage);
        return damage;
    }

    internal static float GetExternalShieldReceiveDamageFinish(IPlayer player, DamageSource damageSource, float damage)
    {
        OnPlayerShieldReceiveDamageFinish?.Invoke(player, damageSource, ref damage);
        return damage;
    }
}
#endregion