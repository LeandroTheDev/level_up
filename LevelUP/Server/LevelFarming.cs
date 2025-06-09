using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LevelUP.Server;

class LevelFarming
{
    public void Init()
    {
        // Instanciate break block event
        Instance.api.Event.BreakBlock += OnBreakBlock;

        Debug.Log("Level Farming initialized");
    }

#pragma warning disable CA1822
    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateFarmingConfiguration(coreAPI);
    }
#pragma warning restore CA1822

    public void OnBreakBlock(IServerPlayer player, BlockSelection breakedBlock, ref float dropQuantityMultiplier, ref EnumHandling handling)
    {
        // Get the exp received
        ulong exp = (ulong)Configuration.expPerHarvestFarming.GetValueOrDefault(breakedBlock.Block.Code.ToString(), 0);
        // No crop exp finded
        if (exp <= 0) return;

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Farming");
        if (Configuration.FarmingIsMaxLevel(playerExp)) return;

        if (Configuration.enableLevelUpExperienceServerLog)
            Debug.Log($"{player.PlayerName} breaked: {breakedBlock.Block.Code}, farming exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Farming", exp);
    }
}