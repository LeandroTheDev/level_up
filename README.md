# Level UP
Add a new mechanic to vintage story, a new level system to make your character stronger and efficient during the gameplay, making your feels you are progressing through the gameplay.

Features:
- GUI Level in chacter view
- Each tools has a chance to not use durability by the level you have for the specific tool
- Fully configurable
- Hardcore mode (configurable)
- Commands for administration manipulation

LEVELS:
- Hunter: Increases all damages sources to others creatures and players, earn xp by killing things
- Bow: Increases bow damage, precision and reduce chance to lose arrows, earn xp by hitting and killing things with bows
- Knife: Increases knifes damages and harvest entity drops, earn xp by harvesting entities, hitting and killing things with knifes
- Spear: Increase spear damages and precision, earn xp by hitting and killing things with spear
- Axe: Increases axes damages and mining speed, earn xp by chopping trees, breaking wood, hitting and killing things with axes
- Pickaxe: Increase pickaxes damages, ore drops and mining speed, earn xp by breaking stones, ores, hitting and killing things with pickaxes
- Shovel: Increase shovels damages and mining speed, earn xp by breaking soil/gravel/sand, hitting and killing things with shovels
- Farming: Increase crop drop rate, earn xp by harvesting crops and till soils
- Vitality: Increase max health and health regen, earn xp by getting hitted

Future features:
- Smithing specialist.
- Cooking specialist.
- Better level view.
- Store data based on player uid.
- Chain Armor Armor levels
- Leather Armor Armor levels
- Cute level up sound

https://github.com/LeandroTheDev/level_up/assets/106118473/8409a3ee-08ce-42b6-8747-aa7bf6405a26

### Observations
This mod needs to be in both sides the client and server for working propertly, you can still build this only in server side, but some things will not work for example the mining speed mechanics will not work because the mining speed is handled by the client, also the level viewer will not be available for the clients.

Increase crop drop rate will only be increased if you harvest a crop with the final stage or the penultimate stage, the same for earning xp harvesting crops, harvesting crops in penultimate stage will decrease the experience gained, (default)

Hunter level damage is increased before the tool damage level, so if you have 2x damage in hunter and 2x damage in spear and you have a spear with 4 damage the calculation is: (4 x 2) x 2 = 16.

To change the configurations go to the mod folder in assets folders, if you want more informations you can see the [wiki](https://github.com/LeandroTheDev/levelup/wiki) to  know what each configuration does.

Level UP Stores datas based on player name, changing the player name will lose all levels.

### Considerations
This mod changes a lot of native codes and can break easily throught updates.

Performances leaks on server can be apparent because of new functions and calculation for damages and levels system,
this is imperceptible for average computers and servers, but can cause a good cpu consumption on low end computers with high player count.

Memory usage in the server can be slightly bigger because of static configurations and events on the server side, imperceptible for servers using more than 1gb ram

Changes in players stats like oreDropRate, animalLootDropRate is completly overwrited, so changing this will not actually give you more animal loots or ore drops, if you want to increase it consider changing in mod configuration the base ore drop and animal loots.

This mod might break other levels mods or be breaked.

All the mod calculations and patches is thinking in the server performance, feel free to suggest improviments if you find something getting slow.

### About Level UP
Level UP is open source project and can easily be accessed on the github, all contents from this mod is completly free.

If you want to contribute into the project you can access the project github and make your pull request.

You are free to fork the project and make your own version of Level UP, as long the name is changed.

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
