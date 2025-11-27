using System;
using LevelUP.Server;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace LevelUP.Shared;

class Instance
{
    internal static LevelHunter levelHunter = new();
    internal static LevelBow levelBow = new();
    internal static LevelSlingshot levelSlingshot = new();
    internal static LevelKnife levelKnife = new();
    internal static LevelAxe levelAxe = new();
    internal static LevelPickaxe levelPickaxe = new();
    internal static LevelShovel levelShovel = new();
    internal static LevelSpear levelSpear = new();
    internal static LevelHammer levelHammer = new();
    internal static LevelSword levelSword = new();
    internal static LevelShield levelShield = new();
    internal static LevelHand levelHand = new();
    internal static LevelFarming levelFarming = new();
    internal static LevelVitality levelVitality = new();
    internal static LevelMetabolism levelMetabolism = new();
    internal static LevelCooking levelCooking = new();
    internal static LevelPanning levelPanning = new();
    internal static LevelLeatherArmor levelLeatherArmor = new();
    internal static LevelChainArmor levelChainArmor = new();
    internal static LevelBrigandineArmor levelBrigandineArmor = new();
    internal static LevelPlateArmor levelPlateArmor = new();
    internal static LevelScaleArmor levelScaleArmor = new();
    internal static LevelSmithing levelSmithing = new();

    internal static OverwriteBlockBreak overwriteBlockBreak = new();
    internal static OverwriteDamageInteraction overwriteDamageInteraction = new();

