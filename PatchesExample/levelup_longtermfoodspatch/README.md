# LevelUP Long-term Food Patch

Grants LevelUP Farming exp for the crops added by the [Long-term Food](https://mods.vintagestory.at/longtermfood)
mod (domain `pemmican`): corn, potato, sugarbeet, sugarcane, aloe and healbulb.

## How it works

LevelUP keeps its per-crop farming exp table in `ModConfig/LevelUP/levelstats/farmingcrops.json`,
populated only with vanilla crops by default. This patch drops its crop exp values at
`assets/levelup_longtermfoodspatch/config/levelup-farmingcrops/pemmicancrops.json` and, on server start,
merges any key missing from `farmingcrops.json` in (never overwriting a value the server owner already
has), then re-runs `LevelUP.Configuration.PopulateFarmingConfiguration` so the new crops grant exp
immediately, without needing a second restart.

To add exp for crops from another mod, drop another `{"modid:crop-code-stage": expAmount, ...}` json at
that same asset path (in your own mod or a resourcepack) - no code changes needed.

## Adjusting the values

The bundled `pemmicancrops.json` values are a starting point (roughly matching the vanilla crops in
`Configuration.BuildFarmingCropsDefaultConfig` of similar growth stage count). Once the server has
generated `ModConfig/LevelUP/levelstats/farmingcrops.json`, edit the values there directly -
this patch never touches keys that already exist in that file.

## Building

Same as the other patches in `PatchesExample`: set the `VINTAGE_STORY` env var, then run `./build.ps1`
(Windows) or `./build.sh` (Linux/macOS). See `LevelUP_LongTermFoodsPatch.csproj` for the sibling `LevelUP`
project reference used only to re-populate the farming config during the same session.
