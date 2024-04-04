using System;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace LevelUP.Shared;

[HarmonyPatchCategory("levelup_damageinteraction")]
class OverrideDamageInteraction
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

    // Override Damage Interaction
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Entity), "ReceiveDamage")]
    public static bool ReceiveDamage(Entity __instance, DamageSource damageSource, float damage)
    {
        // Checking if the damage is from a player
        if (damageSource.SourceEntity is EntityPlayer || damageSource.GetCauseEntity() is EntityPlayer)
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
                    instance.clientAPI?.channel.SendPacket<string>("Increase_Cutlery_Hit");
                };
                #endregion

                #region axe
                // Increase the damage if actual tool is a axe
                if (player.InventoryManager.ActiveTool == EnumTool.Axe)
                {
                    damage *= Configuration.AxeGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Axe"));
                    // Increase exp for using axe weapons
                    instance.clientAPI?.channel.SendPacket<string>("Increase_Axe_Hit");
                };
                #endregion

                #region pickaxe
                // Increase the damage if actual tool is a pickaxe
                if (player.InventoryManager.ActiveTool == EnumTool.Pickaxe)
                {
                    damage *= Configuration.PickaxeGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Pickaxe"));
                    // Increase exp for using pickaxe weapons
                    instance.clientAPI?.channel.SendPacket<string>("Increase_Pickaxe_Hit");
                };
                #endregion

                #region shovel
                // Increase the damage if actual tool is a shovel
                if (player.InventoryManager.ActiveTool == EnumTool.Shovel)
                {
                    damage *= Configuration.ShovelGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Shovel"));
                    // Increase exp for using shovel weapons
                    instance.clientAPI?.channel.SendPacket<string>("Increase_Shovel_Hit");
                };
                #endregion

                #region spear
                // Increase the damage if actual tool is a spear
                if (player.InventoryManager.ActiveTool == EnumTool.Spear)
                {
                    damage *= Configuration.SpearGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Spear"));
                    // Increase exp for using spear weapons
                    instance.clientAPI?.channel.SendPacket<string>("Increase_Spear_Hit");
                }
                #endregion
            }
            // Ranged Action
            else if (damageSource.GetCauseEntity() is EntityPlayer && damageSource.SourceEntity is EntityProjectile)
            {
                EntityPlayer playerEntity = damageSource.GetCauseEntity() as EntityPlayer;
                EntityProjectile itemDamage = damageSource.SourceEntity as EntityProjectile;                

                #region bow
                // Increase the damage if the damage source is from any arrow
                if (itemDamage != null && itemDamage.GetName().Contains("Arrow"))
                {
                    damage *= Configuration.BowGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Bow"));
                    // Increase exp for using bow weapons
                    instance.clientAPI?.channel.SendPacket<string>("Increase_Bow_Hit");
                };
                #endregion

                #region spear
                // Increase the damage if the damage source is from any spear
                if (itemDamage != null && itemDamage.GetName().Contains("spear"))
                {
                    damage *= Configuration.SpearGetDamageMultiplyByEXP(playerEntity.WatchedAttributes.GetInt("LevelUP_Spear"));
                    // Increase exp for using spear weapons
                    instance.clientAPI?.channel.SendPacket<string>("Increase_Spear_Hit");
                };
                #endregion
            }
            // Invalid
            else Debug.Log($"ERROR: Invalid damage type in OverrideDamageInteraction, cause entity is invalid: {damageSource.GetCauseEntity()} or source entity is invalid: {damageSource.SourceEntity}");
        }

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
}