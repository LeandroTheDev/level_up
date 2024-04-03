using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.Json;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace LevelUP.Server;

[HarmonyPatch]
class LevelHunter
{
    private Instance instance;
    Harmony overrider;

    readonly Dictionary<string, int> entityExp = [];

    public void Init(Instance _instance)
    {
        instance = _instance;
        // Instanciate death event
        instance.api.Event.OnEntityDeath += OnEntityDeath;

        //Dummy data
        entityExp["Dead drifter"] = 10;
        entityExp["Dead bear"] = 50;
        entityExp["Dead rooster"] = 1;

        Debug.Log("Level Hunter initialized");
    }

    private Dictionary<string, int> GetSavedLevels()
    {
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData("LevelUPData_Hunter");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(data);
    }

    private void SaveLevels(Dictionary<string, int> HunterLevels)
    {
        instance.api.WorldManager.SaveGame.StoreData("LevelUPData_Hunter", JsonSerializer.Serialize(HunterLevels));
    }

    public void OnEntityDeath(Entity entity, DamageSource damageSource)
    {
        // Entity kill is not from a player
        if (damageSource.SourceEntity == null) return;
        if (damageSource.SourceEntity is not EntityPlayer) return;

        // Get all players levels
        Dictionary<string, int> hunterLevels = GetSavedLevels();

        // Get the exp received
        int exp = entityExp.GetValueOrDefault(entity.GetName(), 0);

        // Get the actual player total exp
        int playerExp = hunterLevels.GetValueOrDefault(damageSource.SourceEntity.GetName(), 0);

        Debug.Log($"Entity died: {entity.GetName()}, by: {damageSource.SourceEntity.GetName()} exp earned: {exp}, actual: {playerExp}");

        // Incrementing
        hunterLevels[damageSource.SourceEntity.GetName()] = playerExp + exp;

        // Saving
        SaveLevels(hunterLevels);
        // Updating
        damageSource.SourceEntity.WatchedAttributes.SetInt("LevelUP_Hunter", playerExp + exp);
    }


    public void OverrideNativeFunctions()
    {
        if (!Harmony.HasAnyPatches("levelup"))
        {
            overrider = new Harmony("levelup");
            // Applies all harmony patches
            overrider.PatchAll();
            Debug.Log("Server Damage function has been overrited");
        }
    }
    public void OverrideDispose()
    {
        // Unpatch if world exist
        overrider?.UnpatchAll("levelup");
    }

    // Override Damage Interaction
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Entity), "ReceiveDamage")]
    public static bool ReceiveDamage(Entity __instance, DamageSource damageSource, float damage)
    {
        damage *= 4;
        if ((!__instance.Alive || __instance.IsActivityRunning("invulnerable")) && damageSource.Type != EnumDamageType.Heal)
        {
            return false;
        }
        if (__instance.ShouldReceiveDamage(damageSource, damage))
        {
            foreach (EntityBehavior behavior in __instance.SidedProperties.Behaviors)
            {
                behavior.OnEntityReceiveDamage(damageSource, ref damage);
            }
            if (damageSource.Type != EnumDamageType.Heal && damage > 0f)
            {
                __instance.WatchedAttributes.SetInt("onHurtCounter", __instance.WatchedAttributes.GetInt("onHurtCounter") + 1);
                __instance.WatchedAttributes.SetFloat("onHurt", damage);
                if (damage > 0.05f)
                {
                    __instance.AnimManager.StartAnimation("hurt");
                }
            }
            if (damageSource.GetSourcePosition() != null)
            {
                Vec3d dir = (__instance.SidedPos.XYZ - damageSource.GetSourcePosition()).Normalize();
                dir.Y = 0.699999988079071;
                float factor = damageSource.KnockbackStrength * GameMath.Clamp((1f - __instance.Properties.KnockbackResistance) / 10f, 0f, 1f);
                __instance.WatchedAttributes.SetFloat("onHurtDir", (float)Math.Atan2(dir.X, dir.Z));
                __instance.WatchedAttributes.SetDouble("kbdirX", dir.X * (double)factor);
                __instance.WatchedAttributes.SetDouble("kbdirY", dir.Y * (double)factor);
                __instance.WatchedAttributes.SetDouble("kbdirZ", dir.Z * (double)factor);
            }
            else
            {
                __instance.WatchedAttributes.SetDouble("kbdirX", 0.0);
                __instance.WatchedAttributes.SetDouble("kbdirY", 0.0);
                __instance.WatchedAttributes.SetDouble("kbdirZ", 0.0);
                __instance.WatchedAttributes.SetFloat("onHurtDir", -999f);
            }
            return damage > 0f;
        }
        return false;
    }

    // // Override Damage Interaction
    // [HarmonyTranspiler]
    // [HarmonyPatch(typeof(EntityAgent), "ReceiveDamage")]
    // public static IEnumerable<CodeInstruction> ReceiveDamage(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    // {
    //     return new CodeMatcher(instructions, generator)
    //         .MatchEndForward(new CodeMatch(OpCodes.Ldarg_1, null, "damage"))
    //         .SetOperandAndAdvance(0.0f)
    //         .InstructionEnumeration();
    // }
}