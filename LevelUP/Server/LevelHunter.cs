#pragma warning disable CA1822
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;

namespace LevelUP.Server;

class LevelHunter
{
    public readonly Harmony patch = new("levelup_hunter");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_hunter"))
        {
            patch.PatchCategory("levelup_hunter");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_hunter"))
        {
            patch.UnpatchCategory("levelup_hunter");
        }
    }

    public void Init()
    {
        // Instanciate death event
        Instance.api.Event.OnEntityDeath += OnEntityDeath;
        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart += HandleDamage;
        Configuration.RegisterNewLevel("Hunter");
        Configuration.RegisterNewLevelTypeEXP("Hunter", Configuration.HunterGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Hunter", Configuration.HunterGetExpByLevel);

        Debug.Log("Level Hunter initialized");
    }

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Hunter initialized");
    }

    public void Dispose()
    {
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;

        OverwriteDamageInteractionEvents.OnPlayerMeleeDoDamageStart -= HandleDamage;
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Hunter") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_damage",
                Configuration.HunterGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Hunter"))
            )
        );
    }

    private void HandleDamage(IPlayer player, DamageSource damageSource, ref float damage)
        => damage *= Configuration.HunterGetDamageMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Hunter"));

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateHunterConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Hunter", Configuration.hunterMaxLevel);
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Error treatment
        if (damageSource == null) return;
        // Get player entity
        EntityPlayer playerEntity;
        if (damageSource.SourceEntity is EntityPlayer) playerEntity = damageSource.SourceEntity as EntityPlayer;
        else if (damageSource.GetCauseEntity() is EntityPlayer) playerEntity = damageSource.GetCauseEntity() as EntityPlayer;
        else return;

        // Get player instance
        IPlayer player = playerEntity.Player;

        ulong exp = (ulong)Configuration.entityExpHunter.GetValueOrDefault(entity.Code.ToString());

        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Hunter");

        Debug.LogDebug($"{player.PlayerName} killed: {entity.Code}, hunter exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        Experience.IncreaseExperience(player, "Hunter", exp);
    }

    [HarmonyPatchCategory("levelup_hunter")]
    private class LevelHunterPatch
    { }
}