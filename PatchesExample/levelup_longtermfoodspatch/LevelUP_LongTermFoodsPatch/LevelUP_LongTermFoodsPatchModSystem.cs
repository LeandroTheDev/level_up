using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LevelUP;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;

namespace LevelUP_LongTermFoodsPatch;

/// <summary>
/// LevelUP writes its per-crop farming exp config to
/// ModConfig/LevelUP/levelstats/farmingcrops.json (Configuration.PopulateFarmingConfiguration),
/// backfilling only the crops it knows about by default (vanilla). Crop blocks added by other mods
/// (like Long-term Food's "pemmican" crops) are never granted farming exp/level unless a server owner
/// manually adds them to that file.
///
/// This mod solves that by using Vintage Story's asset system: the exp jsons live at
/// assets/&lt;domain&gt;/config/levelup-farmingcrops/*.json inside ANY installed mod (not just this
/// one), and here we collect them all with api.Assets.GetMany(...) and merge their keys into
/// farmingcrops.json if they are not already present.
///
/// In other words: to grant farming exp for a new crop block, just drop a
/// {"modid:crop-code": expAmount, ...} .json at that asset path (in your own mod/resourcepack) -
/// no need to edit C# or touch the data folder.
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
public class LevelUP_LongTermFoodsPatchModSystem : ModSystem
{
    private const string FarmingCropsAssetPath = "config/levelup-farmingcrops/";
    private const string FarmingCropsConfigSubPath = "ModConfig/LevelUP/levelstats";
    private const string FarmingCropsConfigFile = "farmingcrops.json";

    public override void AssetsLoaded(ICoreAPI api)
    {
        base.AssetsLoaded(api);

        if (api.Side != EnumAppSide.Server) return;

        string targetDir = Path.Combine(api.DataBasePath, FarmingCropsConfigSubPath);
        Directory.CreateDirectory(targetDir);
        string targetFile = Path.Combine(targetDir, FarmingCropsConfigFile);

        List<IAsset> cropExpAssets = api.Assets.GetMany(FarmingCropsAssetPath);
        if (cropExpAssets.Count == 0) return;

        bool anyKeyAdded = false;

        JObject existing;
        try
        {
            existing = File.Exists(targetFile) ? JObject.Parse(File.ReadAllText(targetFile)) : new JObject();
        }
        catch (Exception ex)
        {
            api.Logger.Error($"[LevelUP_LongTermFoodsPatch] Failed to read {targetFile}: {ex.Message}");
            return;
        }

        foreach (IAsset asset in cropExpAssets)
        {
            JObject defaults;
            try
            {
                defaults = JObject.Parse(Encoding.UTF8.GetString(asset.Data));
            }
            catch (Exception ex)
            {
                api.Logger.Error($"[LevelUP_LongTermFoodsPatch] Failed to parse {asset.Location}: {ex.Message}");
                continue;
            }

            foreach (JProperty property in defaults.Properties())
            {
                if (existing.ContainsKey(property.Name)) continue;

                existing[property.Name] = property.Value;
                anyKeyAdded = true;
                api.Logger.Notification($"[LevelUP_LongTermFoodsPatch] Crop exp key '{property.Name}' missing from {FarmingCropsConfigFile}, adding it from {asset.Location} (mod: {asset.Location.Domain})");
            }
        }

        if (!anyKeyAdded) return;

        try
        {
            File.WriteAllText(targetFile, existing.ToString());
        }
        catch (Exception ex)
        {
            api.Logger.Error($"[LevelUP_LongTermFoodsPatch] Failed to write {targetFile}: {ex.Message}");
            return;
        }

        Configuration.PopulateFarmingConfiguration(api);
    }
}
