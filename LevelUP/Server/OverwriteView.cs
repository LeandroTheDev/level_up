using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace LevelUP.Server;

[HarmonyPatchCategory("levelup_view")]
class OverwriteView
{
    public readonly Harmony patch = new("levelup_view");
    public void Patch()
    {
        if (!Harmony.HasAnyPatches("levelup_view"))
        {
            patch.PatchCategory("levelup_view");
        }
    }
    public void Unpatch()
    {
        if (Harmony.HasAnyPatches("levelup_view"))
        {
            patch.UnpatchCategory("levelup_view");
        }
    }
}

