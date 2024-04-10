# Level UP
Add a new mechanic to vintage story, a new level system to make your character strong and efficient during the gameplay, making your feels you are progressing through the gameplay.

Features:
- GUI Level in chacter view
- LEVELS:
- Hunter: Increases all damages sources to others creatures and players, earn xp by killing things
- Bow: Increases bow damages, earn xp by hitting and killing things with bows
- Cutlery: Increases knifes damages and harvest entity drops, earn xp by harvesting entities, hitting and killing things with knifes
- Spear: Increase spear damages, earn xp by hitting and killing things with spear
- Axe: Increases axes damages and mining speed, earn xp by chopping trees, breaking wood, hitting and killing things with axes
- Pickaxe: Increase pickaxes damages, ore drops and mining speed, earn xp by breaking stones, hitting and killing things with pickaxes
- Shovel: Increase shovels damages and mining speed, earn xp by breaking soil/gravel/sand, hitting and killing things with shovels
- Farming: Increase crop drop rate, earn xp by harvesting crops grown for 3 days or more and till soils
- Cooking: Increase expiration days and servings quantity, earn xp by cooking

![image](https://github.com/LeandroTheDev/level_up/assets/106118473/27de8daf-7b07-464d-9d3b-a108c40bac78)

![image](https://github.com/LeandroTheDev/level_up/assets/106118473/395a7d12-62e8-4a88-9faa-797d3a561826)

### Observations
Integer limit, this mods saves the experiencie from the player as int, and C# integer limit is beyond the 2 billions, so if the player exp is reaching this number is quitly dangerous what would happen.

The nearest player in the cooking fire pit will receive the cooking experience when finish, if no players where found you will lose the experience.

This mod needs to be in both sides the client and server for working propertly, you can still build this only in server side, but some things will not work for example the mining speed mechanics will not work because the mining speed is handled by the client.

### Considerations
This mod changes a lot of native codes and can break easily throught updates.

Performances leaks can be apparent because of damage increase, drops rates system and level calculation, 
memory usage in the server can be slightly bigger because of static configurations.

### Building
Learn more about vintage story modding in [Linux](https://github.com/LeandroTheDev/arch_linux/wiki/Games#vintage-story-modding) or [Windows](https://wiki.vintagestory.at/index.php/Modding:Setting_up_your_Development_Environment)

Download the mod template for vintage store with name LevelUP and paste all contents from this project in there

> Linux

> Not necessary but recomendend, if you dont do that you need to get the all the files in Releases/LevelUP/ and add into Mods folder from vintage story every time you build.

Make a symbolic link for fast tests
- ln -s /path/to/project/Releases/levelup/* /path/to/game/Mods/LevelUP/

Execute the comamnd ./build.sh, consider having setup everthing from vintage story ide before

> Windows

Just open the visual studio with LevelUP.sln

FTM License
