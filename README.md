# Level UP
Brand new levels for your character in Vintage Story, earn levels by doing actions, become stronger and efficient during the gameplay.
Makes you feels progress through the gameplay.

### IMPORTANT
- Consider always making backup for your world, level up is a mod that changes a lot of stats and mechanics that might crash your world/player.
- Is not safe to remove levelup after installation, full player wipe is required

Features:
- Fully configurable
- GUI Level in character view
- Hardcore mode
- Commands for administration manipulation
- Class experience manipulation
- Custom classes and items from other mods (in ModConfig)
- Survival Guide, for undestanding what each level will do
- Multi Language support
- API for modders [wiki](https://github.com/LeandroTheDev/level_up/wiki/Integration-&-Compatibility)

Not all item codes is added to the json configurations files, some armors/items/meats/entities/blocks can be missing, if you think something is missing from vanilla you can contact me or make a pull request in the github.

## Resume
The mods simple increase your status, use a lot of shovel? then your dig speed will be increased, you love bows and only use them? become a ranged killing machine increasing accuracy/damage and increasing chance to keep arrow after hit, you prefer to be a smithing and only smiths items? well other players will love you because your craftings now will have increased damage and durability.

### Observations
Configurations will be located in the folder ModConfig/LevelUP, if you want more informations you can see the [wiki](https://github.com/LeandroTheDev/level_up/wiki) to know what each configuration does, if any update from levelup add new configuration you will need to manually added them to fix logs errors, otherwises will have the default value.

Level UP Stores datas with the player UID, changing the player name will persist all levels.

This mod changes a lot of native codes and can break easily throught updates.

Level UP is a mod that adds several types of events to player actions, don't expect the mod to be lightweight (for big servers).

The experience data is saved everytime the world is saved or the player disconnect from the server, if the server dies unexpectedly, experiences may be lost for a few minutes since the last save.

LevelUP was built to work on large dedicated servers, experiences will be saved in the "ModData" folder separately for each user within the game, you can manually edit it or do whatever you want. (Edit while server is running will be overwrited by LevelUP, close server first)

English is not my main language, if you encounter any translation problems, please let me know.

# Mods Compatibility
| Mod            | Compatibility                                               |
|----------------|-------------------------------------------------------------|
| Item Rarity    | Not compatible with Smithing Level                          |
| XSkills        | Not compatible with Cooking Level                           |
| CombatOverhaul | Not compatible with Bow Level and some custom weapons       |
| Any mod that repair weapons | Requires attention in this [fix](https://github.com/LeandroTheDev/level_up/wiki/Integration-&-Compatibility#crafting)              |

"My custom weapon from my mod does not work with level up!!", check your weapon json, find the "tool" section:
```json
{
    "tool": "axe"
}
```
If the weapon doesn't have this add it to fix the problem, levelup support only: ``shovel,axe,spear,sword,pickaxe,hammer,knife``, "but my weapon uses a custom type that levelup does not support what i need to do?" in that case is more complicated, you need to edit your mod and use the [mod api](https://github.com/LeandroTheDev/level_up/wiki/Integration-&-Compatibility#levelupserveroverwritedamageinteractionevents) to listen for levelup event (OnPlayerMeleeDoDamageStart and OnPlayerRangedDoDamageStart), and make your own handling for the custom type, or alternatively but less recommended fork the project and change the [ReceiveDamageStart](https://github.com/LeandroTheDev/level_up/blob/main/LevelUP/Server/OverwriteDamageInteraction.cs) function, find the ``player.InventoryManager.ActiveTool`` and add the custom ``Enumeration``, my honest recommendation is to edit your mod for that situation.


"But my custom armor also don't work...", if your armor is made of: ``leather/chain/brigandine/plate/scale`` then you are lucky, open the configurations from one of the armor type and put the ``item id`` inside it, or create a [patch](https://wiki.vintagestory.at/Modding:JSON_Patching) to be persistent,  if not..., well you will need to integrate a custom level in your mod, in that case the [api](https://github.com/LeandroTheDev/level_up/wiki/Integration-&-Compatibility#custom-level) will help you.


"None of my food mods work with levelup!!!!", is very simple to fix that, the good way for your mod is to create a [patch](https://wiki.vintagestory.at/Modding:JSON_Patching), patch the ``assets/levelup/config/levelstats/cookingsingles.json`` for singular foods and ``assets/levelup/config/levelstats/cookingpots.json`` for custom pots, and the next time you generate the configurations will automatically be patched, alternatively you can manually edit ``ModConfig/LevelUP/config/levelstats/cooking...`` (This is the easist if you don't know how to patch, but is not persistent if you change the world or servers).


"I have custom classes and a lot of warning is called in my console", this is because you need to add your custom classes inside ``ModConfig/LevelUP/config/classexp``, you can create a [patch](https://wiki.vintagestory.at/Modding:JSON_Patching) or manually edit.

### [Examples in the Wiki](https://github.com/LeandroTheDev/level_up/wiki/Integration-&-Compatibility#examples)

### Warning
Creating a [patch](https://wiki.vintagestory.at/Modding:JSON_Patching) will not automatically update the ModConfig folder (if generated), after the [patch](https://wiki.vintagestory.at/Modding:JSON_Patching) creation you must manually update ModConfig or delete the LevelUP configs and re-generate.

# Important Configurations
Vitality will overwrite the health system, you need manually change configurations in ``levelstats/vitality.json`` changing in default game configuration will have no effects, if you don't want this feature consider disabling in configuration before generating or joining in the world

Metabolism works as the same as vitality, changing base saturation ingame will have no effect you need to change in ``levelstats/metabolis.json``

Debug logs is by default disabled, if you need to send logs you can send logs to [issues](https://github.com/LeandroTheDev/level_up/issues) in case of errors and bugs, you can enable it in base.json: ``enableExtendedLog``, this also can cause some cpu performances problems in low end cpus.

I strongly recommend to send game breaking bugs to [issues](https://github.com/LeandroTheDev/level_up/issues), i don't receive notifications in vsmoddb

# About Level UP
Level UP is open source project and can easily be accessed on the github, all contents from this mod is completly free.

If you want to contribute into the project you can access the project github and make your pull request.

You are free to fork the project and make your own version of Level UP, as long the name is changed.

Inspirations: 
- Valheim Level UP System
- Runescape Level UP System
- Minecraft LevelZ mod
- Project Zomboid Level UP System

# Building
- Install .NET in your system, open terminal type: ``dotnet new install VintageStory.Mod.Templates``
- Create a template with the name ``LevelUP``: ``dotnet new vsmod --AddSolutionFile -o LevelUP``
- [Clone the repository](https://github.com/LeandroTheDev/level_up/archive/refs/heads/main.zip)
- Copy the ``CakeBuild`` and ``build.ps1`` or ``build.sh`` and paste inside the repository

Now you can build using the ``build.ps1`` or ``build.sh`` file

FTM License