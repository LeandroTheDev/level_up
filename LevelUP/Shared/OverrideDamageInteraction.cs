using System;
using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Shared;

#pragma warning disable IDE0060
[HarmonyPatchCategory("levelup_damageinteraction")]
class OverwriteDamageInteraction
{
    #region compatibility
    public static float LevelUP_DamageInteraction_Compatibility_ExtendDamageFinish_ReceiveDamage = 0f;
    public static float LevelUP_DamageInteraction_Compatibility_MultiplyDamageFinish_ReceiveDamage = 0f;
    #endregion

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
    public static void ReceiveDamageStart(Entity __instance, DamageSource damageSource, ref float damage)
    {

        #region compatibility
        // Compatibility Layer Start Calculation
        float compatibilityStartDamage = __instance.Attributes.GetFloat("LevelUP_DamageInteraction_Compatibility_ExtendDamageStart_ReceiveDamage");
        if (compatibilityStartDamage != 0f)
            // Receive damage by the compatibility layer
            damage += compatibilityStartDamage;
        // Compatibility Layer Multiply Final Calculation
        float compatibilityStartDamageMultiply = __instance.Attributes.GetFloat("LevelUP_DamageInteraction_Compatibility_MultiplyDamageStart_ReceiveDamage");
        if (compatibilityStartDamageMultiply != 0f)
            // Receive damage by the compatibility layer
            damage += damage * compatibilityStartDamageMultiply;
        #endregion


        // Damage bug treatment
        if (damage > 0 && __instance.ShouldReceiveDamage(damageSource, damage))
        {

            // Player Does Damage
            // Checking if damage sources is from a player and from a server and if entity is alive
            if (damageSource.SourceEntity is EntityPlayer || damageSource.GetCauseEntity() is EntityPlayer && __instance.World.Side == EnumAppSide.Server && __instance.Alive)
            {

                if (Configuration.enableExtendedLog)
                    Debug.Log($"{(damageSource.SourceEntity as EntityPlayer)?.GetName() ?? "PlayerProjectile"} previous damage: {damage}");

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

                    switch (player.InventoryManager.ActiveTool)
                    {
                        #region knife
                        case EnumTool.Knife:
                            if (Configuration.enableLevelKnife)
                            {
                                damage *= Configuration.KnifeGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Knife"));
                                // Increase exp for using knife weapons
                                if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, "Increase_Knife_Hit");
                                // Single player treatment
                                else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                                    instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Knife_Hit&lanplayername={player.PlayerName}");

                            };
                            break;
                        #endregion
                        #region axe
                        case EnumTool.Axe:
                            if (Configuration.enableLevelAxe)
                            {
                                damage *= Configuration.AxeGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Axe"));
                                // Increase exp for using axe weapons
                                if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, "Increase_Axe_Hit");
                                // Single player treatment
                                else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                                    instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Axe_Hit&lanplayername={player.PlayerName}");

                            };
                            break;
                        #endregion
                        #region pickaxe
                        case EnumTool.Pickaxe:
                            if (Configuration.enableLevelPickaxe)
                            {
                                damage *= Configuration.PickaxeGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe"));
                                // Increase exp for using pickaxe weapons
                                if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, "Increase_Pickaxe_Hit");
                                // Single player treatment
                                else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                                    instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Pickaxe_Hit&lanplayername={player.PlayerName}");

                            };
                            break;
                        #endregion
                        #region shovel
                        case EnumTool.Shovel:
                            if (Configuration.enableLevelShovel)
                            {
                                damage *= Configuration.ShovelGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Shovel"));
                                // Increase exp for using shovel weapons
                                if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, "Increase_Shovel_Hit");
                                // Single player treatment
                                else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                                    instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Shovel_Hit&lanplayername={player.PlayerName}");

                            };
                            break;
                        #endregion
                        #region spear
                        case EnumTool.Spear:
                            if (Configuration.enableLevelSpear)
                            {
                                damage *= Configuration.SpearGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Spear"));
                                // Increase exp for using spear weapons
                                if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, "Increase_Spear_Hit");
                                // Single player treatment
                                else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                                    instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Spear_Hit&lanplayername={player.PlayerName}");

                            };
                            break;
                        #endregion
                        #region hammer
                        case EnumTool.Hammer:
                            if (Configuration.enableLevelHammer)
                            {
                                damage *= Configuration.HammerGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Hammer"));
                                // Increase exp for using hammer weapons
                                if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, "Increase_Hammer_Hit");
                                // Single player treatment
                                else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                                    instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Hammer_Hit&lanplayername={player.PlayerName}");

                            };
                            break;
                        #endregion
                        #region sword
                        case EnumTool.Sword:
                            if (Configuration.enableLevelSword)
                            {
                                damage *= Configuration.SwordGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Sword"));
                                // Increase exp for using sword weapons
                                if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, "Increase_Sword_Hit");
                                // Single player treatment
                                else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                                    instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Sword_Hit&lanplayername={player.PlayerName}");

                            };
                            break;
                            #endregion
                    }


                    #region hand
                    if (Configuration.enableLevelHand && player.InventoryManager.ActiveHotbarSlot != null)
                    {
                        // Check if the active slot is empty
                        if (player.InventoryManager.ActiveHotbarSlot.Itemstack == null)
                        {
                            damage *= Configuration.HandGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Hand"));
                            // Increase exp for using hand
                            if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, "Increase_Hand_Hit");
                            // Single player treatment
                            else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                                instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Hand_Hit&lanplayername={player.PlayerName}");
                        }
                    }
                    #endregion

                }
                // Ranged Action
                else if (damageSource.GetCauseEntity() is EntityPlayer && damageSource.SourceEntity is EntityProjectile)
                {

                    // Get entities
                    EntityPlayer playerEntity = damageSource.GetCauseEntity() as EntityPlayer;

                    if (damageSource.SourceEntity is EntityProjectile itemDamage)
                    {


                        // Get player instance
                        IPlayer player = __instance.Api.World.PlayerByUid(playerEntity.PlayerUID);

                        #region bow
                        // Increase the damage if the damage source is from any arrow
                        if (Configuration.enableLevelBow && itemDamage.GetName().Contains("arrow"))
                        {
                            damage *= Configuration.BowGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow"));
                            // Increase exp for using bow weapons
                            if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, "Increase_Bow_Hit");
                            // Single player treatment
                            else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                                instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Bow_Hit&lanplayername={player.PlayerName}");

                        };
                        #endregion

                        #region spear
                        // Increase the damage if the damage source is from any spear
                        if (Configuration.enableLevelSpear && itemDamage.GetName().Contains("spear"))
                        {
                            damage *= Configuration.SpearGetDamageMultiplyByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_Spear"));
                            // Increase exp for using spear weapons
                            if (player is IServerPlayer && instance.serverAPI != null) instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, "Increase_Spear_Hit");
                            // Single player treatment
                            else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)

                                instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Spear_Hit&lanplayername={player.PlayerName}");

                        };
                        #endregion

                    }
                }
                // Invalid
                else if (Configuration.enableExtendedLog)
                    Debug.Log($"WARNING: Invalid damage type in OverwriteDamageInteraction, cause entity is unhandled: {damageSource.GetCauseEntity()} or source entity is unhandled: {damageSource.SourceEntity}");
                if (Configuration.enableExtendedLog)
                    Debug.Log($"{(damageSource.SourceEntity as EntityPlayer)?.GetName() ?? "PlayerProjectile"} final damage: {damage}");
            }


            #region compatibility
            // Compatibility Layer Extend Final Calculation
            float compatibilityFinalDamage = __instance.Attributes.GetFloat("LevelUP_DamageInteraction_Compatibility_ExtendDamageFinish_ReceiveDamage");
            if (compatibilityFinalDamage != 0f)
                // Receive damage by the compatibility layer
                damage += compatibilityFinalDamage;

            // Compatibility Layer Multiply Final Calculation
            float compatibilityFinalDamageMultiply = __instance.Attributes.GetFloat("LevelUP_DamageInteraction_Compatibility_MultiplyDamageFinish_ReceiveDamage");
            if (compatibilityFinalDamageMultiply != 0f)
                // Receive damage by the compatibility layer
                damage += damage * compatibilityFinalDamageMultiply;
            #endregion
            // Player Receive Damage
            // Checking if received damage is a player and if is a server and if is alive
            if (__instance is EntityPlayer && __instance.World.Side == EnumAppSide.Server && __instance.Alive)
            {


                if (Configuration.enableExtendedLog)
                    Debug.Log($"{(damageSource.SourceEntity as EntityPlayer)?.GetName()} received damage: {damage}");

                // Get player source
                EntityPlayer playerEntity = __instance as EntityPlayer;
                // Get player instance
                IPlayer player = __instance.World.PlayerByUid(playerEntity.PlayerUID);

                #region vitality
                if (Configuration.enableLevelVitality && damage < Configuration.DamageLimitVitality)
                {
                    // Check if damage is bigger than player max health
                    float damageCalculation = damage;
                    float playerMaxHealth = playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                    // If is set the damage experience limit to the player max health
                    if (playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < damage) damageCalculation = playerMaxHealth;
                    if (Configuration.enableExtendedLog)
                        Debug.Log($"Vitality damage received: {damage} on: {player.PlayerName}, final damage calculation: {damageCalculation}");
                    // Dedicated server
                    if (player is IServerPlayer && instance.serverAPI != null)
                        instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, $"Increase_Vitality_Hit&forceexp={Configuration.VitalityEXPEarnedByDAMAGE(damageCalculation)}");
                    // Single player treatment
                    else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                        instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Vitality_Hit&forceexp={Configuration.VitalityEXPEarnedByDAMAGE(damageCalculation)}&lanplayername={player.PlayerName}");

                }
                #endregion
                // Check if the damage received is from a valid entity source damage
                // in others cases the armor shouldn't reduce damage
                if (damageSource.GetCauseEntity() != null || damageSource.SourceEntity != null)
                {
                    #region inventory armors
                    List<string> equippedArmors = [];
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
                    #endregion
                    #region leatherarmor
                    if (Configuration.enableLevelLeatherArmor && damage < Configuration.DamageLimitLeatherArmor)
                    {
                        // Check if damage is bigger than player max health
                        float damageCalculation = damage;
                        float playerMaxHealth = playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                        // If is set the damage experience limit to the player max health
                        if (playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < damage) damageCalculation = playerMaxHealth;

                        // Swipe all inventorys to receive armor multiply
                        double multiply = 1.0;
                        foreach (string armorCode in equippedArmors)
                        {
                            // Check if the armor contains experience
                            double value = Configuration.expMultiplyHitLeatherArmor.GetValueOrDefault(armorCode, 0.0);
                            multiply += value;

                            if (Configuration.enableExtendedLog && value != 0.0)
                                Debug.Log($"{player.PlayerName} received damage using: {armorCode} as armor");
                        }

                        // Check if player is wearing some leather armor
                        if (multiply > 1.0)
                        {
                            // Dedicated Servers
                            if (player is IServerPlayer && instance.serverAPI != null)
                                instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, $"Increase_LeatherArmor_Hit&forceexp={(int)Math.Round(Configuration.LeatherArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}");

                            // Single player treatment
                            else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                                instance.clientAPI.compatibilityChannel.SendPacket($"Increase_LeatherArmor_Hit&forceexp={(int)Math.Round(Configuration.LeatherArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}&lanplayername={player.PlayerName}");


                            float damageReduction = Configuration.LeatherArmorDamageReductionByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_LeatherArmor")) * (float)multiply;
                            damage -= damageReduction;
                            if (Configuration.enableExtendedLog) Debug.Log($"{player.PlayerName} reduced {damageReduction} damage by leather armor level");
                        }
                    }
                    #endregion
                    #region chainarmor
                    if (Configuration.enableLevelChainArmor && damage < Configuration.DamageLimitChainArmor)
                    {
                        // Check if damage is bigger than player max health
                        float damageCalculation = damage;
                        float playerMaxHealth = playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                        // If is set the damage experience limit to the player max health
                        if (playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < damage) damageCalculation = playerMaxHealth;

                        // Swipe all inventorys to receive armor multiply
                        double multiply = 1.0;
                        foreach (string armorCode in equippedArmors)
                        {
                            // Check if the armor contains experience
                            double value = Configuration.expMultiplyHitChainArmor.GetValueOrDefault(armorCode, 0.0);
                            multiply += value;

                            if (Configuration.enableExtendedLog && value != 0.0)
                                Debug.Log($"{player.PlayerName} received damage using: {armorCode} as armor");
                        }

                        // Check if player is wearing some chain armor
                        if (multiply > 1.0)
                        {
                            // Dedicated Servers
                            if (player is IServerPlayer && instance.serverAPI != null)
                                instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, $"Increase_ChainArmor_Hit&forceexp={(int)Math.Round(Configuration.ChainArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}");
                            // Single player treatment
                            else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                                instance.clientAPI.compatibilityChannel.SendPacket($"Increase_ChainArmor_Hit&forceexp={(int)Math.Round(Configuration.ChainArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}&lanplayername={player.PlayerName}");

                            float damageReduction = Configuration.ChainArmorDamageReductionByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_ChainArmor")) * (float)multiply;
                            damage -= damageReduction;
                            if (Configuration.enableExtendedLog) Debug.Log($"{player.PlayerName} reduced {damageReduction} damage by chain armor level");
                        }
                    }
                    #endregion
                    #region brigandinearmor
                    if (Configuration.enableLevelBrigandineArmor && damage < Configuration.DamageLimitBrigandineArmor)
                    {
                        // Check if damage is bigger than player max health
                        float damageCalculation = damage;
                        float playerMaxHealth = playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                        // If is set the damage experience limit to the player max health
                        if (playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < damage) damageCalculation = playerMaxHealth;

                        // Swipe all inventorys to receive armor multiply
                        double multiply = 1.0;
                        foreach (string armorCode in equippedArmors)
                        {
                            // Check if the armor contains experience
                            double value = Configuration.expMultiplyHitBrigandineArmor.GetValueOrDefault(armorCode, 0.0);
                            multiply += value;

                            if (Configuration.enableExtendedLog && value != 0.0)
                                Debug.Log($"{player.PlayerName} received damage using: {armorCode} as armor");
                        }

                        // Check if player is wearing some brigandine armor
                        if (multiply > 1.0)
                        {
                            // Dedicated Servers
                            if (player is IServerPlayer && instance.serverAPI != null)
                                instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, $"Increase_BrigandineArmor_Hit&forceexp={(int)Math.Round(Configuration.BrigandineArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}");
                            // Single player treatment
                            else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                                instance.clientAPI.compatibilityChannel.SendPacket($"Increase_BrigandineArmor_Hit&forceexp={(int)Math.Round(Configuration.BrigandineArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}&lanplayername={player.PlayerName}");

                            float damageReduction = Configuration.BrigandineArmorDamageReductionByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_BrigandineArmor")) * (float)multiply;
                            damage -= damageReduction;
                            if (Configuration.enableExtendedLog) Debug.Log($"{player.PlayerName} reduced {damageReduction} damage by brigandine armor level");
                        }
                    }
                    #endregion
                    #region platearmor
                    if (Configuration.enableLevelPlateArmor && damage < Configuration.DamageLimitPlateArmor)
                    {
                        // Check if damage is bigger than player max health
                        float damageCalculation = damage;
                        float playerMaxHealth = playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                        // If is set the damage experience limit to the player max health
                        if (playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < damage) damageCalculation = playerMaxHealth;

                        // Swipe all inventorys to receive armor multiply
                        double multiply = 1.0;
                        foreach (string armorCode in equippedArmors)
                        {
                            // Check if the armor contains experience
                            double value = Configuration.expMultiplyHitPlateArmor.GetValueOrDefault(armorCode, 0.0);
                            multiply += value;

                            if (Configuration.enableExtendedLog && value != 0.0)
                                Debug.Log($"{player.PlayerName} received damage using: {armorCode} as armor");
                        }

                        // Check if player is wearing some plate armor
                        if (multiply > 1.0)
                        {
                            // Dedicated Servers
                            if (player is IServerPlayer && instance.serverAPI != null)
                                instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, $"Increase_PlateArmor_Hit&forceexp={(int)Math.Round(Configuration.PlateArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}");
                            // Single player treatment
                            else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                                instance.clientAPI.compatibilityChannel.SendPacket($"Increase_PlateArmor_Hit&forceexp={(int)Math.Round(Configuration.PlateArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}&lanplayername={player.PlayerName}");

                            float damageReduction = Configuration.PlateArmorDamageReductionByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_PlateArmor")) * (float)multiply;
                            damage -= damageReduction;
                            if (Configuration.enableExtendedLog) Debug.Log($"{player.PlayerName} reduced {damageReduction} damage by plate armor level");
                        }
                    }
                    #endregion
                    #region scalearmor
                    if (Configuration.enableLevelScaleArmor && damage < Configuration.DamageLimitScaleArmor)
                    {
                        // Check if damage is bigger than player max health
                        float damageCalculation = damage;
                        float playerMaxHealth = playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                        // If is set the damage experience limit to the player max health
                        if (playerEntity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < damage) damageCalculation = playerMaxHealth;

                        // Swipe all inventorys to receive armor multiply
                        double multiply = 1.0;
                        foreach (string armorCode in equippedArmors)
                        {
                            // Check if the armor contains experience
                            double value = Configuration.expMultiplyHitScaleArmor.GetValueOrDefault(armorCode, 0.0);
                            multiply += value;

                            if (Configuration.enableExtendedLog && value != 0.0)
                                Debug.Log($"{player.PlayerName} received damage using: {armorCode} as armor");
                        }

                        // Check if player is wearing some scale armor
                        if (multiply > 1.0)
                        {
                            // Dedicated Servers
                            if (player is IServerPlayer && instance.serverAPI != null)
                                instance.serverAPI?.OnExperienceEarned(player as IServerPlayer, $"Increase_ScaleArmor_Hit&forceexp={(int)Math.Round(Configuration.ScaleArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}");
                            // Single player treatment
                            else if (instance.clientAPI != null && instance.clientAPI.api.IsSinglePlayer && singlePlayerDoubleCheck)
                                instance.clientAPI.compatibilityChannel.SendPacket($"Increase_ScaleArmor_Hit&forceexp={(int)Math.Round(Configuration.ScaleArmorBaseEXPEarnedByDAMAGE(damageCalculation) * multiply)}&lanplayername={player.PlayerName}");

                            float damageReduction = Configuration.ScaleArmorDamageReductionByLevel(playerEntity.WatchedAttributes.GetInt("LevelUP_Level_ScaleArmor")) * (float)multiply;
                            damage -= damageReduction;
                            if (Configuration.enableExtendedLog) Debug.Log($"{player.PlayerName} reduced {damageReduction} damage by scale armor level");
                        }
                    }
                    #endregion

                }

                if (Configuration.enableExtendedLog)
                    Debug.Log($"{(damageSource.SourceEntity as EntityPlayer)?.GetName()} received final damage: {damage}");

            };
            // Double check bug only if is a player hitting in single player
            if (damageSource.SourceEntity is EntityPlayer || damageSource.GetCauseEntity() is EntityPlayer)
                singlePlayerDoubleCheck = !singlePlayerDoubleCheck;

            // If the armor reduces less than 0, just change to 0
            if (damage < 0) damage = 0;
        }
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Entity), "ReceiveDamage")]
    public static void ReceiveDamageFinish(Entity __instance, DamageSource damageSource, float damage)
    {
        // Clean Compatibility layer
        __instance.Attributes.RemoveAttribute("LevelUP_DamageInteraction_Compatibility_ExtendDamageStart_ReceiveDamage");
        __instance.Attributes.RemoveAttribute("LevelUP_DamageInteraction_Compatibility_ExtendDamageFinish_ReceiveDamage");
        __instance.Attributes.RemoveAttribute("LevelUP_DamageInteraction_Compatibility_MultiplyDamageStart_ReceiveDamage");
        __instance.Attributes.RemoveAttribute("LevelUP_DamageInteraction_Compatibility_MultiplyDamageFinish_ReceiveDamage");
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
        if (instance.serverAPI != null && byEntity != null)
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
    public static void OnHeldInteractBowStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
    {
        if (Configuration.enableLevelBow && byEntity is EntityPlayer)
        {
            // Setting new aim accuracy
            byEntity.Attributes.SetFloat("aimingAccuracy", Configuration.BowGetAimAccuracyByLevel(byEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow", 0)));
            if (Configuration.enableExtendedLog)
                Debug.Log($"Bow Accuracy: {Configuration.BowGetAimAccuracyByLevel(byEntity.WatchedAttributes.GetInt("LevelUP_Level_Bow", 0))}");
        }
    }
    #endregion
    #region spear
    // Overwrite Spear shot start
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ItemSpear), "OnHeldInteractStop")]
    public static void OnHeldInteractSpearStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
    {
        if (Configuration.enableLevelSpear && byEntity is EntityPlayer)
        {
            // Setting new aim accuracy
            byEntity.Attributes.SetFloat("aimingAccuracy", Configuration.SpearGetAimAccuracyByLevel(byEntity.WatchedAttributes.GetInt("LevelUP_Level_Spear", 0)));
            if (Configuration.enableExtendedLog)
                Debug.Log($"Spear Accuracy: {Configuration.SpearGetAimAccuracyByLevel(byEntity.WatchedAttributes.GetInt("LevelUP_Level_Spear", 0))}");
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

            // Reduces the damage received more than normal based on shield level
            double damageReduced = damage * Configuration.ShieldGetReductionMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Shield"));
            damage -= (float)damageReduced;
            if (damage < 0) damage = 0;
            if (Configuration.enableExtendedLog) Debug.Log($"{player.PlayerName} reduced: {damageReduced} in shield damage");

            // Servers
            if (instance.serverAPI != null)
                instance.serverAPI.OnExperienceEarned(player as IServerPlayer, "Increase_Shield_Hit");
            // Single player treatment
            else if (instance.clientAPI?.api.IsSinglePlayer ?? false)
                instance.clientAPI.compatibilityChannel.SendPacket($"Increase_Shield_Hit&lanplayername={player.PlayerName}");
        }
    }
    #endregion
}
