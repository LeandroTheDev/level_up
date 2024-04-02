# Level UP
Add a new mechanic to vintage story, a new level system to make your character strong and efficient during the gameplay, making your feels you are progressing through the gameplay, this mod is made thinking in the security for servers.

Features:
- GUI Level in chacter view

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
