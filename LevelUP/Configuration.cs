namespace LevelUP;

public static class Configuration {
    public static int HunterGetLevelByEXP(int exp){
        // No exp = level 1
        if(exp == 0) return 1;
        
        int level = 0;
        // Exp base for level
        double expPerLevelBase = 10;
        double calcExp = double.Parse(exp.ToString());
        while(calcExp > 0){
            level += 1;
            calcExp -= expPerLevelBase;
            // 10 percentage increasing per level
            expPerLevelBase *= 1.1;
        }
        return level;
    }

    public static float HunterGetDamageMultiplyByEXP(int exp) {
        // float baseDamage = 1.0f;
        // float incrementDamage = 0.1f;

        return 0.1f;
    }
}