    /// <summary>
    /// Patch all native codes with harmony, will not patch disabled levels.
    /// Also initialize the level if the api is from Serverside.
    /// 
    /// Consider calling this function only after configurations fully loaded
    /// to prevent patching disabled levels
    /// </summary>
    /// <param name="api"></param>
    public static void PatchAll(ICoreAPI api)
    {
        overwriteBlockBreak.Patch();
        overwriteDamageInteraction.Patch();

        // Smithing should be loaded before any armor or tool level that
        // listen to attributes refresh
        if (Configuration.enableLevelSmithing)
        {
            if (api.Side == EnumAppSide.Server) levelSmithing.Init();
            else if (api.Side == EnumAppSide.Client) levelSmithing.InitClient();
            levelSmithing.Patch();
        }
        if (Configuration.enableLevelHunter)
        {
            if (api.Side == EnumAppSide.Server) levelHunter.Init();
            else if (api.Side == EnumAppSide.Client) levelHunter.InitClient();
            levelHunter.Patch();
        }
        if (Configuration.enableLevelBow)
        {
            if (api.Side == EnumAppSide.Server) levelBow.Init();
            else if (api.Side == EnumAppSide.Client) levelBow.InitClient();
            levelBow.Patch();
        }
        if (Configuration.enableLevelSlingshot)
        {
            if (api.Side == EnumAppSide.Server) levelSlingshot.Init();
            else if (api.Side == EnumAppSide.Client) levelSlingshot.InitClient();
            levelSlingshot.Patch();
        }
        if (Configuration.enableLevelKnife)
        {
            if (api.Side == EnumAppSide.Server) levelKnife.Init();
            else if (api.Side == EnumAppSide.Client) levelKnife.InitClient();
            levelKnife.Patch();
        }
        if (Configuration.enableLevelAxe)
        {
            if (api.Side == EnumAppSide.Server) levelAxe.Init();
            else if (api.Side == EnumAppSide.Client) levelAxe.InitClient();
            levelAxe.Patch();
        }
        if (Configuration.enableLevelPickaxe)
        {
            if (api.Side == EnumAppSide.Server) levelPickaxe.Init();
            else if (api.Side == EnumAppSide.Client) levelPickaxe.InitClient();
            levelPickaxe.Patch();
        }
        if (Configuration.enableLevelShovel)
        {
            if (api.Side == EnumAppSide.Server) levelShovel.Init();
            else if (api.Side == EnumAppSide.Client) levelShovel.InitClient();
            levelShovel.Patch();
        }
        if (Configuration.enableLevelSpear)
        {
            if (api.Side == EnumAppSide.Server) levelSpear.Init();
            else if (api.Side == EnumAppSide.Client) levelSpear.InitClient();
            levelSpear.Patch();
        }
        if (Configuration.enableLevelHammer)
        {
            if (api.Side == EnumAppSide.Server) levelHammer.Init();
            else if (api.Side == EnumAppSide.Client) levelHammer.InitClient();
            levelHammer.Patch();
        }
        if (Configuration.enableLevelSword)
        {
            if (api.Side == EnumAppSide.Server) levelSword.Init();
            else if (api.Side == EnumAppSide.Client) levelSword.InitClient();
            levelSword.Patch();
        }
        if (Configuration.enableLevelShield)
        {
            if (api.Side == EnumAppSide.Server) levelShield.Init();
            else if (api.Side == EnumAppSide.Client) levelShield.InitClient();
            levelShield.Patch();
        }
        if (Configuration.enableLevelHand)
        {
            if (api.Side == EnumAppSide.Server) levelHand.Init();
            else if (api.Side == EnumAppSide.Client) levelHand.InitClient();
            levelHand.Patch();
        }
        if (Configuration.enableLevelFarming)
        {
            if (api.Side == EnumAppSide.Server) levelFarming.Init();
            else if (api.Side == EnumAppSide.Client) levelFarming.InitClient();
            levelFarming.Patch();
        }
        if (Configuration.enableLevelVitality)
        {
            if (api.Side == EnumAppSide.Server) levelVitality.Init();
            else if (api.Side == EnumAppSide.Client) levelVitality.InitClient();
            levelVitality.Patch();
        }
        if (Configuration.enableLevelMetabolism)
        {
            if (api.Side == EnumAppSide.Server) levelMetabolism.Init();
            else if (api.Side == EnumAppSide.Client) levelMetabolism.InitClient();
            levelMetabolism.Patch();
        }
        if (Configuration.enableLevelCooking)
        {
            if (api.Side == EnumAppSide.Server) levelCooking.Init();
            else if (api.Side == EnumAppSide.Client) levelCooking.InitClient();
            levelCooking.Patch();
        }
        if (Configuration.enableLevelPanning)
        {
            if (api.Side == EnumAppSide.Server) levelPanning.Init();
            else if (api.Side == EnumAppSide.Client) levelPanning.InitClient();
            levelPanning.Patch();
        }
        if (Configuration.enableLevelLeatherArmor)
        {
            if (api.Side == EnumAppSide.Server) levelLeatherArmor.Init();
            else if (api.Side == EnumAppSide.Client) levelLeatherArmor.InitClient();
            levelLeatherArmor.Patch();
        }
        if (Configuration.enableLevelChainArmor)
        {
            if (api.Side == EnumAppSide.Server) levelChainArmor.Init();
            else if (api.Side == EnumAppSide.Client) levelChainArmor.InitClient();
            levelChainArmor.Patch();
        }
        if (Configuration.enableLevelBrigandineArmor)
        {
            if (api.Side == EnumAppSide.Server) levelBrigandineArmor.Init();
            else if (api.Side == EnumAppSide.Client) levelBrigandineArmor.InitClient();
            levelBrigandineArmor.Patch();
        }
        if (Configuration.enableLevelPlateArmor)
        {
            if (api.Side == EnumAppSide.Server) levelPlateArmor.Init();
            else if (api.Side == EnumAppSide.Client) levelPlateArmor.InitClient();
            levelPlateArmor.Patch();
        }
        if (Configuration.enableLevelScaleArmor)
        {
            if (api.Side == EnumAppSide.Server) levelScaleArmor.Init();
            else if (api.Side == EnumAppSide.Client) levelScaleArmor.InitClient();
            levelScaleArmor.Patch();
        }
    }

    /// <summary>
    /// Unpatch all patched native codes
    /// </summary>
    public static void UnpatchAll()
    {
        overwriteBlockBreak.Unpatch();
        overwriteDamageInteraction.Unpatch();
        levelHunter.Unpatch();
        levelHunter.Dispose();
        levelBow.Unpatch();
        levelBow.Dispose();
        levelSlingshot.Unpatch();
        levelSlingshot.Dispose();
        levelKnife.Unpatch();
        levelKnife.Dispose();
        levelAxe.Unpatch();
        levelAxe.Dispose();
        levelPickaxe.Unpatch();
        levelPickaxe.Dispose();
        levelShovel.Unpatch();
        levelShovel.Dispose();
        levelSpear.Unpatch();
        levelSpear.Dispose();
        levelHammer.Unpatch();
        levelHammer.Dispose();
        levelSword.Unpatch();
        levelSword.Dispose();
        levelShield.Unpatch();
        levelShield.Dispose();
        levelHand.Unpatch();
        levelHand.Dispose();
        levelFarming.Unpatch();
        levelFarming.Dispose();
        levelVitality.Unpatch();
        levelVitality.Dispose();
        levelMetabolism.Unpatch();
        levelMetabolism.Dispose();
        levelCooking.Unpatch();
        levelCooking.Dispose();
        levelPanning.Unpatch();
        levelPanning.Dispose();
        levelLeatherArmor.Unpatch();
        levelLeatherArmor.Dispose();
        levelChainArmor.Unpatch();
        levelChainArmor.Dispose();
        levelBrigandineArmor.Unpatch();
        levelBrigandineArmor.Dispose();
        levelPlateArmor.Unpatch();
        levelPlateArmor.Dispose();
        levelScaleArmor.Unpatch();
        levelScaleArmor.Dispose();
        levelSmithing.Unpatch();
        levelSmithing.Dispose();
    }

