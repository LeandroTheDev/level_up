# Level Stats
levelstats folders stores all others data about the level system, and for each level have specific configurations to be set, in this example consider changing the ``level`` to the level type you want to chance for example => ``levelEXPPerHit`` = ``axeEXPPerHit``

- levelEXPPerHit => experience earned by hitting any entity with that tool
- levelEXPPerBreaking => experience earned by breaking things with that tool
- levelEXPPerTreeBreaking => experience earned by chopping things with that tool
- levelEXPPerLevelBase => the base experience calculation for levels
- levelEXPMultiplyPerLevel => each level the player earns is more difficulty for getting new levels, this is the modifier for that difficulty
- levelBaseDamage => base damage for that type of tool
- levelIncrementDamagePerLevel => the percentage increasing the damage of that tool each level the player have
- levelBaseMiningSpeed => base mining speed for that tool, only applies in the blocks that tools breaks
- levelIncrementMiningSpeedMultiplyPerLevel => the percentage increasing the mining speed for that tool
- levelBaseChanceToNotLoseArrow => base chance to not losing the arrow when shotting to entity
- levelChanceToNotLoseArrowBaseIncreasePerLevel => how much chance you will earn per level to not lose the arrow when shotting
- levelChanceToNotLoseArrowReduceIncreaseEveryLevel => each level that will reduce the chance earned by levels, for example: until level 5 you earn 2.0, but after level 5 you are earning only 1.5 chances.
- levelChanceToNotLoseArrowReduceQuantityEveryLevel => the actual chance that will be reduced after levelChanceToNotLoseArrowReduceIncreaseEveryLevel
- levelBaseAimAccuracy => base aim accuracy for shotting or throwing with the tool
- levelIncreaseAimAccuracyPerLevel => accuracy gained each level the player has with the tool
- levelEXPPerTill => experience gained by tilling the soil
- levelBaseHarvestMultiply => base amount of drops will be get after harvesting things of the level type
- levelIncrementHarvestMultiplyPerLevel => each level will increase this amount of the harvest drop rate
- levelBaseOreMultiply => increase the ore drop rate by breaking with the tool
- levelIncrementOreMultiplyPerLevel => each level will increase this amount of the ore drop rate
- levelEXPMultiplyByDamage => based on the damage will multiply the xp every levelEXPIncreaseByAmountDamage
- levelEXPIncreaseByAmountDamage => every this damage will increase the experience gained
- levelBaseHP => base character health
- levelBaseHPRegen => base character health regeneration
- levelHPIncreasePerLevel => every level increase this amount of health
- levelHPRegenIncreasePerLevel => every level increase this amount of health regeneration
- levelBaseSmithRetrieveChance => chance to obtain the nugget again when splitting
- levelSmithRetrieveChancePerLevel => increase the chance to obtain the nugget when splitting every level
- levelSmithRetrieveEveryLevelReduceChance => every this level will reduce the chance increaser from levelSmithRetrieveChancePerLevel, making it more difficulty over levels
- levelSmithRetrieveReduceChanceForEveryLevel => the amount of chance that will be reduced in levelSmithRetrieveEveryLevelReduceChance
- levelBaseChanceToDouble => base chance for duplicating incomings results
- levelIncreaseChanceToDoublePerLevel => the base amount for increasing the levelBaseChanceToDouble every level
- levelIncreaseChanceToDoublePerLevelReducerPerLevel => every this level will reduce the levelIncreaseChanceToDoublePerLevel amount
- levelIncreaseChanceToDoublePerLevelReducer => actual quantity to reduce for every levelIncreaseChanceToDoublePerLevelReducerPerLevel
- levelArmorEXPPerReceiveHit => the base experience earned every damage received
- levelArmorEXPIncreaseByAmountDamage => multiply levelArmorEXPPerReceiveHit earned for every this amount of damage received

Armor damage reduction is no longer a single generic pair of keys, each armor type (Leather, Chain, Brigandine, Lamellar, Plate, Scale) has its own two independent groups of 4 keys, prefixed by the armor type instead of ``level``:
- levelRelativeProtectionMultiply / levelRelativeProtectionMultiplyPerLevel / levelRelativeProtectionMultiplyReductionEveryLevel / levelRelativeProtectionMultiplyReductionPerReduce => base relative protection multiplier, how much it increases per level, and the difficulty reduction of that increase over levels (same pattern as the chance/reduction keys above)
- levelFlatDamageReductionMultiply / levelFlatDamageReductionMultiplyPerLevel / levelFlatDamageReductionMultiplyReductionEveryLevel / levelFlatDamageReductionMultiplyReductionPerReduce => same as above but for the flat damage reduction multiplier

