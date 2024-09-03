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

Increase crop drop rate will only be increased if you harvest a crop with the final stage or the penultimate stage, the same for earning xp harvesting crops, harvesting crops in penultimate stage will decrease the experience gained, (default)

Hunter level damage is increased before the tool damage level, so if you have 2x damage in hunter and 2x damage in spear and you have a spear with 4 damage the calculation is: (4 x 2) x 2 = 16.

Armors levels calculation, every level with that type of armor you gain more base damage reduction for example in level 5 you gain 1 damage reduction using leather armors, wearing more leather armors will increase this value, lets take any example wearing 3 pieces of leather armor: head(0.1) body(0.5) leg(0.3) multiply = 0.9, this means you will increase your damage reduction by 90%, so you will reduce a total of 1.9 damage.

Cooking experience and food status will be considered for the most nearest player from fire pit when the food finish cooking, the calculation for the servings increase is very simple, each level you earn more chance for additional servings, the roll is increased every 5 levels, for example in level 10 you have 3 rolls and 20% chance for increasing, if you are very lucky you can increase a max of 3 servings in level 10.

Shield calculation only increases the base damage absorved from the shield, working the same as the vanilla if the damage absorved is bigger than damage received the final damage is 0

To change the configurations go to the vintage story data, for windows in appdata for linux in .config for server is the path you set in serverconfig.json, find the folder ModConfig/LevelUP, if you want more informations you can see the [wiki](https://github.com/LeandroTheDev/level_up/wiki) to  know what each configuration does, if any update from levelup add new configuration you will need to manually added them to fix logs errors, otherwises will have the default value.

Level UP Stores datas with player UID, changing the player name will persist all levels.

Some people have issues with the drop rate of entities after removing LevelUP, for this theres is a command for administration uses, type /levelup resetplayerstatus PlayerName or PlayerUID, this command will reset all status and buffs from the player that can cause bugs on vanilla (Needs to be in all players that played in the moment the LevelUP is present).

Commands limitations, the command cannot receive player names with spaces, for that consider getting the player UID instead, for handling levels with space for example "Leather Armor" you need to remove the spaces: "LeatherArmor"

### Considerations
This mod changes a lot of native codes and can break easily throught updates.

Level UP doesn't register tickrates for the main game, so level up cannot be the cause for lower tickrate on servers.

Memory usage in the server can be slightly bigger because of static configurations and events on the server side, imperceptible for servers using more than 1gb ram

Changes in players stats like oreDropRate, animalLootDropRate is completly overwrited, so changing this will not actually give you more animal loots or ore drops, if you want to increase it consider changing in mod configuration the base ore drop and animal loots.

This mod might break other levels mods or be breaked.

All the mod calculations and patches is thinking in the server performance, feel free to suggest improviments if you find something getting slow.

Level UP is not compatible with mods that fully overwrite the ReceiveDamage function for Entity class and applyShieldProtection for ModSystemWearableStats class

Balancing was not very well thought out, you will probably find some things unusual or very strong, with everything you can make your changes in the configurations folder

The configuration enableExtended logs can cause performances problems, because a lot of things in the mod is logged out, if the mod is very stable in your world/modpack please consider desabling it in configurations

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