    /// <summary>
    /// Generate the base attributes for armors
    /// </summary>
    /// <param name="item"></param>
    public static void GenerateBaseArmorStatus(ItemStack item)
    {
        if (item.Item is not ItemWearable) return;

        if (item.Attributes.GetBool("BaseGenerated")) return;
        item.Attributes.SetBool("BaseGenerated", true);

        if (item.Collectible?.Attributes?["protectionModifiers"] != null && item.Collectible.Attributes["protectionModifiers"].Token != null)
        {
            // item.Attributes.SetString("BaseProtectionModifiers", item.Collectible.Attributes["protectionModifiers"].Token.ToString(Formatting.None));

            JsonObject protectionModifiers = item.Collectible.Attributes["protectionModifiers"];
            if (protectionModifiers != null && protectionModifiers.Exists && protectionModifiers.Token is JObject protectionModifiersObj)
            {
                float passiveProjectile = protectionModifiersObj["relativeProtection"]?.Value<float>() ?? 0f;
                if (passiveProjectile != 0)
                    item.Attributes.SetString("BaseRelativeProtection", passiveProjectile.ToString());

                float flatDamageReduction = protectionModifiersObj["flatDamageReduction"]?.Value<float>() ?? 0f;
                if (flatDamageReduction != 0)
                    item.Attributes.SetString("BaseFlatDamageReduction", flatDamageReduction.ToString());

                float protectionTier = protectionModifiersObj["protectionTier"]?.Value<float>() ?? 0f;
                if (protectionTier != 0)
                    item.Attributes.SetString("BaseProtectionTier", protectionTier.ToString());

                float[] perTierRelativeProtectionLoss = protectionModifiersObj["perTierRelativeProtectionLoss"]?.ToObject<float[]>() ?? [];
                if (perTierRelativeProtectionLoss.Length != 0)
                    item.Attributes.SetString("BasePerTierRelativeProtectionLoss", $"[{string.Join(",", perTierRelativeProtectionLoss)}]");

                float[] perTierFlatDamageReductionLoss = protectionModifiersObj["perTierFlatDamageReductionLoss"]?.ToObject<float[]>() ?? [];
                if (perTierFlatDamageReductionLoss.Length != 0)
                    item.Attributes.SetString("BasePerTierFlatDamageReductionLoss", $"[{string.Join(",", perTierFlatDamageReductionLoss)}]");
            }
        }

        if (item.Collectible?.Attributes?["statModifiers"] != null && item.Collectible.Attributes["statModifiers"].Token != null)
        {
            // item.Attributes.SetString("BaseStatsModifiers", item.Collectible.Attributes["statModifiers"].Token.ToString(Formatting.None));

            JsonObject statModifiers = item.Collectible.Attributes["statModifiers"];
            if (statModifiers != null && statModifiers.Exists && statModifiers.Token is JObject statModifiersObj)
            {
                float rangedWeaponsAcc = statModifiersObj["rangedWeaponsAcc"]?.Value<float>() ?? 0f;
                if (rangedWeaponsAcc != 0)
                    item.Attributes.SetString("BaseRangedWeaponsAccuracy", rangedWeaponsAcc.ToString());

                float rangedWeaponsSpeed = statModifiersObj["rangedWeaponsSpeed"]?.Value<float>() ?? 0f;
                if (rangedWeaponsSpeed != 0)
                    item.Attributes.SetString("BaseRangedWeaponsSpeed", rangedWeaponsSpeed.ToString());

                float walkSpeed = statModifiersObj["walkSpeed"]?.Value<float>() ?? 0f;
                if (walkSpeed != 0)
                    item.Attributes.SetString("BaseWalkSpeed", walkSpeed.ToString());

                float healingeffectivness = statModifiersObj["healingeffectivness"]?.Value<float>() ?? 0f;
                if (healingeffectivness != 0)
                    item.Attributes.SetString("BaseHealingEffectivness", healingeffectivness.ToString());

                float hungerrate = statModifiersObj["hungerrate"]?.Value<float>() ?? 0f;
                if (hungerrate != 0)
                    item.Attributes.SetString("BaseHungerRate", hungerrate.ToString());
            }
        }
    }

