using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace LevelUP.Client;

[HarmonyPatch]
class LevelHunter
{
    private Instance instance;
    Harmony overrider;

    public void OverrideNativeFunctions()
    {
        if (!Harmony.HasAnyPatches("levelup"))
        {
            overrider = new Harmony("levelup");
            // Applies all harmony patches
            overrider.PatchAll();
            Debug.Log("Client Damage function has been overrited");
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
    public static void IncreaseDamage(Entity __instance, DamageSource damageSource, float damage)
    {
        
    }
}