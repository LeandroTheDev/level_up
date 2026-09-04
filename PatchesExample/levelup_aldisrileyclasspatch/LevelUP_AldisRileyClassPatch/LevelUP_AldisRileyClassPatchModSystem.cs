using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LevelUP;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;

namespace LevelUP_AldisRileyClassPatch;

/// <summary>
/// LevelUP writes its per-class multiplier config to
/// ModConfig/LevelUP/classexp/&lt;classCode&gt;class.json and, on every start,
/// loads ALL files it finds in that folder (Configuration.PopulateClassConfigurations).
/// This already allows adding new classes by just dropping a json there - the problem is
/// that it requires manually touching the server's data folder.
///
/// This mod solves that by using Vintage Story's asset system: the jsons live in
/// assets/&lt;domain&gt;/config/levelup-classexp/&lt;classCode&gt;class.json inside ANY installed
/// mod (not just this one), and here we collect them all with api.Assets.GetMany(...) and copy
/// them into ModConfig/LevelUP/classexp/ if the file doesn't already exist.
///
/// In other words: to add a new class, just drop a .json at that asset path
/// (in your own mod/resourcepack) - no need to edit C# or touch the data folder.
///
/// If the target file already exists, it is never overwritten - but keys present in the
/// asset json that are missing from the existing file (e.g. added by a mod update) are
/// backfilled in, so server owners keep their manual edits while still getting new keys.
///
/// Since this mod depends on LevelUP, LevelUP's own AssetsLoaded (which reads classexp/
/// and populates its in-memory class configs) runs BEFORE this one. So on a fresh server,
/// or the first boot after adding a new class, LevelUP would only pick up the file we just
/// wrote on the NEXT restart. To avoid that, we re-run LevelUP.Configuration.PopulateClassConfigurations
/// ourselves after writing/backfilling anything, so the new class is live immediately.
/// </summary>
public class LevelUP_AldisRileyClassPatchModSystem : ModSystem
{
    private const string ClassExpAssetPath = "config/levelup-classexp/";
    private const string ClassExpConfigSubPath = "ModConfig/LevelUP/classexp";

    public override void AssetsLoaded(ICoreAPI api)
    {
        base.AssetsLoaded(api);

        if (api.Side != EnumAppSide.Server) return;

        string targetDir = Path.Combine(api.DataBasePath, ClassExpConfigSubPath);
        Directory.CreateDirectory(targetDir);

        bool anyFileChanged = false;

        List<IAsset> classAssets = api.Assets.GetMany(ClassExpAssetPath);
        foreach (IAsset asset in classAssets)
        {
            string className = Path.GetFileNameWithoutExtension(asset.Location.Path);
            string targetFile = Path.Combine(targetDir, className + ".json");
            string assetJson = Encoding.UTF8.GetString(asset.Data);

            if (File.Exists(targetFile))
            {
                anyFileChanged |= BackfillMissingKeys(api, targetFile, className, assetJson);
                continue;
            }

            try
            {
                File.WriteAllText(targetFile, assetJson);
                anyFileChanged = true;
                api.Logger.Notification($"[LevelUP_AldisRileyClassPatch] {className}.json created from {asset.Location} (mod: {asset.Location.Domain})");
            }
            catch (Exception ex)
            {
                api.Logger.Error($"[LevelUP_AldisRileyClassPatch] Failed to write {targetFile}: {ex.Message}");
            }
        }

        if (anyFileChanged) Configuration.PopulateClassConfigurations(api);
    }

    /// <summary>
    /// Adds keys from the asset json that are missing from the existing file, without
    /// touching keys the server owner already has (default or manually edited).
    /// Returns whether the file was changed.
    /// </summary>
    private static bool BackfillMissingKeys(ICoreAPI api, string targetFile, string className, string assetJson)
    {
        try
        {
            JObject existing = JObject.Parse(File.ReadAllText(targetFile));
            JObject defaults = JObject.Parse(assetJson);

            bool missingKeyAdded = false;
            foreach (JProperty property in defaults.Properties())
            {
                if (existing.ContainsKey(property.Name)) continue;

                existing[property.Name] = property.Value;
                missingKeyAdded = true;
                api.Logger.Notification($"[LevelUP_AldisRileyClassPatch] Key '{property.Name}' missing from {className}.json, adding it with its default value");
            }

            if (!missingKeyAdded) return false;

            File.WriteAllText(targetFile, existing.ToString());
            api.Logger.Notification($"[LevelUP_AldisRileyClassPatch] {className}.json updated with new default keys");
            return true;
        }
        catch (Exception ex)
        {
            api.Logger.Error($"[LevelUP_AldisRileyClassPatch] Failed to backfill keys for {targetFile}: {ex.Message}");
            return false;
        }
    }
}
