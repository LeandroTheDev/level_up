using System;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Shared;

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
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Entity), "ReceiveDamage")]
    public static bool ReceiveDamage(Entity __instance, DamageSource damageSource, float damage)
    {
        // Player Does Damage
        // Checking if damage sources is from a player
        if (damageSource.SourceEntity is EntityPlayer || damageSource.GetCauseEntity() is EntityPlayer && __instance.Api.World.Side == EnumAppSide.Server)
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
                damage *= Configuration.HunterGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Hunter"));
                #endregion

                #region cutlery
                // Increase the damage if actual tool is a knife
                if (player.InventoryManager.ActiveTool == EnumTool.Knife)
                {
                    damage *= Configuration.CutleryGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Cutlery"));
                    // Increase exp for using cutlery weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Cutlery_Hit");
                    else instance.clientAPI?.channel.SendPacket<string>("Increase_Cutlery_Hit");
                };
                #endregion

                #region axe
                // Increase the damage if actual tool is a axe
                if (player.InventoryManager.ActiveTool == EnumTool.Axe)
                {
                    damage *= Configuration.AxeGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Axe"));
                    // Increase exp for using axe weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Axe_Hit");
                    else instance.clientAPI?.channel.SendPacket<string>("Increase_Axe_Hit");
                };
                #endregion

                #region pickaxe
                // Increase the damage if actual tool is a pickaxe
                if (player.InventoryManager.ActiveTool == EnumTool.Pickaxe)
                {
                    damage *= Configuration.PickaxeGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Pickaxe"));
                    // Increase exp for using pickaxe weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Pickaxe_Hit");
                    else instance.clientAPI?.channel.SendPacket<string>("Increase_Pickaxe_Hit");
                };
                #endregion

                #region shovel
                // Increase the damage if actual tool is a shovel
                if (player.InventoryManager.ActiveTool == EnumTool.Shovel)
                {
                    damage *= Configuration.ShovelGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Shovel"));
                    // Increase exp for using shovel weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Shovel_Hit");
                    else instance.clientAPI?.channel.SendPacket<string>("Increase_Shovel_Hit");
                };
                #endregion

                #region spear
                // Increase the damage if actual tool is a spear
                if (player.InventoryManager.ActiveTool == EnumTool.Spear)
                {
                    damage *= Configuration.SpearGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Spear"));
                    // Increase exp for using spear weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Spear_Hit");
                    else instance.clientAPI?.channel.SendPacket<string>("Increase_Spear_Hit");
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
                if (itemDamage != null && itemDamage.GetName().Contains("arrow"))
                {
                    damage *= Configuration.BowGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Bow"));
                    // Increase exp for using bow weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Bow_Hit");
                    else instance.clientAPI?.channel.SendPacket<string>("Increase_Bow_Hit");
                };
                #endregion

                #region spear
                // Increase the damage if the damage source is from any spear
                if (itemDamage != null && itemDamage.GetName().Contains("spear"))
                {
                    damage *= Configuration.SpearGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Spear"));
                    // Increase exp for using spear weapons
                    if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Spear_Hit");
                    else instance.clientAPI?.channel.SendPacket<string>("Increase_Spear_Hit");
                };
                #endregion
            }
            // Invalid
            else Debug.Log($"ERROR: Invalid damage type in OverwriteDamageInteraction, cause entity is invalid: {damageSource.GetCauseEntity()} or source entity is invalid: {damageSource.SourceEntity}");
        }

        // Player Receive Damage
        // Checking if received damage is a player
        if (__instance.Api.World.Side == EnumAppSide.Server && __instance.Api.World.GetEntityById(__instance.EntityId) is EntityPlayer)
        {
            // To do
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
        if (itemslot == null || byEntity == null) return true;

        // Check if the entity is a player and if this code is running on the server
        if (byEntity is EntityPlayer && world.Side == EnumAppSide.Server)
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
            EntityPlayer playerEntity = byEntity as EntityPlayer;
            // Get change of not using durability
            switch (itemslot.Itemstack?.Item?.Tool)
            {
                case EnumTool.Bow: return !Configuration.BowRollChanceToNotReduceDurabilityByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Bow"));
                case EnumTool.Axe: return !Configuration.AxeRollChanceToNotReduceDurabilityByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Axe"));
                case EnumTool.Knife: return !Configuration.CutleryRollChanceToNotReduceDurabilityByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Cutlery"));
                case EnumTool.Pickaxe: return !Configuration.PickaxeRollChanceToNotReduceDurabilityByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Pickaxe"));
                case EnumTool.Shovel: return !Configuration.ShovelRollChanceToNotReduceDurabilityByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Shovel"));
                case EnumTool.Spear: return !Configuration.SpearRollChanceToNotReduceDurabilityByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Spear"));
            }
        }
        return true;
    }
}