Shield also does not use a generic reduction pair, it has 6 independent base/per-level pairs, prefixed with ``shield`` instead of ``level``:
- shieldBasePassiveProjectile / shieldPassiveProjectilePerLevel
- shieldBaseActiveProjectile / shieldActiveProjectilePerLevel
- shieldBasePassive / shieldPassivePerLevel
- shieldBaseActive / shieldActivePerLevel
- shieldBaseProjectileDamageAbsorption / shieldProjectileDamageAbsorptionPerLevel
- shieldBaseDamageAbsorption / shieldDamageAbsorptionPerLevel
- levelBaseFreshHoursMultiply => base hours for rooting the food
- levelFreshHoursMultiplyPerLevel => how much the fresh hours will be increased every level for each cooking
- levelBaseChanceToIncreaseServings => base chance to increase the servings for cooking pots
- levelIncrementChanceToIncreaseServings => every level will increment this chance to earn any additional servings
- levelEveryChanceToIncreaseServingsReduceChance => every this amount of actual chance will decrease the chance earned by levelIncrementChanceToIncreaseServings
- levelChanceToIncreaseServingsReducerTotal => the actual chance to reduce every levelEveryChanceToIncreaseServingsReduceChance
- levelBaseRollsChanceToIncreaseServings => base max chance to increase a servings, 1 means you can increase 1 total servings
- levelEarnRollsChanceToIncreaseServingsEveryLevel => every this level will increase the max servings by levelEarnRollsChanceToIncreaseServingsQuantity
- levelEarnRollsChanceToIncreaseServingsQuantity => the actual quantity to increase every levelEarnRollsChanceToIncreaseServingsEveryLevel 

### Farming crops
farmingcrops.json stores the crops that you will earn experience by breaking, if you add a block code id you will earn farming experience by breaking it.

### Pickaxes ores
pickaxesores.json stores all the ores that you will earn experience by breaking

### Cooking
cookingpots.json stores all cooked pots ids and the experience to multiply, cookingsingles.json stores all the single foods and the experience to multiply

Cooking pots and single cookings works in differents ways so is necessary to not add cookings singles in cooking pots and cooking pots in cooking singles.

### Armors
Each armor type has its own ``{type}armoritems.json`` file storing which items count as that armor type and the resistance multiply: leatherarmoritems.json, chainarmoritems.json, brigandinearmoritems.json, lamellararmoritems.json, platearmoritems.json, scalearmoritems.json.

### Hammers
hammersmiths.json stores the ingots that will retrieve the nugget when splitting the ingot with the hammer

### Smithing
smithingcrafts.json stores the items code to earn smithing experience when crafting

# Entity EXP
entityexp folder stores all tools levels that the player will earn by killing entity with the tool, for example the axe.json file stores all entities and experience that axe will earn by killing that entities

# Base
base.json is the main configuration for the level up, here you can change a wider range of settings
- enableHardcore => enable or disable losing experience when the player dies
- hardcoreLosePercentage => the percentage of the current level progress that is actually LOST, for example you have 100 experience towards the next level and you want to lose only 20% of it, use ``0.2``, to lose 50% use ``0.5``, to lose 80% use ``0.8`` (default is ``0.8``)
- hardcorePenaltyDelayInWorldSeconds -> Amount of seconds (ingame seconds) to the player not lose experience when dying again
- hardcoreIgnoreLevelMinimum -> if false the exp lost is calculated by the current level to the previous one, if true the calculation is all the experience
- hardcoreMessageWhenDying -> Should appear a death message indicating the experience lost when die?
- enableLevel... => enable or disable the functionalities for that level, e.g. ``enableLevelMetabolism`` toggles the Metabolism level
- enableLevelUPUIDSecurity => extra security check validating the player UID length, disabled by default
- minimumEXPEarned => this is the minimum experience a class can earn from any action
- enableLevelUpChatMessages => when the player level up he receives a message in the chat indicating the level, false for disabling
- enableLevelUpExperienceServerLog => all experience earned by the players will be logged in server terminal or server-main.txt in logs folder
- enableExtendedLog => disable or enable the extended logs, used when you find some bug and you need to post some [issue](https://github.com/LeandroTheDev/level_up/issues)

# Class EXP
Stores all the classes from the game, with this folder you can manipulate how the class will receive experience in levelup, by default comes with the vanilla classes, you can create classes by copying one and changing the name to the new class id
