# Level UP
Add a new mechanic to vintage story, a new level system to make your character stronger and efficient during the gameplay, making your feels you are progressing through the gameplay.

Consider always making backup for your world, level up is a mod that changes a lot of stats and mechanics that might crash your world/player.

### IMPORTANT
This mod drastically changes the player status and its not recoverable in normal ways, PLEASE make a backup of your world, to reset players status you will need to use specific commands, please review the [Wiki-Commands](https://github.com/LeandroTheDev/level_up/wiki/Commands) to get more information to reset player status.

Features:
- Fully configurable
- GUI Level in character view
- Hardcore mode
- Commands for administration manipulation
- Classes and custom classes for manipulating the experience gain for each level type
- Survival Guide, for undestanding what each level will do
- Multi Language support

Not all item codes is added to the json configurations files, some armors/items/meats/entities/blocks can be missing, if you think something is missing from vanilla you can contact me or make a pull request in the github.

### Observations
This mod needs to be in both sides the client and server for working propertly, you can still build this only in server side, but some things will not work for example the mining speed mechanics will not work because the mining speed is handled by the client, also the level viewer will not be available for the clients.

To change the configurations go to the vintage story data, for windows in appdata for linux in .config for server is the path you set in serverconfig.json, find the folder ModConfig/LevelUP, if you want more informations you can see the [wiki](https://github.com/LeandroTheDev/level_up/wiki) to know what each configuration does, if any update from levelup add new configuration you will need to manually added them to fix logs errors, otherwises will have the default value.

Level UP Stores datas with player UID, changing the player name will persist all levels.

### Considerations
This mod changes a lot of native codes and can break easily throught updates.

Level UP doesn't register tickrates for the main game, so level up cannot be the cause for lower tickrate on servers.

Memory usage in the server can be slightly bigger because of static configurations and events on the server side, imperceptible for servers using more than 1gb ram

This mod might break other levels mods or be breaked.

All the mod calculations and patches is thinking in the server performance, feel free to suggest improviments if you find something getting slow.

Balancing was not very well thought out, you will probably find some things unusual or very strong, with everything you can make your changes in the configurations folder

### Important Configurations
Vitality will overwrite the health system, you need to change in configuration the base health system in ``levelstats/vitality.json`` changing in default game configuration will have no effects

Debug logs is by default disabled, if you need to send logs you can send logs in [issues](https://github.com/LeandroTheDev/level_up/issues) case of errors and bugs, you can enable it in base.json: ``enableExtendedLog``, this also can cause some cpu performances problems in low end cpus.

### About Level UP
Level UP is open source project and can easily be accessed on the github, all contents from this mod is completly free.

If you want to contribute into the project you can access the project github and make your pull request.

You are free to fork the project and make your own version of Level UP, as long the name is changed.

Inspirations: 
- Valheim Level UP System
- Minecraft LevelZ mod

### Building
Learn more about vintage story modding in [Linux](https://github.com/LeandroTheDev/arch_linux/wiki/Games#vintage-story-modding) or [Windows](https://wiki.vintagestory.at/index.php/Modding:Setting_up_your_Development_Environment)

Download the mod template for vintage store with name LevelUP and paste all contents from this project in there

> Linux

Make a symbolic link for fast tests
- ln -s /path/to/project/Releases/levelup/* /path/to/game/Mods/LevelUP/

Execute the comamnd ./build.sh, consider having setup everthing from vintage story ide before

> Windows

Just open the visual studio with LevelUP.sln

FTM License