    /// <summary>
    /// Generate the base attributes for shield
    /// </summary>
    /// <param name="item"></param>
    public static void GenerateBaseShieldStatus(ItemStack item)
    {
        JsonObject attr = item.ItemAttributes?["shield"];
        if (attr == null || !attr.Exists) return;

        if (item.Attributes.GetBool("BaseGenerated")) return;
        item.Attributes.SetBool("BaseGenerated", true);

        // item.Attributes.SetString("BaseProtectionModifiers", attr.Token.ToString(Formatting.None));

        JsonObject protection = attr["protectionChance"];
        if (protection != null && protection.Exists && protection.Token is JObject protectionObj)
        {
            float passiveProjectile = protectionObj["passive-projectile"]?.Value<float>() ?? 0f;
            if (passiveProjectile != 0)
                item.Attributes.SetString("BasePassiveProjectile", passiveProjectile.ToString());

            float activeProjectile = protectionObj["active-projectile"]?.Value<float>() ?? 0f;
            if (activeProjectile != 0)
                item.Attributes.SetString("BaseActiveProjectile", activeProjectile.ToString());

            float passive = protectionObj["passive"]?.Value<float>() ?? 0f;
            if (passive != 0)
                item.Attributes.SetString("BasePassive", passive.ToString());

            float active = protectionObj["active"]?.Value<float>() ?? 0f;
            if (active != 0)
                item.Attributes.SetString("BaseActive", active.ToString());
        }

        JsonObject projectileDamageAbsorption = attr["projectileDamageAbsorption"];
        if (projectileDamageAbsorption?.Token is JValue projectileDamageAbsorptionObj)
        {
            item.Attributes.SetString("BaseProjectileDamageAbsorption", projectileDamageAbsorptionObj.Value<float>().ToString());
        }

        JsonObject damageAbsorption = attr["damageAbsorption"];
        if (damageAbsorption?.Token is JValue damageAbsorptionObj)
        {
            item.Attributes.SetString("BaseDamageAbsorption", damageAbsorptionObj.Value<float>().ToString());
        }
    }

