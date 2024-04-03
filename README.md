# Level UP
Add a new mechanic to vintage story, a new level system to make your character strong and efficient during the gameplay, making your feels you are progressing through the gameplay.

Features:
- GUI Level in chacter view
- LEVELS:
- Hunter: Increases all damages sources to others creatures and players, earn xp by killing things
- Bow: Increases bow damages, earn xp by using and killing things with bows
- Cutlery: Increases knifes damages, earn by using and killing things with knifes
- Axe: Increases axes damages, earn by using and killing things with axes
- Pickaxe: Increase pickaxes damages, earn by using and killing things with pickaxes
- Shovel: Increase showvels damages, earn xp by using and killing things with shovels


### Building
Learn more about vintage story modding in [Linux](https://github.com/LeandroTheDev/arch_linux/wiki/Games#vintage-story-modding) or [Windows](https://wiki.vintagestory.at/index.php/Modding:Setting_up_your_Development_Environment)

> Not necessary but recomendend, if you dont do that you need to get the all the files in Releases/LevelUP/ and add into Mods folder from vintage story every time you build for test

Make a symbolic link for fast tests
- ln -s /path/to/project/Releases/levelup/LevelUP.dll /path/to/game/Mods/LevelUP/
- ln -s /path/to/project/Releases/levelup/LevelUP.pdb /path/to/game/Mods/LevelUP/
- ln -s /path/to/project/Releases/levelup/LevelUP.deps.json /path/to/game/Mods/LevelUP/
- ln -s /path/to/project/Releases/levelup/modinfo.json /path/to/game/Mods/LevelUP/

Execute the comamnd ./build.sh, consider having setup everthing from vintage story ide before

FTM License
