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

    public static void PatchAll(ICoreAPI api)
    {
        overwriteBlockBreak.Patch();
        overwriteDamageInteraction.Patch();

        if (Configuration.enableLevelHunter)
        {
            if (api.Side == EnumAppSide.Server) levelHunter.Init();
            levelHunter.Patch();
        }
        if (Configuration.enableLevelBow)
        {
            if (api.Side == EnumAppSide.Server) levelBow.Init();
            levelBow.Patch();
        }
        if (Configuration.enableLevelSlingshot)
        {
            if (api.Side == EnumAppSide.Server) levelSlingshot.Init();
            levelSlingshot.Patch();
        }
        if (Configuration.enableLevelKnife)
        {
            if (api.Side == EnumAppSide.Server) levelKnife.Init();
            levelKnife.Patch();
        }
        if (Configuration.enableLevelAxe)
        {
            if (api.Side == EnumAppSide.Server) levelAxe.Init();
            levelAxe.Patch();
        }
        if (Configuration.enableLevelPickaxe)
        {
            if (api.Side == EnumAppSide.Server) levelPickaxe.Init();
            levelPickaxe.Patch();
        }
        if (Configuration.enableLevelShovel)
        {
            if (api.Side == EnumAppSide.Server) levelShovel.Init();
            levelShovel.Patch();
        }
        if (Configuration.enableLevelSpear)
        {
            if (api.Side == EnumAppSide.Server) levelSpear.Init();
            levelSpear.Patch();
        }
        if (Configuration.enableLevelHammer)
        {
            if (api.Side == EnumAppSide.Server) levelHammer.Init();
            levelHammer.Patch();
        }
        if (Configuration.enableLevelSword)
        {
            if (api.Side == EnumAppSide.Server) levelSword.Init();
            levelSword.Patch();
        }
        if (Configuration.enableLevelShield)
        {
            if (api.Side == EnumAppSide.Server) levelShield.Init();
            levelShield.Patch();
        }
        if (Configuration.enableLevelHand)
        {
            if (api.Side == EnumAppSide.Server) levelHand.Init();
            levelHand.Patch();
        }
        if (Configuration.enableLevelFarming)
        {
            if (api.Side == EnumAppSide.Server) levelFarming.Init();
            levelFarming.Patch();
        }
        if (Configuration.enableLevelVitality)
        {
            if (api.Side == EnumAppSide.Server) levelVitality.Init();
            levelVitality.Patch();
        }
        if (Configuration.enableLevelMetabolism)
        {
            if (api.Side == EnumAppSide.Server) levelMetabolism.Init();
            levelMetabolism.Patch();
        }
        if (Configuration.enableLevelCooking)
        {
            if (api.Side == EnumAppSide.Server) levelCooking.Init();
            levelCooking.Patch();
        }
        if (Configuration.enableLevelPanning)
        {
            if (api.Side == EnumAppSide.Server) levelPanning.Init();
            levelPanning.Patch();
        }
        if (Configuration.enableLevelLeatherArmor)
        {
            if (api.Side == EnumAppSide.Server) levelLeatherArmor.Init();
            levelLeatherArmor.Patch();
        }
        if (Configuration.enableLevelChainArmor)
        {
            if (api.Side == EnumAppSide.Server) levelChainArmor.Init();
            levelChainArmor.Patch();
        }
        if (Configuration.enableLevelBrigandineArmor)
        {
            if (api.Side == EnumAppSide.Server) levelBrigandineArmor.Init();
            levelBrigandineArmor.Patch();
        }
        if (Configuration.enableLevelPlateArmor)
        {
            if (api.Side == EnumAppSide.Server) levelPlateArmor.Init();
            levelPlateArmor.Patch();
        }
        if (Configuration.enableLevelScaleArmor)
        {
            if (api.Side == EnumAppSide.Server) levelScaleArmor.Init();
            levelScaleArmor.Patch();
        }
        if (Configuration.enableLevelSmithing)
        {
            if (api.Side == EnumAppSide.Server) levelSmithing.Init();
            levelSmithing.Patch();
        }
    }

    public static void UnpatchAll()
    {
        overwriteBlockBreak.Unpatch();
        overwriteDamageInteraction.Unpatch();
        levelHunter.Unpatch();
        levelBow.Unpatch();
        levelSlingshot.Unpatch();
        levelKnife.Unpatch();
        levelAxe.Unpatch();
        levelPickaxe.Unpatch();
        levelShovel.Unpatch();
        levelSpear.Unpatch();
        levelHammer.Unpatch();
        levelSword.Unpatch();
        levelShield.Unpatch();
        levelHand.Unpatch();
        levelFarming.Unpatch();
        levelVitality.Unpatch();
        levelMetabolism.Unpatch();
        levelCooking.Unpatch();
        levelPanning.Unpatch();
        levelLeatherArmor.Unpatch();
        levelChainArmor.Unpatch();
        levelBrigandineArmor.Unpatch();
        levelPlateArmor.Unpatch();
        levelScaleArmor.Unpatch();
        levelSmithing.Unpatch();
    }

    /// Generate the base attributes for armors
    public static void GenerateBaseArmorStatus(ItemStack item)
    {
        if (item.Item is not ItemWearable itemWearable) return;

        if (item.Attributes.GetBool("BaseGenerated")) return;
        item.Attributes.SetBool("BaseGenerated", true);

        if (item.Attributes.TryGetFloat("BaseFlatDamageReduction") == null)
            if (itemWearable.ProtectionModifiers != null)
                item.Attributes.SetFloat("BaseFlatDamageReduction", itemWearable.ProtectionModifiers.FlatDamageReduction);

        if (item.Attributes.TryGetFloat("BaseRelativeProtection") == null)
            if (itemWearable.ProtectionModifiers != null)
                item.Attributes.SetFloat("BaseRelativeProtection", itemWearable.ProtectionModifiers.RelativeProtection);

        if (item.Attributes.TryGetFloat("BaseHealingEffectivness") == null)
            if (itemWearable.StatModifers != null)
                item.Attributes.SetFloat("BaseHealingEffectivness", itemWearable.StatModifers.healingeffectivness);

        if (item.Attributes.TryGetFloat("BaseHungerRate") == null)
            if (itemWearable.StatModifers != null)
                item.Attributes.SetFloat("BaseHungerRate", itemWearable.StatModifers.hungerrate);

        if (item.Attributes.TryGetFloat("BaseRangedWeaponsAccuracy") == null)
            if (itemWearable.StatModifers != null)
                item.Attributes.SetFloat("BaseRangedWeaponsAccuracy", itemWearable.StatModifers.rangedWeaponsAcc);

        if (item.Attributes.TryGetFloat("BaseRangedWeaponsSpeed") == null)
            if (itemWearable.StatModifers != null)
                item.Attributes.SetFloat("BaseRangedWeaponsSpeed", itemWearable.StatModifers.rangedWeaponsSpeed);

        if (item.Attributes.TryGetFloat("BaseWalkSpeed") == null)
            if (itemWearable.StatModifers != null)
                item.Attributes.SetFloat("BaseWalkSpeed", itemWearable.StatModifers.walkSpeed);
    }

    // Generate the base attributes for shield
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
                item.Attributes.SetFloat("BasePassiveProjectile", passiveProjectile);

            float activeProjectile = protectionObj["active-projectile"]?.Value<float>() ?? 0f;
            if (activeProjectile != 0)
                item.Attributes.SetFloat("BaseActiveProjectile", activeProjectile);

            float passive = protectionObj["passive"]?.Value<float>() ?? 0f;
            if (passive != 0)
                item.Attributes.SetFloat("BasePassive", passive);

            float active = protectionObj["active"]?.Value<float>() ?? 0f;
            if (active != 0)
                item.Attributes.SetFloat("BaseActive", active);
        }

        JsonObject projectileDamageAbsorption = attr["projectileDamageAbsorption"];
        if (projectileDamageAbsorption?.Token is JValue projectileDamageAbsorptionObj)
        {
            item.Attributes.SetFloat("BaseProjectileDamageAbsorption", projectileDamageAbsorptionObj.Value<float>());
        }

        JsonObject damageAbsorption = attr["damageAbsorption"];
        if (damageAbsorption?.Token is JValue damageAbsorptionObj)
        {
            item.Attributes.SetFloat("BaseDamageAbsorption", damageAbsorptionObj.Value<float>());
        }
    }
}