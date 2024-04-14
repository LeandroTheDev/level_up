using System;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
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
            else Debug.Log("ERROR: Damage interaction overwriter has already patched, did some mod already has levelup_damageinteraction in harmony?");
        }
    }

    // Overwrite Damage Interaction
    static bool singlePlayerDoubleCheck = true; // for some reason in single player the client instance is called 2 times in a row
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Entity), "ReceiveDamage")]
    public static bool ReceiveDamage(Entity __instance, DamageSource damageSource, float damage)
    {
        // Damage bug treatment
        if (damage < 0) return true;

        // Player Does Damage
        // Checking if damage sources is from a player and from a server and if entity is alive
        if (damageSource.SourceEntity is EntityPlayer || damageSource.GetCauseEntity() is EntityPlayer && __instance.Api.World.Side == EnumAppSide.Server && __instance.Alive)
        {
            // Melee Action
            if (damageSource.SourceEntity is EntityPlayer)
            {
                // Get player source
                EntityPlayer playerEntity = damageSource.SourceEntity as EntityPlayer;
                // Get player instance
                IPlayer player = __instance.Api.World.PlayerByUid(playerEntity.PlayerUID);

                #region hunter            
                // Increase the damage
                damage *= Configuration.HunterGetDamageMultiplyByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Hunter"));
                #endregion

                #region knife
                // Increase the damage if actual tool is a knife
                if (Configuration.enableLevelKnife && player.InventoryManager.ActiveTool == EnumTool.Knife)
                {
                    damage *= Configuration.KnifeGetDamageMultiplyByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Knife"));
                    // Increase exp for using knife weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Knife_Hit");
                    // Single player treatment
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                    {
                        instance.clientAPI.channel.SendPacket("Increase_Knife_Hit");
                        singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                    }
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer) singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                };
                #endregion

                #region axe
                // Increase the damage if actual tool is a axe
                if (Configuration.enableLevelAxe && player.InventoryManager.ActiveTool == EnumTool.Axe)
                {
                    damage *= Configuration.AxeGetDamageMultiplyByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Axe"));
                    // Increase exp for using axe weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Axe_Hit");
                    // Single player treatment
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                    {
                        instance.clientAPI.channel.SendPacket("Increase_Axe_Hit");
                        singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                    }
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer) singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                };
                #endregion

                #region pickaxe
                // Increase the damage if actual tool is a pickaxe
                if (Configuration.enableLevelPickaxe && player.InventoryManager.ActiveTool == EnumTool.Pickaxe)
                {
                    damage *= Configuration.PickaxeGetDamageMultiplyByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Pickaxe"));
                    // Increase exp for using pickaxe weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Pickaxe_Hit");
                    // Single player treatment
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                    {
                        instance.clientAPI.channel.SendPacket("Increase_Pickaxe_Hit");
                        singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                    }
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer) singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                };
                #endregion

                #region shovel
                // Increase the damage if actual tool is a shovel
                if (Configuration.enableLevelShovel && player.InventoryManager.ActiveTool == EnumTool.Shovel)
                {
                    damage *= Configuration.ShovelGetDamageMultiplyByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Shovel"));
                    // Increase exp for using shovel weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Shovel_Hit");
                    // Single player treatment
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                    {
                        instance.clientAPI.channel.SendPacket("Increase_Shovel_Hit");
                        singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                    }
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer) singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                };
                #endregion

                #region spear
                // Increase the damage if actual tool is a spear
                if (Configuration.enableLevelSpear && player.InventoryManager.ActiveTool == EnumTool.Spear)
                {
                    damage *= Configuration.SpearGetDamageMultiplyByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Spear"));
                    // Increase exp for using spear weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Spear_Hit");
                    // Single player treatment
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                    {
                        instance.clientAPI.channel.SendPacket("Increase_Spear_Hit");
                        singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                    }
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer) singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                }
                #endregion
            }
            // Ranged Action
            else if (damageSource.GetCauseEntity() is EntityPlayer && damageSource.SourceEntity is EntityProjectile)
            {
                // Get entities
                EntityPlayer playerEntity = damageSource.GetCauseEntity() as EntityPlayer;
                EntityProjectile itemDamage = damageSource.SourceEntity as EntityProjectile;

                // Get player instance
                IPlayer player = __instance.Api.World.PlayerByUid(playerEntity.PlayerUID);

                #region bow
                // Increase the damage if the damage source is from any arrow
                if (Configuration.enableLevelBow && itemDamage != null && itemDamage.GetName().Contains("arrow"))
                {
                    damage *= Configuration.BowGetDamageMultiplyByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Bow"));
                    // Increase exp for using bow weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Bow_Hit");
                    // Single player treatment
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                    {
                        instance.clientAPI.channel.SendPacket("Increase_Bow_Hit");
                        singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                    }
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer) singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                };
                #endregion

                #region spear
                // Increase the damage if the damage source is from any spear
                if (Configuration.enableLevelSpear && itemDamage != null && itemDamage.GetName().Contains("spear"))
                {
                    damage *= Configuration.SpearGetDamageMultiplyByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Spear"));
                    // Increase exp for using spear weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Spear_Hit");
                    // Single player treatment
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                    {
                        instance.clientAPI.channel.SendPacket("Increase_Spear_Hit");
                        singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                    }
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer) singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                };
                #endregion
            }
            // Invalid
            else Debug.Log($"ERROR: Invalid damage type in OverwriteDamageInteraction, cause entity is invalid: {damageSource.GetCauseEntity()} or source entity is invalid: {damageSource.SourceEntity}");
        }

        // Player Receive Damage
        // Checking if received damage is a player and if is a server and if is alive
        if (__instance is EntityPlayer && __instance.Api.World.Side == EnumAppSide.Server && __instance.Alive)
        {
            // Get player source
            EntityPlayer playerEntity = __instance as EntityPlayer;
            // Get player instance
            IPlayer player = __instance.Api.World.PlayerByUid(playerEntity.PlayerUID);

            #region vitality
            if (Configuration.enableLevelVitality)
            {
                // Check if damage is bigger than player max health
                float damageCalculation = damage;
                float playerMaxHealth = playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                // If is set the damage experience limit to the player max health
                if (playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < damage) damageCalculation = playerMaxHealth;

                if (player is IServerPlayer && instance.serverAPI != null)
                    instance.serverAPI?.OnClientMessage(player as IServerPlayer, $"Increase_Vitality_Hit&forceexp={Configuration.VitalityEXPEarnedByDAMAGE(damageCalculation)}");

                // Single player treatment
                else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                {
                    instance.clientAPI.channel.SendPacket($"Increase_Vitality_Hit&forceexp={Configuration.VitalityEXPEarnedByDAMAGE(damageCalculation)}");
                    singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
                }
                else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer) singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
            }
            #endregion
        };

        #region native
        if ((!__instance.Alive || __instance.IsActivityRunning("invulnerable")) && damageSource.Type != EnumDamageType.Heal)
        {
            return false;
        }
        if (__instance.ShouldReceiveDamage(damageSource, damage))
        {
            foreach (EntityBehavior behavior in __instance.SidedProperties.Behaviors)
            {
                behavior.OnEntityReceiveDamage(damageSource, ref damage);
            }
            if (damageSource.Type != EnumDamageType.Heal && damage > 0f)
            {
                __instance.WatchedAttributes.SetInt("onHurtCounter", __instance.WatchedAttributes.GetInt("onHurtCounter") + 1);
                __instance.WatchedAttributes.SetFloat("onHurt", damage);
                if (damage > 0.05f)
                {
                    __instance.AnimManager.StartAnimation("hurt");
                }
            }
            if (damageSource.GetSourcePosition() != null)
            {
                Vec3d dir = (__instance.SidedPos.XYZ - damageSource.GetSourcePosition()).Normalize();
                dir.Y = 0.699999988079071;
                float factor = damageSource.KnockbackStrength * GameMath.Clamp((1f - __instance.Properties.KnockbackResistance) / 10f, 0f, 1f);
                __instance.WatchedAttributes.SetFloat("onHurtDir", (float)Math.Atan2(dir.X, dir.Z));
                __instance.WatchedAttributes.SetDouble("kbdirX", dir.X * (double)factor);
                __instance.WatchedAttributes.SetDouble("kbdirY", dir.Y * (double)factor);
                __instance.WatchedAttributes.SetDouble("kbdirZ", dir.Z * (double)factor);
            }
            else
            {
                __instance.WatchedAttributes.SetDouble("kbdirX", 0.0);
                __instance.WatchedAttributes.SetDouble("kbdirY", 0.0);
                __instance.WatchedAttributes.SetDouble("kbdirZ", 0.0);
                __instance.WatchedAttributes.SetFloat("onHurtDir", -999f);
            }
            return damage > 0f;
        }
        return false;
        #endregion end
    }

    // Overwrite Durability lost
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CollectibleObject), "DamageItem")]
    public static bool DamageItem(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, int amount = 1)
    {
        // Error treatment
        if (!Configuration.enableDurabilityMechanic || itemslot == null || byEntity == null) return true;

        // Check if the entity is a player and if this code is running on the server
        if (byEntity is EntityPlayer && world.Side == EnumAppSide.Server)
        {
            // Dedicated Server needs to broadcast the durability restoration, single player no
            if (instance.serverAPI != null)
            {
                // Refresh player inventory
                foreach (IPlayer iplayer in instance.serverAPI.api.World.AllOnlinePlayers)
                {
                    // Find the player instance
                    if (iplayer.PlayerName == byEntity.GetName())
                    {
                        IServerPlayer player = iplayer as IServerPlayer;
                        // We need to refresh player inventory with new durability
                        Task.Delay(100).ContinueWith((_) => player.BroadcastPlayerData(true));
                        break;
                    }
                }
            }
            EntityPlayer playerEntity = byEntity as EntityPlayer;
            // Get change of not using durability
            switch (itemslot.Itemstack?.Item?.Tool)
            {
                case EnumTool.Bow: return !Configuration.BowRollChanceToNotReduceDurabilityByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Bow"));
                case EnumTool.Axe: return !Configuration.AxeRollChanceToNotReduceDurabilityByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Axe"));
                case EnumTool.Knife: return !Configuration.KnifeRollChanceToNotReduceDurabilityByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Knife"));
                case EnumTool.Pickaxe: return !Configuration.PickaxeRollChanceToNotReduceDurabilityByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Pickaxe"));
                case EnumTool.Shovel: return !Configuration.ShovelRollChanceToNotReduceDurabilityByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Shovel"));
                case EnumTool.Spear: return !Configuration.SpearRollChanceToNotReduceDurabilityByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Spear"));
            }
        }
        return true;
    }

    // Overwrite Projectile impact
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EntityProjectile), "impactOnEntity")]
    public static void ImpactOnEntity(EntityProjectile __instance, Entity entity)
    {
        // Check if is not the server and do nothing
        if (Configuration.enableLevelBow || __instance.World.Side != EnumAppSide.Server) return;

        // Check if is a arrow
        if (__instance.GetName().Contains("arrow"))
        {
            // Check if arrow is shotted by a player
            if (__instance.FiredBy is not EntityPlayer) return;
            EntityPlayer playerEntity = __instance.FiredBy as EntityPlayer;

            // Change the change based on level
            __instance.DropOnImpactChance = Configuration.BowGetChanceToNotLoseArrowByEXP((ulong)playerEntity.WatchedAttributes.GetLong("LevelUP_Bow"));
        }
    }

    #region bow
    // Overwrite Bow shot start
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ItemBow), "OnHeldInteractStop")]
    public static void OnHeldInteractStopBowStart(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
    {
        if (Configuration.enableLevelBow && byEntity is EntityPlayer)
        {
            // Saving aim accurracy
            byEntity.Attributes.SetFloat("old_aimingAccuracy", byEntity.Attributes.GetFloat("aimingAccuracy"));
            // Setting new aim accuracy
            byEntity.Attributes.SetFloat("aimingAccuracy", Configuration.BowGetAimAccuracyByEXP((ulong)byEntity.WatchedAttributes.GetLong("LevelUP_Bow", 0)));
        }
    }
    // Overwrite Bow shot finish
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemBow), "OnHeldInteractStop")]
    public static void OnHeldInteractStopBowFinish(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
    {
        // Reset aiming accuracy
        if (Configuration.enableLevelBow && byEntity is EntityPlayer)
            byEntity.Attributes.SetFloat("aimingAccuracy", byEntity.Attributes.GetFloat("old_aimingAccuracy"));
    }
    #endregion
    #region spear
    // Overwrite Spear shot start
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ItemSpear), "OnHeldInteractStop")]
    public static void OnHeldInteractStopSpearStart(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
    {
        if (Configuration.enableLevelSpear && byEntity is EntityPlayer)
        {
            // Saving aim accurracy
            byEntity.Attributes.SetFloat("old_aimingAccuracy", byEntity.Attributes.GetFloat("aimingAccuracy"));
            // Setting new aim accuracy
            byEntity.Attributes.SetFloat("aimingAccuracy", Configuration.SpearGetAimAccuracyByEXP((ulong)byEntity.WatchedAttributes.GetLong("LevelUP_Spear", 0)));
        }
    }
    // Overwrite Spear shot finish
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemSpear), "OnHeldInteractStop")]
    public static void OnHeldInteractStopSpearFinish(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
    {
        // Reset aiming accuracy
        if (Configuration.enableLevelSpear && byEntity is EntityPlayer)
            byEntity.Attributes.SetFloat("aimingAccuracy", byEntity.Attributes.GetFloat("old_aimingAccuracy"));
    }

    #endregion
}