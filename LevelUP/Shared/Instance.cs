using System;
using System.Collections.Generic;
using LevelUP.Server;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace LevelUP.Shared;

class Instance
{
    internal static ICoreAPI api = null;
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
    internal static LevelLamellarArmor levelLamellarArmor = new();
    internal static LevelPlateArmor levelPlateArmor = new();
    internal static LevelScaleArmor levelScaleArmor = new();
    internal static LevelSmithing levelSmithing = new();

    internal static OverwriteBlockBreak overwriteBlockBreak = new();
    internal static OverwriteDamageInteraction overwriteDamageInteraction = new();
    internal static OverwriteAimingAccuracy overwriteAimingAccuracy = new();

    /// <summary>
    /// Patch all native codes with harmony, will not patch disabled levels.
    /// Also initialize the level if the api is from Serverside.
    /// 
    /// Consider calling this function only after configurations fully loaded
    /// to prevent patching disabled levels
    /// </summary>
    /// <param name="api"></param>
    public static void PatchAll()
    {
        overwriteBlockBreak.Patch();
        overwriteDamageInteraction.Patch();
        if (Configuration.enableLevelBow || Configuration.enableLevelSpear)
            overwriteAimingAccuracy.Patch();

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
            if (api.Side == EnumAppSide.Server) { levelAxe.Init(); levelAxe.Patch(); }
            else if (api.Side == EnumAppSide.Client) levelAxe.InitClient();
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
        // Smithing should be loaded before any armor or tool level that
        // listen to attributes refresh
        if (Configuration.enableLevelSmithing)
        {
            if (api.Side == EnumAppSide.Server) levelSmithing.Init();
            else if (api.Side == EnumAppSide.Client) levelSmithing.InitClient();
            levelSmithing.Patch();
        }
        if (Configuration.enableLevelShield)
        {
            if (api.Side == EnumAppSide.Server) levelShield.Init();
            else if (api.Side == EnumAppSide.Client) levelShield.InitClient();
            levelShield.Patch();
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
        if (Configuration.enableLevelLamellarArmor)
        {
            if (api.Side == EnumAppSide.Server) levelLamellarArmor.Init();
            else if (api.Side == EnumAppSide.Client) levelLamellarArmor.InitClient();
            levelLamellarArmor.Patch();
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
        overwriteAimingAccuracy.Unpatch();
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
        if (item.Item?.GetCollectibleBehavior<CollectibleBehaviorWearable>(true) == null) return;

        if (item.Attributes.GetBool("BaseGenerated")) return;
        item.Attributes.SetBool("BaseGenerated", true);

        if (item.Collectible?.Attributes?["protectionModifiers"] != null && item.Collectible.Attributes["protectionModifiers"].Token != null)
        {
            JsonObject protectionModifiers = item.Collectible.Attributes["protectionModifiers"];
            if (protectionModifiers != null && protectionModifiers.Exists && protectionModifiers.Token is JObject protectionModifiersObj)
            {
                float passiveProjectile = protectionModifiersObj["relativeProtection"]?.Value<float>() ?? 0f;
                if (passiveProjectile != 0)
                    item.Attributes.SetString("BaseRelativeProtection", passiveProjectile.ToStringCulturized());

                float flatDamageReduction = protectionModifiersObj["flatDamageReduction"]?.Value<float>() ?? 0f;
                if (flatDamageReduction != 0)
                    item.Attributes.SetString("BaseFlatDamageReduction", flatDamageReduction.ToStringCulturized());

                float protectionTier = protectionModifiersObj["protectionTier"]?.Value<float>() ?? 0f;
                if (protectionTier != 0)
                    item.Attributes.SetString("BaseProtectionTier", protectionTier.ToStringCulturized());

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
            JsonObject statModifiers = item.Collectible.Attributes["statModifiers"];
            if (statModifiers != null && statModifiers.Exists && statModifiers.Token is JObject statModifiersObj)
            {
                float rangedWeaponsAcc = statModifiersObj["rangedWeaponsAcc"]?.Value<float>() ?? 0f;
                if (rangedWeaponsAcc != 0)
                    item.Attributes.SetString("BaseRangedWeaponsAccuracy", rangedWeaponsAcc.ToStringCulturized());

                float rangedWeaponsSpeed = statModifiersObj["rangedWeaponsSpeed"]?.Value<float>() ?? 0f;
                if (rangedWeaponsSpeed != 0)
                    item.Attributes.SetString("BaseRangedWeaponsSpeed", rangedWeaponsSpeed.ToStringCulturized());

                float walkSpeed = statModifiersObj["walkSpeed"]?.Value<float>() ?? 0f;
                if (walkSpeed != 0)
                    item.Attributes.SetString("BaseWalkSpeed", walkSpeed.ToStringCulturized());

                float healingeffectivness = statModifiersObj["healingeffectivness"]?.Value<float>() ?? 0f;
                if (healingeffectivness != 0)
                    item.Attributes.SetString("BaseHealingEffectivness", healingeffectivness.ToStringCulturized());

                float hungerrate = statModifiersObj["hungerrate"]?.Value<float>() ?? 0f;
                if (hungerrate != 0)
                    item.Attributes.SetString("BaseHungerRate", hungerrate.ToStringCulturized());
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

        JsonObject protection = attr["protectionChance"];
        if (protection != null && protection.Exists && protection.Token is JObject protectionObj)
        {
            float passiveProjectile = protectionObj["passive-projectile"]?.Value<float>() ?? 0f;
            if (passiveProjectile != 0)
                item.Attributes.SetString("BasePassiveProjectile", passiveProjectile.ToStringCulturized());

            float activeProjectile = protectionObj["active-projectile"]?.Value<float>() ?? 0f;
            if (activeProjectile != 0)
                item.Attributes.SetString("BaseActiveProjectile", activeProjectile.ToStringCulturized());

            float passive = protectionObj["passive"]?.Value<float>() ?? 0f;
            if (passive != 0)
                item.Attributes.SetString("BasePassive", passive.ToStringCulturized());

            float active = protectionObj["active"]?.Value<float>() ?? 0f;
            if (active != 0)
                item.Attributes.SetString("BaseActive", active.ToStringCulturized());
        }

        JsonObject projectileDamageAbsorption = attr["projectileDamageAbsorption"];
        if (projectileDamageAbsorption?.Token is JValue projectileDamageAbsorptionObj)
        {
            item.Attributes.SetString("BaseProjectileDamageAbsorption", projectileDamageAbsorptionObj.Value<float>().ToStringCulturized());
        }

        JsonObject damageAbsorption = attr["damageAbsorption"];
        if (damageAbsorption?.Token is JValue damageAbsorptionObj)
        {
            item.Attributes.SetString("BaseDamageAbsorption", damageAbsorptionObj.Value<float>().ToStringCulturized());
        }
    }

    /// <summary>
    /// Generate the base attributes for tools
    /// </summary>
    /// <param name="item"></param>
    public static void GenerateBaseToolStatus(IItemStack item)
    {
        if (item.Item?.Tool == null)
        {
            Debug.LogDebug($"{item.Collectible.Code} ignored stats generation because is not a tool!");
            return;
        }

        if (item.Attributes.GetBool("BaseGenerated")) return;
        item.Attributes.SetBool("BaseGenerated", true);

        if (item.Collectible.AttackPower > 0.0f)
        {
            item.Attributes.SetString("BaseAttack", item.Collectible.AttackPower.ToStringCulturized());
        }

        if (item.Collectible.MiningSpeed != null)
        {
            List<EnumBlockMaterial> keys = [.. item.Item.MiningSpeed.Keys];
            foreach (EnumBlockMaterial key in keys)
            {
                item.Attributes.SetString($"BaseMiningSpeed_{key}", item.Collectible.MiningSpeed[key].ToStringCulturized());
            }
        }
    }

    // Cache: itemCode → (statKey → base float value). null entry means stat doesn't exist on this armor type.
    private static readonly Dictionary<string, Dictionary<string, float?>> _baseValueCache = [];

    private static float? GetCachedBaseFloat(ItemSlot slot, string key)
    {
        string code = slot.Itemstack.Collectible.Code.ToString();
        if (!_baseValueCache.TryGetValue(code, out Dictionary<string, float?> stats))
        {
            stats = [];
            _baseValueCache[code] = stats;
        }
        if (stats.TryGetValue(key, out float? cached))
            return cached;

        string str = slot.Itemstack.Attributes.GetString(key);
        float? result = str != null ? UtilsCulture.ParseFloatCulturized(str) : null;
        stats[key] = result;
        return result;
    }

    /// <summary>
    /// Increases armor currently status, 1.0 = same, 1.5 = 50% increase.
    /// Compatible with multi calls
    /// </summary>
    /// <param name="armorSlot"></param>
    /// <param name="statsIncrease"></param>
    public static void RefreshArmorAttributes(
        ItemSlot armorSlot,
        float relativeProtection = 1.0f,
        float flatDamageReduction = 1.0f,
        float healingEffectivness = 1.0f,
        float hungerRate = 1.0f,
        float rangedWeaponsAccuracy = 1.0f,
        float rangedWeaponsSpeed = 1.0f,
        float walkSpeed = 1.0f,
        CollectibleBehaviorWearable behavior = null)
    {
        CollectibleBehaviorWearable collectibleBehaviorWearable = behavior ?? armorSlot.Itemstack?.Item?.GetCollectibleBehavior<CollectibleBehaviorWearable>(true);
        if (collectibleBehaviorWearable == null) return;
        JsonObject attr = armorSlot.Itemstack.Item?.Attributes;
        if (attr == null || !attr.Exists) return;

        JsonObject protectionModifiers = attr["protectionModifiers"];
        if (protectionModifiers != null && protectionModifiers.Exists && protectionModifiers.Token is JObject protectionModifiersObj)
        {
            if (GetCachedBaseFloat(armorSlot, "BaseRelativeProtection").HasValue)
            {
                float current = protectionModifiersObj["relativeProtection"].Value<float>();
                float result = (float)Math.Round(current * relativeProtection, 2);
                protectionModifiersObj["relativeProtection"] = result;
                collectibleBehaviorWearable.GetProtectionModifiers(armorSlot).RelativeProtection = result;
            }

            if (GetCachedBaseFloat(armorSlot, "BaseFlatDamageReduction").HasValue)
            {
                float current = protectionModifiersObj["flatDamageReduction"].Value<float>();
                float result = (float)Math.Round(current * flatDamageReduction, 2);
                protectionModifiersObj["flatDamageReduction"] = result;
                collectibleBehaviorWearable.GetProtectionModifiers(armorSlot).FlatDamageReduction = result;
            }
        }

        JsonObject statModifiers = attr["statModifiers"];
        if (statModifiers != null && statModifiers.Exists && statModifiers.Token is JObject statModifiersObj)
        {
            if (GetCachedBaseFloat(armorSlot, "BaseHealingEffectivness").HasValue)
            {
                float current = statModifiersObj["healingeffectivness"].Value<float>();
                float result = (float)Math.Round(current + (Math.Abs(current) * Math.Max(healingEffectivness - 1, 0)), 2);
                statModifiersObj["healingeffectivness"] = result;
                collectibleBehaviorWearable.GetStatModifiers(armorSlot).healingeffectivness = result;
            }

            if (GetCachedBaseFloat(armorSlot, "BaseHungerRate").HasValue)
            {
                float current = statModifiersObj["hungerrate"].Value<float>();
                float result = (float)Math.Round(current - (Math.Abs(current) * Math.Max(hungerRate - 1, 0)), 2);
                statModifiersObj["hungerrate"] = result;
                collectibleBehaviorWearable.GetStatModifiers(armorSlot).hungerrate = result;
            }

            if (GetCachedBaseFloat(armorSlot, "BaseRangedWeaponsAccuracy").HasValue)
            {
                float current = statModifiersObj["rangedWeaponsAcc"].Value<float>();
                float result = (float)Math.Round(current + (Math.Abs(current) * Math.Max(rangedWeaponsAccuracy - 1, 0)), 2);
                statModifiersObj["rangedWeaponsAcc"] = result;
                collectibleBehaviorWearable.GetStatModifiers(armorSlot).rangedWeaponsAcc = result;
            }

            if (GetCachedBaseFloat(armorSlot, "BaseRangedWeaponsSpeed").HasValue)
            {
                float current = statModifiersObj["rangedWeaponsSpeed"].Value<float>();
                float result = (float)Math.Round(current + (Math.Abs(current) * Math.Max(rangedWeaponsSpeed - 1, 0)), 2);
                statModifiersObj["rangedWeaponsSpeed"] = result;
                collectibleBehaviorWearable.GetStatModifiers(armorSlot).rangedWeaponsSpeed = result;
            }

            if (GetCachedBaseFloat(armorSlot, "BaseWalkSpeed").HasValue)
            {
                float current = statModifiersObj["walkSpeed"].Value<float>();
                float result = (float)Math.Round(current + (Math.Abs(current) * Math.Max(walkSpeed - 1, 0)), 2);
                statModifiersObj["walkSpeed"] = result;
                collectibleBehaviorWearable.GetStatModifiers(armorSlot).walkSpeed = result;
            }
        }
    }

    /// <summary>
    /// Increases shield currently status, 1.0 = same, 1.5 = 50% increase.
    /// Compatible with multi calls
    /// </summary>
    /// <param name="shieldSlot"></param>
    /// <param name="statsIncrease"></param>
    public static void RefreshShieldAttributes(
        ItemSlot shieldSlot,
        float passiveProjectile = 1.0f,
        float activeProjectile = 1.0f,
        float passive = 1.0f,
        float active = 1.0f,
        float projectileDamageAbsorption = 1.0f,
        float damageAbsorption = 1.0f)
    {
        JsonObject attr = shieldSlot.Itemstack?.ItemAttributes?["shield"];
        if (attr == null || !attr.Exists) return;
        JObject shieldObj = (JObject)attr.Token;

        JsonObject protection = attr["protectionChance"];
        if (protection != null && protection.Exists && protection.Token is JObject originalProtObj)
        {
            JObject protectionObj = (JObject)originalProtObj.DeepClone();

            if (shieldSlot.Itemstack.Attributes.GetString("BasePassiveProjectile") != null)
            {
                float currentMeasured = protectionObj["passive-projectile"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BasePassiveProjectile")),
                    currentMeasured
                );

                protectionObj["passive-projectile"] =
                    (float)Math.Round((UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BasePassiveProjectile")) + difference) * passiveProjectile, 2);
            }

            if (shieldSlot.Itemstack.Attributes.GetString("BaseActiveProjectile") != null)
            {
                float currentMeasured = protectionObj["active-projectile"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BaseActiveProjectile")),
                    currentMeasured
                );

                protectionObj["active-projectile"] =
                    (float)Math.Round((UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BaseActiveProjectile")) + difference) * activeProjectile, 2);
            }

            if (shieldSlot.Itemstack.Attributes.GetString("BasePassive") != null)
            {
                float currentMeasured = protectionObj["passive"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BasePassive")),
                    currentMeasured
                );

                protectionObj["passive"] =
                    (float)Math.Round((UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BasePassive")) + difference) * passive, 2);
            }

            if (shieldSlot.Itemstack.Attributes.GetString("BaseActive") != null)
            {
                float currentMeasured = protectionObj["active"].Value<float>();
                float difference = Utils.GetDifferenceBetweenTwoFloats(
                    UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BaseActive")),
                    currentMeasured
                );

                protectionObj["active"] =
                    (float)Math.Round((UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BaseActive")) + difference) * active, 2);
            }

            shieldObj["protectionChance"] = protectionObj;
        }

        JsonObject jsonProjectileDamageAbsorption = attr["projectileDamageAbsorption"];
        if (shieldSlot.Itemstack.Attributes.GetString("BaseProjectileDamageAbsorption") != null &&
            jsonProjectileDamageAbsorption?.Token is JValue projectileJVal)
        {
            float currentMeasured = projectileJVal.Value<float>();

            float difference = Utils.GetDifferenceBetweenTwoFloats(
                UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BaseProjectileDamageAbsorption")),
                currentMeasured
            );

            shieldObj["projectileDamageAbsorption"] =
                (float)Math.Round((UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BaseProjectileDamageAbsorption")) + difference) * projectileDamageAbsorption, 2);
        }

        JsonObject jsonDamageAbsorption = attr["damageAbsorption"];
        if (shieldSlot.Itemstack.Attributes.GetString("BaseDamageAbsorption") != null &&
            jsonDamageAbsorption?.Token is JValue damageAbsorptionJVal)
        {
            float currentMeasured = damageAbsorptionJVal.Value<float>();

            float difference = Utils.GetDifferenceBetweenTwoFloats(
                UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BaseDamageAbsorption")),
                currentMeasured
            );

            shieldObj["damageAbsorption"] =
                (float)Math.Round((UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BaseDamageAbsorption")) + difference) * damageAbsorption, 2);
        }
    }

    /// <summary>
    /// Increases tool currently status, 1.0 = same, 1.5 = 50% increase.
    /// </summary>
    /// <param name="item"></param>
    public static void RefreshToolAttributes(
        IItemStack item,
        float baseAttack = 1.0f,
        float miningSpeed = 1.0f)
    {
        if (item.Attributes.GetString("BaseAttack") != null)
        {
            float result = UtilsCulture.ParseFloatCulturized(item.Attributes.GetString("BaseAttack")) * baseAttack;
            item.Collectible.AttackPower = result;
        }

        if (item.Collectible.MiningSpeed != null)
        {
            List<EnumBlockMaterial> keys = [.. item.Collectible.MiningSpeed.Keys];
            foreach (EnumBlockMaterial key in keys)
            {
                if (item.Attributes.GetString($"BaseMiningSpeed_{key}") != null)
                {
                    float result = UtilsCulture.ParseFloatCulturized(item.Attributes.GetString($"BaseMiningSpeed_{key}")) * miningSpeed;
                    item.Collectible.MiningSpeed[key] = (float)Math.Round(result, 2);
                }
            }
        }
    }

    /// <summary>
    /// Reset armor status increase to default, should be called before and after armor refresh
    /// </summary>
    /// <param name="armorSlot"></param>
    /// <param name="behavior">Pre-fetched CollectibleBehaviorWearable to avoid redundant lookup</param>
    public static void ResetArmorAttributes(ItemSlot armorSlot, CollectibleBehaviorWearable behavior = null)
    {
        CollectibleBehaviorWearable armorSlotBehavior = behavior ?? armorSlot.Itemstack?.Item?.GetCollectibleBehavior<CollectibleBehaviorWearable>(true);
        if (armorSlotBehavior == null) return;
        JsonObject attr = armorSlot.Itemstack.Item?.Attributes;
        if (attr == null || !attr.Exists) return;

        GenerateBaseArmorStatus(armorSlot.Itemstack);

        JsonObject protectionModifiers = attr["protectionModifiers"];
        if (protectionModifiers != null && protectionModifiers.Exists && protectionModifiers.Token is JObject protectionModifiersObj)
        {
            float? baseRel = GetCachedBaseFloat(armorSlot, "BaseRelativeProtection");
            if (baseRel.HasValue)
            {
                protectionModifiersObj["relativeProtection"] = baseRel.Value;
                armorSlotBehavior.GetProtectionModifiers(armorSlot).RelativeProtection = baseRel.Value;
            }

            float? baseFlatDmg = GetCachedBaseFloat(armorSlot, "BaseFlatDamageReduction");
            if (baseFlatDmg.HasValue)
            {
                protectionModifiersObj["flatDamageReduction"] = baseFlatDmg.Value;
                armorSlotBehavior.GetProtectionModifiers(armorSlot).FlatDamageReduction = baseFlatDmg.Value;
            }
        }

        JsonObject statModifiers = attr["statModifiers"];
        if (statModifiers != null && statModifiers.Exists && statModifiers.Token is JObject statModifiersObj)
        {
            float? baseHealing = GetCachedBaseFloat(armorSlot, "BaseHealingEffectivness");
            if (baseHealing.HasValue)
            {
                statModifiersObj["healingeffectivness"] = baseHealing.Value;
                armorSlotBehavior.GetStatModifiers(armorSlot).healingeffectivness = baseHealing.Value;
            }

            float? baseHunger = GetCachedBaseFloat(armorSlot, "BaseHungerRate");
            if (baseHunger.HasValue)
            {
                statModifiersObj["hungerrate"] = baseHunger.Value;
                armorSlotBehavior.GetStatModifiers(armorSlot).hungerrate = baseHunger.Value;
            }

            float? baseRangedAcc = GetCachedBaseFloat(armorSlot, "BaseRangedWeaponsAccuracy");
            if (baseRangedAcc.HasValue)
            {
                statModifiersObj["rangedWeaponsAcc"] = baseRangedAcc.Value;
                armorSlotBehavior.GetStatModifiers(armorSlot).rangedWeaponsAcc = baseRangedAcc.Value;
            }

            float? baseRangedSpd = GetCachedBaseFloat(armorSlot, "BaseRangedWeaponsSpeed");
            if (baseRangedSpd.HasValue)
            {
                statModifiersObj["rangedWeaponsSpeed"] = baseRangedSpd.Value;
                armorSlotBehavior.GetStatModifiers(armorSlot).rangedWeaponsSpeed = baseRangedSpd.Value;
            }

            float? baseWalk = GetCachedBaseFloat(armorSlot, "BaseWalkSpeed");
            if (baseWalk.HasValue)
            {
                statModifiersObj["walkSpeed"] = baseWalk.Value;
                armorSlotBehavior.GetStatModifiers(armorSlot).walkSpeed = baseWalk.Value;
            }
        }
    }

    /// <summary>
    /// Reset shield status increase to default, should be called before and after tool refresh
    /// </summary>
    /// <param name="shieldSlot"></param>
    public static void ResetShieldAttributes(ItemSlot shieldSlot)
    {
        JsonObject attr = shieldSlot.Itemstack?.ItemAttributes?["shield"];
        if (attr == null || !attr.Exists) return;
        JObject shieldObj = (JObject)attr.Token;

        GenerateBaseShieldStatus(shieldSlot.Itemstack);

        JsonObject protection = attr["protectionChance"];
        if (protection != null && protection.Exists && protection.Token is JObject originalProtObj)
        {
            JObject protectionObj = (JObject)originalProtObj.DeepClone();

            if (shieldSlot.Itemstack.Attributes.GetString("BasePassiveProjectile") != null)
                protectionObj["passive-projectile"] = UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BasePassiveProjectile"));
            if (shieldSlot.Itemstack.Attributes.GetString("BaseActiveProjectile") != null)
                protectionObj["active-projectile"] = UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BaseActiveProjectile"));
            if (shieldSlot.Itemstack.Attributes.GetString("BasePassive") != null)
                protectionObj["passive"] = UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BasePassive"));
            if (shieldSlot.Itemstack.Attributes.GetString("BaseActive") != null)
                protectionObj["active"] = UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BaseActive"));

            shieldObj["protectionChance"] = protectionObj;
        }

        JsonObject projectileDamageAbsorption = attr["projectileDamageAbsorption"];
        if (projectileDamageAbsorption?.Token is JValue)
        {
            if (shieldSlot.Itemstack.Attributes.GetString("BaseProjectileDamageAbsorption") != null)
                shieldObj["projectileDamageAbsorption"] = UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BaseProjectileDamageAbsorption"));
        }

        JsonObject damageAbsorption = attr["damageAbsorption"];
        if (damageAbsorption?.Token is JValue)
        {
            if (shieldSlot.Itemstack.Attributes.GetString("BaseDamageAbsorption") != null)
                shieldObj["damageAbsorption"] = UtilsCulture.ParseFloatCulturized(shieldSlot.Itemstack.Attributes.GetString("BaseDamageAbsorption"));
        }
    }

    /// <summary>
    /// Reset tool status increase to default, should be called before and after tool refresh
    /// </summary>
    /// <param name="item"></param>
    public static void ResetToolAttributes(IItemStack item)
    {
        GenerateBaseToolStatus(item);

        if (item.Collectible.AttackPower > 0.0f)
        {
            if (item.Attributes.GetString("BaseAttack") != null)
            {
                float result = UtilsCulture.ParseFloatCulturized(item.Attributes.GetString("BaseAttack"));
                item.Collectible.AttackPower = result;
            }
        }

        if (item.Collectible.MiningSpeed != null)
        {
            List<EnumBlockMaterial> keys = [.. item.Item.MiningSpeed.Keys];
            foreach (EnumBlockMaterial key in keys)
            {
                if (item.Attributes.GetString($"BaseMiningSpeed_{key}") != null)
                {
                    float result = UtilsCulture.ParseFloatCulturized(item.Attributes.GetString($"BaseMiningSpeed_{key}"));
                    item.Collectible.MiningSpeed[key] = result;
                }
            }
        }
    }
}