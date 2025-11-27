#pragma warning disable CA1822
using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelKnife
{
    public readonly Harmony patch = new("levelup_knife");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_knife"))
        {
            patch.PatchCategory("levelup_knife");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_knife"))
        {
            patch.UnpatchCategory("levelup_knife");
        }
    }

    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        // Instanciate break block event
        Instance.api.Event.BreakBlock += OnBreakBlock;
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart += HandleDamage;
        Configuration.RegisterNewLevel("Knife");
        Configuration.RegisterNewLevelTypeEXP("Knife", Configuration.KnifeGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Knife", Configuration.KnifeGetExpByLevel);

        Debug.Log("Level Knife initialized");
    }

    public void InitClient()
    {
        Debug.Log("Level Knife initialized");
    }

    private void HandleDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (player.InventoryManager.ActiveTool == EnumTool.Knife)
        {
            damage *= Configuration.KnifeGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife"));
            Experience.IncreaseExperience(player, "Knife", "Hit");
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateKnifeConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Knife", Configuration.knifeMaxLevel);
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // The cause of the death is from a projectile
        if (damageSource.GetCauseEntity() is not EntityPlayer && damageSource.SourceEntity is EntityProjectile) return;
        // Entity kill is not from a player
        if (damageSource.SourceEntity is not EntityPlayer) return;

        // Get player entity
        EntityPlayer playerEntity = damageSource.SourceEntity as EntityPlayer;

        // Get player instance
        IPlayer player = playerEntity.Player;

        // Check if player is using a bow
        if (player.InventoryManager.ActiveTool != EnumTool.Knife) return;

        // Get the exp received
        ulong exp = (ulong)Configuration.entityExpKnife.GetValueOrDefault(entity.Code.ToString());

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Knife");

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, knife exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Knife", exp);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        // If not a knife ignore
        if (player.InventoryManager.ActiveTool != EnumTool.Knife) return;
        // If not a valid block for knife
        switch (breakedBlock.Block.BlockMaterial)
        {
            case EnumBlockMaterial.Plant: break;
            case EnumBlockMaterial.Leaves: break;
            default: return;
        }

        // Get the exp received
        ulong exp = (ulong)Configuration.ExpPerBreakingKnife;

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Knife");

        Debug.LogDebug($"{player.PlayerName} breaked: {breakedBlock.Block.Code}, knife exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Knife", exp);
    }

    [HarmonyPatchCategory("levelup_knife")]
    private class LevelKnifePatch
    {
        // Overwrite Knife Harvesting
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EntityBehaviorHarvestable), "GenerateDrops")]
        internal static void GenerateDropsStart(EntityBehaviorHarvestable __instance, IPlayer byPlayer)
        {
            if (!Configuration.enableLevelKnife) return;
            if (__instance.entity.World.Side != EnumAppSide.Server) return;

            // Get the final droprate
            float dropRate = Configuration.KnifeGetHarvestMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Knife"));

            // Integration
            dropRate = LevelKnifeEvents.GetExternalKnifeHarvest(byPlayer, __instance.entity, dropRate);

            // Don't worry, it will be reseted automatically by the game
            // 1 means 100%, but your formula return 1 by base, so lets put a -1f
            byPlayer.Entity.Stats.Set("animalLootDropRate", "animalLootDropRate", dropRate - 1f);

            // Earn xp by harvesting the entity
            Experience.IncreaseExperience(byPlayer, "Knife", "Harvest");

            Debug.LogDebug($"{byPlayer.PlayerName} harvested any entity with knife, multiply drop: {dropRate}/{byPlayer.Entity.Stats.GetBlended("animalLootDropRate")}");
        }
    }
}
public class LevelKnifeEvents
{
    public delegate void PlayerEntityFloatModifierHandler(IPlayer player, Entity harvestedEntity, ref float number);

    public static event PlayerEntityFloatModifierHandler OnKnifeHarvested;

    internal static float GetExternalKnifeHarvest(IPlayer player, Entity harvestedEntity, float multiply)
    {
        OnKnifeHarvested?.Invoke(player, harvestedEntity, ref multiply);
        return multiply;
    }
}