    /// <summary>
    /// Increases armor currently status, 1.0 = same, 1.5 = 50% increase.
    /// Compatible with multi calls
    /// </summary>
    /// <param name="armorSlot"></param>
    /// <param name="statsIncrease"></param>
    public static void RefreshArmorAttributes(ItemSlot armorSlot, float statsIncrease)
    {
        if (armorSlot.Itemstack?.Item is not ItemWearable itemWearable) return;
        JsonObject attr = armorSlot.Itemstack.Item?.Attributes;
        if (attr == null || !attr.Exists) return;
        JObject armorObj = (JObject)attr.Token;

        JsonObject protectionModifiers = attr["protectionModifiers"];
        if (protectionModifiers != null && protectionModifiers.Exists && protectionModifiers.Token is JObject protectionModifiersObj)
        {
            JObject protectionObj = (JObject)protectionModifiersObj.DeepClone();

            if (armorSlot.Itemstack.Attributes.GetString("BaseRelativeProtection") != null)
            {
                float currentMeasured = protectionObj["relativeProtection"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRelativeProtection")),
                    currentMeasured
                );

                float result = (float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRelativeProtection")) + difference) * statsIncrease;
                protectionObj["relativeProtection"] = result;
                itemWearable.ProtectionModifiers.RelativeProtection = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseFlatDamageReduction") != null)
            {
                float currentMeasured = protectionObj["flatDamageReduction"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseFlatDamageReduction")),
                    currentMeasured
                );

                float result = (float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseFlatDamageReduction")) + difference) * statsIncrease;
                protectionObj["flatDamageReduction"] = result;
                itemWearable.ProtectionModifiers.FlatDamageReduction = result;
            }

            armorObj["protectionModifiers"] = protectionObj;
        }

        JsonObject statModifiers = attr["statModifiers"];
        if (statModifiers != null && statModifiers.Exists && statModifiers.Token is JObject statModifiersObj)
        {
            JObject statsObj = (JObject)statModifiersObj.DeepClone();

            if (armorSlot.Itemstack.Attributes.GetString("BaseHealingEffectivness") != null)
            {
                float currentMeasured = statsObj["healingeffectivness"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseHealingEffectivness")), currentMeasured);
                float baseValue = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseHealingEffectivness")) + difference;
                float positiveValue = Math.Abs(baseValue);

                float result = baseValue + (positiveValue * Math.Min(statsIncrease - 1, 0));
                statsObj["healingeffectivness"] = result;
                itemWearable.StatModifers.healingeffectivness = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseHungerRate") != null)
            {
                float currentMeasured = statsObj["hungerrate"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseHungerRate")), currentMeasured);
                float baseValue = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseHungerRate")) + difference;
                float positiveValue = Math.Abs(baseValue);

                float result = baseValue + (positiveValue * Math.Min(statsIncrease - 1, 0));
                statsObj["hungerrate"] = result;
                itemWearable.StatModifers.hungerrate = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsAccuracy") != null)
            {
                float currentMeasured = statsObj["rangedWeaponsAcc"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsAccuracy")), currentMeasured);
                float baseValue = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsAccuracy")) + difference;
                float positiveValue = Math.Abs(baseValue);

                float result = baseValue + (positiveValue * Math.Min(statsIncrease - 1, 0));
                statsObj["rangedWeaponsAcc"] = result;
                itemWearable.StatModifers.rangedWeaponsAcc = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsSpeed") != null)
            {
                float currentMeasured = statsObj["rangedWeaponsSpeed"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsSpeed")), currentMeasured);
                float baseValue = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsSpeed")) + difference;
                float positiveValue = Math.Abs(baseValue);

                float result = baseValue + (positiveValue * Math.Min(statsIncrease - 1, 0));
                statsObj["rangedWeaponsSpeed"] = result;
                itemWearable.StatModifers.rangedWeaponsSpeed = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseWalkSpeed") != null)
            {
                float currentMeasured = statsObj["walkSpeed"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseWalkSpeed")), currentMeasured);
                float baseValue = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseWalkSpeed")) + difference;
                float positiveValue = Math.Abs(baseValue);

                float result = baseValue + (positiveValue * Math.Min(statsIncrease - 1, 0));
                statsObj["walkSpeed"] = result;
                itemWearable.StatModifers.walkSpeed = result;
            }

            armorObj["statModifiers"] = statsObj;
        }
    }

    /// <summary>
    /// Reset armor status increase to default, should be called after armor calculations.
    /// Refer to wiki to know why
    /// </summary>
    /// <param name="armorSlot"></param>
    public static void ResetArmorAttributes(ItemSlot armorSlot)
    {
        if (armorSlot.Itemstack?.Item is not ItemWearable itemWearable) return;
        JsonObject attr = armorSlot.Itemstack.Item?.Attributes;
        if (attr == null || !attr.Exists) return;
        JObject armorObj = (JObject)attr.Token;

        Shared.Instance.GenerateBaseArmorStatus(armorSlot.Itemstack);

        JsonObject protectionModifiers = attr["protectionModifiers"];
        if (protectionModifiers != null && protectionModifiers.Exists && protectionModifiers.Token is JObject protectionModifiersObj)
        {
            JObject protectionObj = (JObject)protectionModifiersObj.DeepClone();

            if (armorSlot.Itemstack.Attributes.GetString("BaseRelativeProtection") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRelativeProtection"));
                protectionObj["relativeProtection"] = result;
                itemWearable.ProtectionModifiers.RelativeProtection = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseFlatDamageReduction") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseFlatDamageReduction"));
                protectionObj["flatDamageReduction"] = result;
                itemWearable.ProtectionModifiers.FlatDamageReduction = result;
            }

            armorObj["protectionModifiers"] = protectionObj;
        }

        JsonObject statModifiers = attr["statModifiers"];
        if (statModifiers != null && statModifiers.Exists && statModifiers.Token is JObject statModifiersObj)
        {
            JObject statsObj = (JObject)statModifiersObj.DeepClone();

            if (armorSlot.Itemstack.Attributes.GetString("BaseHealingEffectivness") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseHealingEffectivness"));
                statsObj["healingeffectivness"] = result;
                itemWearable.StatModifers.healingeffectivness = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseHungerRate") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseHungerRate"));
                statsObj["hungerrate"] = result;
                itemWearable.StatModifers.hungerrate = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsAccuracy") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsAccuracy"));
                statsObj["rangedWeaponsAcc"] = result;
                itemWearable.StatModifers.rangedWeaponsAcc = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsSpeed") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseRangedWeaponsSpeed"));
                statsObj["rangedWeaponsSpeed"] = result;
                itemWearable.StatModifers.rangedWeaponsSpeed = result;
            }

            if (armorSlot.Itemstack.Attributes.GetString("BaseWalkSpeed") != null)
            {
                float result = float.Parse(armorSlot.Itemstack.Attributes.GetString("BaseWalkSpeed"));
                statsObj["walkSpeed"] = result;
                itemWearable.StatModifers.walkSpeed = result;
            }

            armorObj["statModifiers"] = statsObj;
        }
    }

}