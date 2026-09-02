#pragma warning disable CA1822
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using LevelUP.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace LevelUP.Server;

class LevelVitality
{
    public readonly Harmony patch = new("levelup_vitality");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_vitality"))
        {
            patch.PatchCategory("levelup_vitality");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_vitality"))
        {
            patch.UnpatchCategory("levelup_vitality");
        }
    }

    public void Init()
    {
        OverwriteDamageInteractionEvents.OnPlayerReceiveDamageStart += HandleReceiveDamage;
        Configuration.RegisterNewLevel("Vitality");
        Configuration.RegisterNewLevelTypeEXP("Vitality", Configuration.VitalityGetLevelByEXP);
        Configuration.RegisterNewEXPLevelType("Vitality", Configuration.VitalityGetExpByLevel);

        Debug.Log("Level Vitality initialized");
    }

    public void InitClient()
    {
        StatusViewEvents.OnStatusRequested += StatusViewRequested;

        Debug.Log("Level Vitality initialized");
    }

    public void Dispose()
    {
        OverwriteDamageInteractionEvents.OnPlayerReceiveDamageStart -= HandleReceiveDamage;
        StatusViewEvents.OnStatusRequested -= StatusViewRequested;
    }

    private void StatusViewRequested(IPlayer player, ref StringBuilder stringBuilder, string levelType)
    {
        if (levelType != "Vitality") return;

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_regen",
                Utils.GetPorcentageFromFloatsStart1(Configuration.VitalityGetHealthRegenMultiplyByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Vitality")))
            )
        );

        stringBuilder.AppendLine(
            Lang.Get("levelup:status_additionalhealth",
                Configuration.VitalityGetMaxHealthByLevel(player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Vitality")) - Configuration.BaseHPVitality
            )
        );
    }

    private void HandleReceiveDamage(IPlayer player, DamageSource damageSource, ref float damage)
    {
        float receivedDamage = damage;
        // Running on secondary thread to not suffocate the thread
        Task.Run(() =>
        {
            if (receivedDamage < Configuration.DamageLimitVitality)
            {
                // Check if damage is bigger than player max health
                float damageCalculation = receivedDamage;
                float playerMaxHealth = player.Entity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) ?? 15f;
                // If is set the damage experience limit to the player max health
                if (player.Entity.WatchedAttributes.GetTreeAttribute("health")?.GetFloat("basemaxhealth", 15f) < receivedDamage) damageCalculation = playerMaxHealth;

                Experience.IncreaseExperience(player, "Vitality", (ulong)Configuration.VitalityEXPEarnedByDAMAGE(damageCalculation));
            }
        });
    }

    public void PopulateConfiguration(ICoreAPI coreAPI)
    {
        // Populate configuration
        Configuration.PopulateVitalityConfiguration(coreAPI);
        Configuration.RegisterNewMaxLevelByLevelTypeEXP("Vitality", Configuration.vitalityMaxLevel);
    }

    static public EntityBehaviorHealth RefreshMaxHealth(IPlayer player)
    {
        // Get the actual player total exp
        ulong playerExp = Experience.GetExperience(player, "Vitality");

        // Get player stats
        EntityBehaviorHealth playerStats = player.Entity.GetBehavior<EntityBehaviorHealth>();
        // Check if stats is null
        if (playerStats == null) { Debug.LogError($"[VITALITY] ERROR SETTING MAX HEALTH: Player Stats is null, caused by {player.PlayerName}"); return playerStats; }

        // Getting health stats
        float playerMaxHealth = Configuration.VitalityGetMaxHealthByLevel(Configuration.VitalityGetLevelByEXP(playerExp));
        if (float.IsInfinity(playerMaxHealth))
        {
            Debug.LogError($"[VITALITY] ERROR: Max health calculation returned any infinity number, please report this issue, base health set to {Configuration.BaseHPVitality}");
            playerMaxHealth = Configuration.BaseHPVitality;
        }

        // Getting regen stats
        float playerRegen = Configuration.VitalityGetHealthRegenMultiplyByLevel(Configuration.VitalityGetLevelByEXP(playerExp));
        if (float.IsInfinity(playerRegen))
        {
            Debug.LogError($"[VITALITY] ERROR: Regeneration calculation returned any infinity number, please report this issue, base regen set to {Configuration.BaseHPRegenVitality}");
            playerRegen = Configuration.BaseHPRegenVitality;
        }

        playerStats.BaseMaxHealth = playerMaxHealth;
        playerStats.MaxHealth = playerMaxHealth;

        // "base" already contributes 1 to the WeightedSum blend, so only the delta above that
        // baseline goes in our own code - see the levelup_vitality entry read by the transpiler below
        player.Entity.Stats.Set("regenSpeed", "levelup_vitality", playerRegen - 1f);

        // Refresh for the player
        playerStats.UpdateMaxHealth();

        return playerStats;
    }

    [HarmonyPatchCategory("levelup_vitality")]
    private class LevelVitalityPatch
    {
        // Transpiler to edit healthRegenSpeed to receive entity.Stats.GetBlended("regenSpeed")
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EntityBehaviorHealth), "ApplyRegenAndHunger")]
        internal static List<CodeInstruction> ApplyRegenAndHunger_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var entityField = AccessTools.Field(typeof(EntityBehaviorHealth), "entity");
            var statsField = AccessTools.Field(typeof(Entity), "Stats");
            var getBlended = AccessTools.Method(typeof(EntityStats), "GetBlended", [typeof(string)]);

            for (int i = 0; i < codes.Count; i++)
            {
                // Local 2 = healthRegenSpeed
                if (codes[i].opcode == OpCodes.Stloc_2)
                {
                    var inject = new List<CodeInstruction>()
                    {
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldfld, entityField),

                        // entity.Stats
                        new(OpCodes.Ldfld, statsField),

                        // "regenSpeed"
                        new(OpCodes.Ldstr, "regenSpeed"),

                        // callvirt EntityStats.GetBlended (returns 1f when the category was never set, same as before)
                        new(OpCodes.Callvirt, getBlended),

                        // Save result into local 2
                        new(OpCodes.Stloc_2)
                    };

                    // Insert afterstloc.2 original
                    codes.InsertRange(i + 1, inject);

                    Debug.LogDebug("healthRegenSpeed overwrite by entity.Stats.GetBlended(\"regenSpeed\")");
                    break;
                }
            }

            return codes;
        }

    }
}