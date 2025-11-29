#pragma warning disable CA1822
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelPickaxe
{
    public readonly Harmony patch = new("levelup_pickaxe");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_pickaxe"))
        {
            patch.PatchCategory("levelup_pickaxe");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_pickaxe"))
        {
            patch.UnpatchCategory("levelup_pickaxe");
        }
    }

    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        // Instanciate break block event
        Instance.api.Event.BreakBlock += OnBreakBlock;
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart += HandleDamage;
        Configuration.RegisterNewLevel("Pickaxe");
        Configuration.RegisterNewLevelTypeEXP("Pickaxe", Configuration.PickaxeGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Pickaxe", Configuration.PickaxeGetExpByLevel);

        Debug.Log("Level Pickaxe initialized");
    }

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Pickaxe initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart -= HandleDamage;
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Pickaxe") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_damage",
                Utils.GetPorcentageFromFloatsStart1(Configuration.PickaxeGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_miningspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.PickaxeGetMiningMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_ore",
                Utils.GetPorcentageFromFloatsStart0(Configuration.PickaxeGetOreMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe")))
            )
        );
    }

    private void HandleDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (player.InventoryManager.ActiveTool == EnumTool.Pickaxe)
        {
            damage *= Configuration.PickaxeGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe"));
            Experience.IncreaseExperience(player, "Pickaxe", "Hit");
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulatePickaxeConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Pickaxe", Configuration.pickaxeMaxLevel);
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

        // Check if player is using a Pickaxe
        if (player.InventoryManager.ActiveTool != EnumTool.Pickaxe) return;

        // Get the exp received
        ulong exp = (ulong)Configuration.entityExpPickaxe.GetValueOrDefault(entity.Code.ToString());

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Pickaxe");

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, pickaxe exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Pickaxe", exp);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        // If not a shovel ignore
        if (player.InventoryManager.ActiveTool != EnumTool.Pickaxe) return;
        if (breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Stone && breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Ore) return;

        // Get the exp received
        ulong exp = (ulong)Configuration.oresExpPickaxe.GetValueOrDefault(breakedBlock.Block.Code.ToString(), Configuration.ExpPerBreakingPickaxe);

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Pickaxe");

        Debug.LogDebug($"{player.PlayerName} breaked: {breakedBlock.Block.Code}, pickaxe exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Pickaxe", exp);
    }

    [HarmonyPatchCategory("levelup_pickaxe")]
    private class LevelPickaxePatch
    {
        // Overwrite Ores Drop
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockOre), "OnBlockBroken")]
        public static void OnBlockBroken(BlockOre __instance, IWorldAccessor world, IPlayer byPlayer, ref float dropQuantityMultiplier)
        {
            if (!Configuration.enableLevelPickaxe) return;
            if (world.Side != EnumAppSide.Server) return;
            if (byPlayer == null) return;

            // Increasing ore drop rate
            dropQuantityMultiplier += Configuration.PickaxeGetOreMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe"));

            // Integration
            dropQuantityMultiplier = LevelPickaxeEvents.GetExternalMiningMultiplier(byPlayer, dropQuantityMultiplier);

            Debug.LogDebug($"{byPlayer.PlayerName} breaked a ore, multiply drop: {Configuration.PickaxeGetOreMultiplyByLevel(byPlayer.Entity.WatchedAttributes.GetInt("LevelUP_Level_Pickaxe"))}");
        }

    }
}

public class LevelPickaxeEvents
{
    public delegate void PlayerMiningOre(IPlayer player, ref float dropQuantityMultiplier);

    public static event PlayerMiningOre OnMiningOre;

    internal static float GetExternalMiningMultiplier(IPlayer player, float multiply)
    {
        OnMiningOre?.Invoke(player, ref multiply);
        return multiply;
    }
}