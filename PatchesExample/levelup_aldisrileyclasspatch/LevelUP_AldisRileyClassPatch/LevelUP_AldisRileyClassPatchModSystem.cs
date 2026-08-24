using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;

namespace LevelUP_AldisRileyClassPatch;

/// <summary>
/// LevelUP writes its per-class multiplier config to
/// ModConfig/LevelUP/config/classexp/&lt;classCode&gt;class.json and, on every start,
/// loads ALL files it finds in that folder (Configuration.PopulateClassConfigurations).
/// This already allows adding new classes by just dropping a json there - the problem is
/// that it requires manually touching the server's data folder.
///
/// This mod solves that by using Vintage Story's asset system: the jsons live in
/// assets/&lt;domain&gt;/config/levelup-classexp/&lt;classCode&gt;class.json inside ANY installed
/// mod (not just this one), and here we collect them all with api.Assets.GetMany(...) and copy
/// them into ModConfig/LevelUP/config/classexp/ if the file doesn't already exist.
///
/// In other words: to add a new class, just drop a .json at that asset path
/// (in your own mod/resourcepack) - no need to edit C# or touch the data folder.
///
/// If the target file already exists, it is never overwritten - but keys present in the
/// asset json that are missing from the existing file (e.g. added by a mod update) are
/// backfilled in, so server owners keep their manual edits while still getting new keys.
/// </summary>
public class LevelUP_AldisRileyClassPatchModSystem : ModSystem
{
    private const string ClassExpAssetPath = "config/levelup-classexp/";
    private const string ClassExpConfigSubPath = "ModConfig/LevelUP/config/classexp";

    public override void AssetsLoaded(ICoreAPI api)
    {
        base.AssetsLoaded(api);

        if (api.Side != EnumAppSide.Server) return;

        string targetDir = Path.Combine(api.DataBasePath, ClassExpConfigSubPath);
        Directory.CreateDirectory(targetDir);

        List<IAsset> classAssets = api.Assets.GetMany(ClassExpAssetPath);
        foreach (IAsset asset in classAssets)
        {
            string className = Path.GetFileNameWithoutExtension(asset.Location.Path);
            string targetFile = Path.Combine(targetDir, className + ".json");
            string assetJson = Encoding.UTF8.GetString(asset.Data);

            if (File.Exists(targetFile))
            {
                BackfillMissingKeys(api, targetFile, className, assetJson);
                continue;
            }

            try
            {
                File.WriteAllText(targetFile, assetJson);
                api.Logger.Notification($"[LevelUP_AldisRileyClassPatch] {className}.json created from {asset.Location} (mod: {asset.Location.Domain})");
            }
            catch (Exception ex)
            {
                api.Logger.Error($"[LevelUP_AldisRileyClassPatch] Failed to write {targetFile}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Adds keys from the asset json that are missing from the existing file, without
    /// touching keys the server owner already has (default or manually edited).
    /// </summary>
    private static void BackfillMissingKeys(ICoreAPI api, string targetFile, string className, string assetJson)
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

            if (!missingKeyAdded) return;

            File.WriteAllText(targetFile, existing.ToString());
            api.Logger.Notification($"[LevelUP_AldisRileyClassPatch] {className}.json updated with new default keys");
        }
        catch (Exception ex)
        {
            api.Logger.Error($"[LevelUP_AldisRileyClassPatch] Failed to backfill keys for {targetFile}: {ex.Message}");
        }
    }
}
