# Level UP
Add a new mechanic to vintage story, a new level system to make your character stronger and efficient during the gameplay, making your feels you are progressing through the gameplay.

### IMPORTANT
This mod drastically changes the player status and its not recoverable in normal ways, PLEASE make a backup of your world, to reset players status you will need to use specific commands, please review the [Wiki-Commands](https://github.com/LeandroTheDev/level_up/wiki/Commands) to get more information to reset player status.

Consider always making backup for your world, level up is a mod that changes a lot of stats and mechanics that might crash your world/player.

Features:
- Fully configurable
- GUI Level in character view
- Hardcore mode
- Commands for administration manipulation
- Class experience manipulation
- Custom classes and items from other mods (in ModConfig)
- Survival Guide, for undestanding what each level will do
- Multi Language support

Not all item codes is added to the json configurations files, some armors/items/meats/entities/blocks can be missing, if you think something is missing from vanilla you can contact me or make a pull request in the github.

### Observations
To change the configurations go to the vintage story data, for windows in appdata for linux in .config for server is the path you set in serverconfig.json, find the folder ModConfig/LevelUP, if you want more informations you can see the [wiki](https://github.com/LeandroTheDev/level_up/wiki) to know what each configuration does, if any update from levelup add new configuration you will need to manually added them to fix logs errors, otherwises will have the default value.

Level UP Data will be stored in ModData/LevelUP/WorldIdentifier.

Level UP Stores datas with the player UID, changing the player name will persist all levels.

This mod changes a lot of native codes and can break easily throught updates.

Level UP is a mod that adds several types of events to player actions, don't expect the mod to be lightweight.

Hardcore mode will not reduce the level, only the progress to the next level.

The experience data is saved everytime the world is saved or the player disconnect from the server, if the server dies unexpectedly, experiences may be lost for a few minutes since the last save.

### Important Configurations
Vitality will overwrite the health system, you need to change in configuration the base health system in ``levelstats/vitality.json`` changing in default game configuration will have no effects, if you don't want this feature consider disabling in configuration before generating joining the world

Debug logs is by default disabled, if you need to send logs you can send logs in [issues](https://github.com/LeandroTheDev/level_up/issues) case of errors and bugs, you can enable it in base.json: ``enableExtendedLog``, this also can cause some cpu performances problems in low end cpus.

# About Level UP
Level UP is open source project and can easily be accessed on the github, all contents from this mod is completly free.

If you want to contribute into the project you can access the project github and make your pull request.

You are free to fork the project and make your own version of Level UP, as long the name is changed.

Inspirations: 
- Valheim Level UP System
- Minecraft LevelZ mod

# Building
- Install .NET in your system, open terminal type: ``dotnet new install VintageStory.Mod.Templates``
- Create a template with the name ``LevelUP``: ``dotnet new vsmod --AddSolutionFile -o LevelUP``
- [Clone the repository](https://github.com/LeandroTheDev/level_up/archive/refs/heads/main.zip)
- Copy the ``CakeBuild`` and ``build.ps1`` or ``build.sh`` and paste inside the repository

Now you can build using the ``build.ps1`` or ``build.sh`` file

FTM License