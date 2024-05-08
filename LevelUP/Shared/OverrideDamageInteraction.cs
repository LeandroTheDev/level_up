using System;
using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
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
    public static bool ReceiveDamageStart(Entity __instance, DamageSource damageSource, float damage)
    {
        #region compatibility
        // Compatibility Layer Start Calculation
        double compatibilityStartDamage = __instance.Stats.GetBlended("LevelUP_DamageInteraction_Compatibility_ExtendDamageStart_ReceiveDamage");
        if (compatibilityStartDamage != 0)
        {
            // Lose damage by the compatibility layer
            if (compatibilityStartDamage < 0) damage -= (float)compatibilityStartDamage;
            // Receive damage by the compatibility layer
            else damage += (float)compatibilityStartDamage;
        }
        #endregion

        // Damage bug treatment
        if (damage > 0 && __instance.ShouldReceiveDamage(damageSource, damage))
        {
            // Player Does Damage
            // Checking if damage sources is from a player and from a server and if entity is alive
            if (damageSource.SourceEntity is EntityPlayer || damageSource.GetCauseEntity() is EntityPlayer && __instance.World.Side == EnumAppSide.Server && __instance.Alive)
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
                    damage *= Configuration.HunterGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Hunter"));
                    #endregion

                    #region knife
                    // Increase the damage if actual tool is a knife
                    if (Configuration.enableLevelKnife && player.InventoryManager.ActiveTool == EnumTool.Knife)
                    {
                        damage *= Configuration.KnifeGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Knife"));
                        // Increase exp for using knife weapons
                        if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Knife_Hit");
                        // Single player treatment
                        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                            instance.clientAPI.channel.SendPacket($"Increase_Knife_Hit&lanplayername={player.PlayerName}");

                    };
                    #endregion

                    #region axe
                    // Increase the damage if actual tool is a axe
                    if (Configuration.enableLevelAxe && player.InventoryManager.ActiveTool == EnumTool.Axe)
                    {
                        damage *= Configuration.AxeGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Axe"));
                        // Increase exp for using axe weapons
                        if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Axe_Hit");
                        // Single player treatment
                        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                            instance.clientAPI.channel.SendPacket($"Increase_Axe_Hit&lanplayername={player.PlayerName}");

                    };
                    #endregion

                    #region pickaxe
                    // Increase the damage if actual tool is a pickaxe
                    if (Configuration.enableLevelPickaxe && player.InventoryManager.ActiveTool == EnumTool.Pickaxe)
                    {
                        damage *= Configuration.PickaxeGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe"));
                        // Increase exp for using pickaxe weapons
                        if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Pickaxe_Hit");
                        // Single player treatment
                        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                            instance.clientAPI.channel.SendPacket($"Increase_Pickaxe_Hit&lanplayername={player.PlayerName}");

                    };
                    #endregion

                    #region shovel
                    // Increase the damage if actual tool is a shovel
                    if (Configuration.enableLevelShovel && player.InventoryManager.ActiveTool == EnumTool.Shovel)
                    {
                        damage *= Configuration.ShovelGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Shovel"));
                        // Increase exp for using shovel weapons
                        if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Shovel_Hit");
                        // Single player treatment
                        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                            instance.clientAPI.channel.SendPacket($"Increase_Shovel_Hit&lanplayername={player.PlayerName}");

                    };
                    #endregion

                    #region spear
                    // Increase the damage if actual tool is a spear
                    if (Configuration.enableLevelSpear && player.InventoryManager.ActiveTool == EnumTool.Spear)
                    {
                        damage *= Configuration.SpearGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Spear"));
                        // Increase exp for using spear weapons
                        if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Spear_Hit");
                        // Single player treatment
                        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                            instance.clientAPI.channel.SendPacket($"Increase_Spear_Hit&lanplayername={player.PlayerName}");

                    }
                    #endregion

                    #region hammer
                    // Increase the damage if actual tool is a hammer
                    if (Configuration.enableLevelHammer && player.InventoryManager.ActiveTool == EnumTool.Hammer)
                    {
                        damage *= Configuration.HammerGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Hammer"));
                        // Increase exp for using hammer weapons
                        if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Hammer_Hit");
                        // Single player treatment
                        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                            instance.clientAPI.channel.SendPacket($"Increase_Hammer_Hit&lanplayername={player.PlayerName}");

                    }
                    #endregion

                    #region sword
                    // Increase the damage if actual tool is a sword
                    if (Configuration.enableLevelSword && player.InventoryManager.ActiveTool == EnumTool.Sword)
                    {
                        damage *= Configuration.SwordGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Sword"));
                        // Increase exp for using sword weapons
                        if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Sword_Hit");
                        // Single player treatment
                        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                            instance.clientAPI.channel.SendPacket($"Increase_Sword_Hit&lanplayername={player.PlayerName}");

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
                        damage *= Configuration.BowGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow"));
                        // Increase exp for using bow weapons
                        if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Bow_Hit");
                        // Single player treatment
                        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                            instance.clientAPI.channel.SendPacket($"Increase_Bow_Hit&lanplayername={player.PlayerName}");

                    };
                    #endregion

                    #region spear
                    // Increase the damage if the damage source is from any spear
                    if (Configuration.enableLevelSpear && itemDamage != null && itemDamage.GetName().Contains("spear"))
                    {
                        damage *= Configuration.SpearGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Spear"));
                        // Increase exp for using spear weapons
                        if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnClientMessage(player as IServerPlayer, "Increase_Spear_Hit");
                        // Single player treatment
                        else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                            instance.clientAPI.channel.SendPacket($"Increase_Spear_Hit&lanplayername={player.PlayerName}");

                    };
                    #endregion
                }
                // Invalid
                else Debug.Log($"ERROR: Invalid damage type in OverwriteDamageInteraction, cause entity is invalid: {damageSource.GetCauseEntity()} or source entity is invalid: {damageSource.SourceEntity}");
            }

            #region compatibility
            // Compatibility Layer Final Calculation
            float compatibilityFinalDamage = __instance.Stats.GetBlended("LevelUP_DamageInteraction_Compatibility_ExtendDamageFinish_ReceiveDamage");
            if (compatibilityFinalDamage != 0)
            {
                // Lose damage by the compatibility layer
                if (compatibilityFinalDamage < 0) damage -= (float)compatibilityFinalDamage;
                // Receive damage by the compatibility layer
                else damage += (float)compatibilityFinalDamage;
            }
            // 0 Damage treatment
            if (damage < 0) damage = 0;
            #endregion

            // Player Receive Damage
            // Checking if received damage is a player and if is a server and if is alive
            if (__instance is EntityPlayer && __instance.World.Side == EnumAppSide.Server && __instance.Alive)
            {
                // Get player source
                EntityPlayer playerEntity = __instance as EntityPlayer;
                // Get player instance
                IPlayer player = __instance.World.PlayerByUid(playerEntity.PlayerUID);

                #region vitality
                if (Configuration.enableLevelVitality)
                {
                    // Check if damage is bigger than player max health
                    float damageCalculation = damage;
                    float playerMaxHealth = playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                    // If is set the damage experience limit to the player max health
                    if (playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < damage) damageCalculation = playerMaxHealth;

                    // Dedicated server
                    if (player is IServerPlayer && instance.serverAPI != null)
                        instance.serverAPI?.OnClientMessage(player as IServerPlayer, $"Increase_Vitality_Hit&forceexp={Configuration.VitalityEXPEarnedByDAMAGE(damageCalculation)}");
                    // Single player treatment
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                        instance.clientAPI.channel.SendPacket($"Increase_Vitality_Hit&forceexp={Configuration.VitalityEXPEarnedByDAMAGE(damageCalculation)}&lanplayername={player.PlayerName}");

                }
                #endregion

                // Check if the damage received is from a valid entity source damage
                // in others cases the armor shouldn't reduce damage
                if (damageSource.GetCauseEntity() != null || damageSource.SourceEntity != null)
                {
                    Debug.Log($"Cause: {damageSource.GetCauseEntity()} source: {damageSource.SourceEntity}");
                    #region leatherarmor
                    if (Configuration.enableLevelLeatherArmor)
                    {
                        // Check if damage is bigger than player max health
                        float damageCalculation = damage;
                        float playerMaxHealth = playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                        // If is set the damage experience limit to the player max health
                        if (playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < damage) damageCalculation = playerMaxHealth;

                        // Swipe all inventorys to receive armor multiply
                        double multiply = 1.0;
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

                                    // Check if the armor contains experience
                                    double value = Configuration.expMultiplyHitLeatherArmor.GetValueOrDefault(item.Itemstack.Item.Code.ToString(), 0.0);
                                    multiply += value;
                                    index++;

                                    if (Configuration.enableExtendedLog && value != 0.0)
                                        Debug.Log($"{player.PlayerName} received damage using: {item.Itemstack.Item.Code} as armor");
                                }
                                break;
                            }
                        }

                        // Check if player is wearing some leather armor
                        if (multiply > 1.0)
                        {
                            // Dedicated Servers
                            if (player is IServerPlayer && instance.serverAPI != null)
                                instance.serverAPI?.OnClientMessage(player as IServerPlayer, $"Increase_LeatherArmor_Hit&forceexp={(int)Math.Round(Configuration.LeatherArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}");

                            // Single player treatment
                            else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                                instance.clientAPI.channel.SendPacket($"Increase_LeatherArmor_Hit&forceexp={(int)Math.Round(Configuration.LeatherArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}&lanplayername={player.PlayerName}");


                            float damageReduction = Configuration.LeatherArmorDamageReductionByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor")) * (float)multiply;
                            damage -= damageReduction;
                            if (Configuration.enableExtendedLog) Debug.Log($"{player.PlayerName} reduced {damageReduction} damage by leather armor level");
                        }
                    }
                    #endregion
                    #region chainarmor
                    if (Configuration.enableLevelChainArmor)
                    {
                        // Check if damage is bigger than player max health
                        float damageCalculation = damage;
                        float playerMaxHealth = playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                        // If is set the damage experience limit to the player max health
                        if (playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < damage) damageCalculation = playerMaxHealth;

                        // Swipe all inventorys to receive armor multiply
                        double multiply = 1.0;
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

                                    // Check if the armor contains experience
                                    double value = Configuration.expMultiplyHitChainArmor.GetValueOrDefault(item.Itemstack.Item.Code.ToString(), 0.0);
                                    multiply += value;
                                    index++;

                                    if (Configuration.enableExtendedLog && value != 0.0)
                                        Debug.Log($"{player.PlayerName} received damage using: {item.Itemstack.Item.Code} as armor");
                                }
                                break;
                            }
                        }

                        // Check if player is wearing some chain armor
                        if (multiply > 1.0)
                        {
                            // Dedicated Servers
                            if (player is IServerPlayer && instance.serverAPI != null)
                                instance.serverAPI?.OnClientMessage(player as IServerPlayer, $"Increase_ChainArmor_Hit&forceexp={(int)Math.Round(Configuration.ChainArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}");
                            // Single player treatment
                            else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                                instance.clientAPI.channel.SendPacket($"Increase_ChainArmor_Hit&forceexp={(int)Math.Round(Configuration.ChainArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}&lanplayername={player.PlayerName}");

                            float damageReduction = Configuration.ChainArmorDamageReductionByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor")) * (float)multiply;
                            damage -= damageReduction;
                            if (Configuration.enableExtendedLog) Debug.Log($"{player.PlayerName} reduced {damageReduction} damage by chain armor level");
                        }
                    }
                    #endregion
                }
            };

            // Double check bug only if is a player hitting in single player
            if (damageSource.SourceEntity is EntityPlayer || damageSource.GetCauseEntity() is EntityPlayer)
                singlePlayerDoubleCheck = !singlePlayerDoubleCheck;
        }

        // 0 Damage treatment
        if (damage < 0) damage = 0;

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
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Entity), "ReceiveDamage")]
    public static void ReceiveDamageFinish(Entity __instance, DamageSource damageSource, float damage)
    {
        // Clean Compatibility layer
        __instance.Stats.Remove("LevelUP_DamageInteraction_Compatibility_ExtendDamageStart_ReceiveDamage", "LevelUP_DamageInteraction_Compatibility_ExtendDamageStart_ReceiveDamage");
        __instance.Stats.Remove("LevelUP_DamageInteraction_Compatibility_ExtendDamageFinish_ReceiveDamage", "LevelUP_DamageInteraction_Compatibility_ExtendDamageFinish_ReceiveDamage");
    }

    // Overwrite Durability lost start
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CollectibleObject), "DamageItem")]
    public static bool DamageItemStart(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, int amount = 1)
    {
        // Error treatment
        if ((!Configuration.enableDurabilityMechanic || itemslot == null || byEntity == null) && world.Side != EnumAppSide.Server) return true;

        // Check if the entity is a player and if this code is running on the server
        if (byEntity is EntityPlayer && world.Side == EnumAppSide.Server)
        {
            EntityPlayer playerEntity = byEntity as EntityPlayer;
            // Get change of not using durability
            switch (itemslot.Itemstack?.Collectible?.Tool)
            {
                case EnumTool.Bow: return !Configuration.BowRollChanceToNotReduceDurabilityByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow"));
                case EnumTool.Axe: return !Configuration.AxeRollChanceToNotReduceDurabilityByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Axe"));
                case EnumTool.Knife: return !Configuration.KnifeRollChanceToNotReduceDurabilityByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Knife"));
                case EnumTool.Pickaxe: return !Configuration.PickaxeRollChanceToNotReduceDurabilityByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe"));
                case EnumTool.Shovel: return !Configuration.ShovelRollChanceToNotReduceDurabilityByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Shovel"));
                case EnumTool.Spear: return !Configuration.SpearRollChanceToNotReduceDurabilityByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Spear"));
                case EnumTool.Hammer: return !Configuration.HammerRollChanceToNotReduceDurabilityByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Hammer"));
                case EnumTool.Sword: return !Configuration.SwordRollChanceToNotReduceDurabilityByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Sword"));
            }
        }
        return true;
    }

    // Overwrite Durability lost finish
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CollectibleObject), "DamageItem")]
    public static void DamageItemFinish(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, int amount = 1)
    {
        // Dedicated Server needs to broadcast the durability restoration
        if (instance.serverAPI != null)
        {
            // Refresh player inventory
            foreach (IPlayer iplayer in instance.serverAPI.api.World.AllOnlinePlayers)
            {
                // Find the player instance
                if (iplayer.PlayerName == byEntity.GetName())
                {
                    IServerPlayer player = iplayer as IServerPlayer;
                    // We need to refresh player inventory with restored durability
                    player.BroadcastPlayerData(true);
                    break;
                }
            }
        }
    }

    #region bow
    // Overwrite Projectile impact
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EntityProjectile), "impactOnEntity")]
    public static void ImpactOnEntity(EntityProjectile __instance, Entity entity)
    {
        // Check if bow is enabled and if is from the server side
        if (Configuration.enableLevelBow && __instance.World.Side == EnumAppSide.Server)
        {
            // Check if is a arrow
            if (__instance.Code.ToString().Contains("arrow"))
            {
                // Check if arrow is shotted by a player
                if (__instance.FiredBy is EntityPlayer)
                {
                    EntityPlayer playerEntity = __instance.FiredBy as EntityPlayer;
                    // Change the change based on level
                    __instance.DropOnImpactChance = Configuration.BowGetChanceToNotLoseArrowByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow"));
                }
            }
        }
    }
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
            byEntity.Attributes.SetFloat("aimingAccuracy", Configuration.BowGetAimAccuracyByLevel(byEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow", 0)));
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
            byEntity.Attributes.SetFloat("aimingAccuracy", Configuration.SpearGetAimAccuracyByLevel(byEntity.WatchedAttributes.GetInt("LevelUP_Level_Spear", 0)));
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
    #region shield
    // Overwrite the Shield function end
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ModSystemWearableStats), "applyShieldProtection")]
    public static float ApplyShieldProtectionFinish(float __result, ModSystemWearableStats __instance, IPlayer player, float damage, DamageSource dmgSource)
    {
        if (!Configuration.enableLevelShield) return __result;

        // Pickup the native api
        ICoreAPI api = player.Entity.Api;

        #region native
        double horizontalAngleProtectionRange = 1.0471975803375244;
        ItemSlot[] shieldSlots =
        [
            player.Entity.LeftHandItemSlot,
            player.Entity.RightHandItemSlot
        ];
        for (int i = 0; i < shieldSlots.Length; i++)
        {
            ItemSlot shieldSlot = shieldSlots[i];
            JsonObject attr = shieldSlot.Itemstack?.ItemAttributes?["shield"];
            if (attr == null || !attr.Exists)
            {
                continue;
            }
            string usetype = player.Entity.Controls.Sneak ? "active" : "passive";
            float dmgabsorb = attr["damageAbsorption"][usetype].AsFloat();
            float chance = attr["protectionChance"][usetype].AsFloat();
            (player as IServerPlayer)?.SendMessage(GlobalConstants.DamageLogChatGroup, Lang.Get("{0:0.#} of {1:0.#} damage blocked by shield", Math.Min(dmgabsorb, damage), damage), EnumChatType.Notification);
            double dx;
            double dy;
            double dz;
            if (dmgSource.HitPosition != null)
            {
                dx = dmgSource.HitPosition.X;
                dy = dmgSource.HitPosition.Y;
                dz = dmgSource.HitPosition.Z;
            }
            else if (dmgSource.SourceEntity != null)
            {
                dx = dmgSource.SourceEntity.Pos.X - player.Entity.Pos.X;
                dy = dmgSource.SourceEntity.Pos.Y - player.Entity.Pos.Y;
                dz = dmgSource.SourceEntity.Pos.Z - player.Entity.Pos.Z;
            }
            else
            {
                if (!(dmgSource.SourcePos != null))
                {
                    break;
                }
                dx = dmgSource.SourcePos.X - player.Entity.Pos.X;
                dy = dmgSource.SourcePos.Y - player.Entity.Pos.Y;
                dz = dmgSource.SourcePos.Z - player.Entity.Pos.Z;
            }
            double playerYaw = player.Entity.Pos.Yaw + (float)Math.PI / 2f;
            double playerPitch = player.Entity.Pos.Pitch;
            double attackYaw = Math.Atan2(dx, dz);
            double a = dy;
            float b = (float)Math.Sqrt(dx * dx + dz * dz);
            float attackPitch = (float)Math.Atan2(a, b);
            bool inProtectionRange = (!(Math.Abs(attackPitch) > (float)Math.PI * 13f / 36f)) ? ((double)Math.Abs(GameMath.AngleRadDistance((float)playerYaw, (float)attackYaw)) < horizontalAngleProtectionRange) : (Math.Abs(GameMath.AngleRadDistance((float)playerPitch, attackPitch)) < (float)Math.PI / 6f);
            if (inProtectionRange && api.World.Rand.NextDouble() < (double)chance)
            {
                #endregion
                // Reduces the damage received more than normal based on shield level
                float damageReduction = dmgabsorb * Configuration.ShieldGetReductionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield"));
                damage = Math.Max(0f, damage - damageReduction);
                if (Configuration.enableExtendedLog) Debug.Log($"{player.PlayerName} reduced: {damageReduction} in shield damage");
                #region native

                string loc = shieldSlot.Itemstack.ItemAttributes["blockSound"].AsString("held/shieldblock");
                api.World.PlaySoundAt(AssetLocation.Create(loc, shieldSlot.Itemstack.Collectible.Code.Domain).WithPathPrefixOnce("sounds/").WithPathAppendixOnce(".ogg"), player);
                (api as ICoreServerAPI).Network.BroadcastEntityPacket(player.Entity.EntityId, 200, SerializerUtil.Serialize("shieldBlock" + ((i == 0) ? "L" : "R")));
                if (api.Side == EnumAppSide.Server)
                {
                    #endregion
                    // Roll chance for not losing the durability
                    if (!Configuration.ShieldRollChanceToNotReduceDurabilityByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield")))
                    {
                        #region native
                        shieldSlot.Itemstack.Collectible.DamageItem(api.World, dmgSource.SourceEntity, shieldSlot);
                        shieldSlot.MarkDirty();
                        #endregion
                    }
                }
            }
            // Experience increase
            if (inProtectionRange)
            {
                // Servers
                if (instance.serverAPI != null)
                    instance.serverAPI.OnClientMessage(player as IServerPlayer, "Increase_Shield_Hit");
                // Single player treatment
                else if (instance.clientAPI?.api.IsSinglePlayer ?? false)
                    instance.clientAPI.channel.SendPacket($"Increase_Shield_Hit&lanplayername={player.PlayerName}");
            }
        }
        #region native
        return damage;
        #endregion
    }
    #endregion
}
