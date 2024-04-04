# Level UP
Add a new mechanic to vintage story, a new level system to make your character strong and efficient during the gameplay, making your feels you are progressing through the gameplay.

Features:
- GUI Level in chacter view
- LEVELS:
- Hunter: Increases all damages sources to others creatures and players, earn xp by killing things
- Bow: Increases bow damages, earn xp by hitting and killing things with bows
- Cutlery: Increases knifes damages, earn xp by hitting and killing things with knifes
- Spear: Increase spear damages, earn xp by hitting and killing things with spear
- Axe: Increases axes damages and mining speed, earn xp by breaking wood, hitting and killing things with axes
- Pickaxe: Increase pickaxes damages and mining speed, earn xp by breaking stones, hitting and killing things with pickaxes
- Shovel: Increase shovels damages and mining speed, earn xp by breaking soil/gravel/sand, hitting and killing things with shovels

![image](https://github.com/LeandroTheDev/level_up/assets/106118473/27de8daf-7b07-464d-9d3b-a108c40bac78)

![image](https://github.com/LeandroTheDev/level_up/assets/106118473/395a7d12-62e8-4a88-9faa-797d3a561826)


### Building
Learn more about vintage story modding in [Linux](https://github.com/LeandroTheDev/arch_linux/wiki/Games#vintage-story-modding) or [Windows](https://wiki.vintagestory.at/index.php/Modding:Setting_up_your_Development_Environment)

> Not necessary but recomendend, if you dont do that you need to get the all the files in Releases/LevelUP/ and add into Mods folder from vintage story every time you build.

Make a symbolic link for fast tests
- ln -s /path/to/project/Releases/levelup/LevelUP.dll /path/to/game/Mods/LevelUP/
- ln -s /path/to/project/Releases/levelup/LevelUP.pdb /path/to/game/Mods/LevelUP/
- ln -s /path/to/project/Releases/levelup/LevelUP.deps.json /path/to/game/Mods/LevelUP/
- ln -s /path/to/project/Releases/levelup/modinfo.json /path/to/game/Mods/LevelUP/

Execute the comamnd ./build.sh, consider having setup everthing from vintage story ide before

FTM License
