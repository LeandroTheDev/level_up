#pragma warning disable CA1822
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelAxe
{
    public readonly Harmony patch = new("levelup_axe");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_axe"))
        {
            patch.PatchCategory("levelup_axe");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_axe"))
        {
            patch.UnpatchCategory("levelup_axe");
        }
    }

    public void Init()
    {
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        Instance.api.Event.BreakBlock += OnBreakBlock;
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart += HandleDamage;
        Configuration.RegisterNewLevel("Axe");
        Configuration.RegisterNewLevelTypeEXP("Axe", Configuration.AxeGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Axe", Configuration.AxeGetExpByLevel);

        Debug.Log("Level Axe initialized");
    }

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested += StatusViewRequested;
        OverwriteBlockBreakEvents.OnMiningSpeedRefreshed += RefreshMiningSpeed;
        Client.Instance.RefreshWatchedAttributes += RefreshWatchedAttributes;
        RefreshWatchedAttributes();

        Debug.Log("Level Axe initialized");
    }

    public void Dispose()
    {
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
        OverwriteBlockBreakEvents.OnMiningSpeedRefreshed -= RefreshMiningSpeed;
        Client.Instance.RefreshWatchedAttributes -= RefreshWatchedAttributes;
    }

    static private float currentAxeMiningSpeed = 1.0f;
    private void RefreshWatchedAttributes()
    {
        var api = Shared.Instance.api as ICoreClientAPI;
        currentAxeMiningSpeed = api.World.Player.Entity.WatchedAttributes.GetFloat("LevelUP_Axe_MiningSpeed");
    }
    private void RefreshMiningSpeed(CollectibleObject collectible, IItemStack itemstack, BlockSelection blockSel, Block block, IPlayer player, ref float multiply)
    {
        if (block.BlockMaterial == EnumBlockMaterial.Wood)
            multiply *= currentAxeMiningSpeed;
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Axe") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_miningspeed",
                Utils.GetPorcentageFromFloatsStart1(Configuration.AxeGetMiningMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Axe")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_damage",
                Utils.GetPorcentageFromFloatsStart1(Configuration.AxeGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Axe")))
            )
        );
    }

    private void HandleDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        if (player.InventoryManager.ActiveTool == EnumTool.Axe)
        {
            damage *= Configuration.AxeGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Axe"));
            Experience.IncreaseExperience(player, "Axe", "Hit");
        }
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateAxeConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Axe", Configuration.axeMaxLevel);
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

        // Check if player is using a Axe
        if (player.InventoryManager.ActiveTool != EnumTool.Axe) return;

        ulong exp = (ulong)Configuration.entityExpAxe.GetValueOrDefault(entity.Code.ToString(), 0);
        if (exp <= 0) return;
        Experience.IncreaseExperience(player, "Axe", exp);
    }

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        // If not a axe ignore
        if (player.InventoryManager.ActiveTool != EnumTool.Axe) return;
        if (breakedBlock.Block.BlockMaterial != EnumBlockMaterial.Wood) return;

        ulong exp = (ulong)Configuration.ExpPerBreakingAxe;
        Experience.IncreaseExperience(player, "Axe", exp);
    }

    [HarmonyPatchCategory("levelup_axe")]
    private class LevelAxePatch
    {
        // Overwrite Wood Axe Tree Breaking
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemAxe), "OnBlockBrokenWith")]
        public static void OnBlockBrokenWith(ItemAxe __instance, IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier = 1f)
        {
            if (!Configuration.enableLevelAxe) return;

            // Check if axe breaked is a player
            if (byEntity is EntityPlayer)
            {
                Experience.IncreaseExperience((byEntity as EntityPlayer).Player, "Axe", "TreeBreak");
            }
        }

    }
}