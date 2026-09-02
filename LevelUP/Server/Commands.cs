#pragma warning disable CA1822
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LevelUP.Server;

class Commands
{
    public void Init()
    {
        // Create register command
        Instance.api.ChatCommands.Create("levelup")
        // Description
        .WithDescription("Manipulate level UP")
        // Chat privilege
        .RequiresPrivilege(Privilege.root)
        // Need a argument called password
        .WithArgs(new StringArgParser("arguments", false))
        // Function Handle
        .HandleWith(HandleCommands);
    }

    private TextCommandResult HandleCommands(TextCommandCallingArgs args)
    {
        if (args.Parsers[0].IsMissing) return TextCommandResult.Success("No arguments", "0");
        string[] arguments = args[0].ToString().Split(" ");
        if (arguments.Length == 0) return TextCommandResult.Success("No arguments", "0");
        // Get the handler
        string handler = arguments[0];
        // Handle the command
        return handler switch
        {
            "changeexperience" => ChangeExperience(arguments),
            "addexperience" => AddExperience(arguments),
            "reduceexperience" => ReduceExperience(arguments),
            "resetplayerstatus" => ResetPlayerStatus(arguments),
            "resetplayerlevels" => ResetPlayerLevels(arguments),
            _ => TextCommandResult.Success($"Invalid command {handler}", "1"),
        };
    }

    private TextCommandResult ChangeExperience(string[] args)
    {
        //args:
        //1 => LevelType
        //2 => PlayerName to be changed (or proficiency if 5 args)
        //3 => Experience quantity to change (or PlayerName if 5 args)
        //--- optional (proficiency form) ---
        //2 => Proficiency
        //3 => PlayerName to be changed
        //4 => Experience quantity to change
        if (args.Length != 4 && args.Length != 5) return TextCommandResult.Success($"Invalid arguments", "2");

        if (args.Length == 5)
        {
            // Proficiency form: changeexperience LevelType Proficiency PlayerName Amount
            if (!long.TryParse(args[4], out _)) return TextCommandResult.Success($"Invalid experience value, use only decimal numbers", "3");

            IServerPlayer player = GetPlayerByUsernameOrUID(args[3]);
            if (player == null) return TextCommandResult.Success($"Player {args[3]} not found or not online", "14");

            Experience.ChangeSubExperience(player, args[1], args[2], ulong.Parse(args[4]));

            Instance.UpdatePlayerLevels(player, Instance.api);
            Instance.RefreshStatus(player, args[1]);

            return TextCommandResult.Success($"Changed experience from {player.PlayerName} to {args[4]} on level {args[1]}/{args[2]}", "10");
        }

        // Normal form: changeexperience LevelType PlayerName Amount
        if (!long.TryParse(args[3], out _)) return TextCommandResult.Success($"Invalid experience value, use only decimal numbers", "3");

        IServerPlayer normalPlayer = GetPlayerByUsernameOrUID(args[2]);
        if (normalPlayer == null) return TextCommandResult.Success($"Player {args[2]} not found or not online", "14");

        // Update player levels
        Experience.ChangeExperience(normalPlayer, args[1], ulong.Parse(args[3]));

        // Refresh player levels
        Instance.UpdatePlayerLevels(normalPlayer, Instance.api);
        Instance.RefreshStatus(normalPlayer, args[1]);

        return TextCommandResult.Success($"Changed experience from {normalPlayer.PlayerName} to {args[3]} on level {args[1]}", "10");
    }

    private TextCommandResult AddExperience(string[] args)
    {
        //args:
        //1 => LevelType
        //2 => PlayerName to be changed
        //3 => Experience quantity to change
        if (args.Length != 4) return TextCommandResult.Success($"Invalid arguments", "2");

        // Check if experience is a valid decimal number
        if (!long.TryParse(args[3], out _)) return TextCommandResult.Success($"Invalid experience value, use only decimal numbers", "3");

        IServerPlayer player = GetPlayerByUsernameOrUID(args[2]);
        if (player == null) return TextCommandResult.Success($"Player {args[2]} not found or not online", "14");

        // Incrementing player experience
        Experience.IncreaseExperience(player, args[1], ulong.Parse(args[3]));

        // Refresh player levels
        Instance.UpdatePlayerLevels(player, Instance.api);
        Instance.RefreshStatus(player, args[1]);

        return TextCommandResult.Success($"Added {args[3]} experience to {player.PlayerName} on level {args[1]}", "11");
    }

    private TextCommandResult ReduceExperience(string[] args)
    {
        //args:
        //1 => LevelType
        //2 => PlayerName to be changed
        //3 => Experience quantity to change
        if (args.Length != 4) return TextCommandResult.Success($"Invalid arguments", "2");

        // Check if experience is a valid decimal number
        if (!long.TryParse(args[3], out _)) return TextCommandResult.Success($"Invalid experience value, use only decimal numbers", "3");

        IServerPlayer player = GetPlayerByUsernameOrUID(args[2]);
        if (player == null) return TextCommandResult.Success($"Player {args[2]} not found or not online", "14");

        // Reducing the player experience
        Experience.ReduceExperience(player, args[1], ulong.Parse(args[3]), true);

        // Refresh player levels
        Instance.UpdatePlayerLevels(player, Instance.api);
        Instance.RefreshStatus(player, args[1]);

        return TextCommandResult.Success($"Reduced {args[3]} experience to {player.PlayerName} on level {args[1]}", "12");
    }

    private TextCommandResult ResetPlayerStatus(string[] args)
    {
        //args:
        //1 => playerName to reset
        //2 => Optional: stats type
        //3 => Optional: quantity

        // To much arguments
        if (args.Length <= 1 || args.Length > 4) return TextCommandResult.Success($"Invalid arguments", "2");

        // Check if value is a valid decimal number
        if (args.Length > 3 && !float.TryParse(args[3], out _)) return TextCommandResult.Success($"Invalid quantity value, use only float numbers", "15");

        IServerPlayer player = GetPlayerByUsernameOrUID(args[1]);
        if (player == null) return TextCommandResult.Success($"Player {args[1]} not found or not online", "14");

        // Specific status
        if (args.Length == 3)
        {
            switch (args[2])
            {
                case "animalLootDropRate": ResetLevelUpStat(player, "animalLootDropRate", ["levelup_knife"]); break;
                case "aimingAccuracy": player.Entity.Attributes.RemoveAttribute("aimingAccuracy"); break;
                case "regenSpeed": ResetLevelUpStat(player, "regenSpeed", ["levelup_vitality"]); break;
                case "rangedWeaponsAcc": ResetLevelUpStat(player, "rangedWeaponsAcc", ["levelup_bow", "levelup_spear"]); break;
                case "rangedWeaponsSpeed": ResetLevelUpStat(player, "rangedWeaponsSpeed", ["levelup_bow", "levelup_spear"]); break;
                default: return TextCommandResult.Success($"Invalid status", "16");
            }
            return TextCommandResult.Success($"{args[1]} {args[2]} has been reseted to vanilla default", "17");
        }
        // Specific status + specific quantity
        else if (args.Length > 3)
        {
            switch (args[2])
            {
                case "animalLootDropRate": ResetLevelUpStat(player, "animalLootDropRate", ["levelup_knife"], UtilsCulture.ParseFloatCulturized(args[3])); break;
                case "aimingAccuracy": player.Entity.Attributes.SetFloat("aimingAccuracy", UtilsCulture.ParseFloatCulturized(args[3])); break;
                case "regenSpeed": ResetLevelUpStat(player, "regenSpeed", ["levelup_vitality"], UtilsCulture.ParseFloatCulturized(args[3])); break;
                case "rangedWeaponsAcc": ResetLevelUpStat(player, "rangedWeaponsAcc", ["levelup_bow", "levelup_spear"], UtilsCulture.ParseFloatCulturized(args[3])); break;
                case "rangedWeaponsSpeed": ResetLevelUpStat(player, "rangedWeaponsSpeed", ["levelup_bow", "levelup_spear"], UtilsCulture.ParseFloatCulturized(args[3])); break;
                default: return TextCommandResult.Success($"Invalid status", "16");
            }
            return TextCommandResult.Success($"{args[1]} {args[2]} has been reseted to {args[3]}", "18");
        }

        // Nothing specific change everthing to default value
        ResetLevelUpStat(player, "animalLootDropRate", ["levelup_knife"]);
        player.Entity.Attributes.RemoveAttribute("aimingAccuracy");
        ResetLevelUpStat(player, "regenSpeed", ["levelup_vitality"]);
        ResetLevelUpStat(player, "rangedWeaponsAcc", ["levelup_bow", "levelup_spear"]);
        ResetLevelUpStat(player, "rangedWeaponsSpeed", ["levelup_bow", "levelup_spear"]);

        // Refresh player levels
        Instance.UpdatePlayerLevels(player, Instance.api);
        Instance.RefreshStatus(player, args[1]);

        return TextCommandResult.Success($"{args[1]} status has been reseted to vanilla default", "13");
    }

    // Every LevelUP source contributes to its stat category under its own "levelup_<levelname>" code
    // (e.g. LevelBow/LevelSpear both feed rangedWeaponsAcc, LevelKnife feeds animalLootDropRate), so
    // Set-ing the category name itself wouldn't clear those - it would just add an extra entry on top.
    // Remove every known contributor code, then optionally pin an override value on top.
    private static void ResetLevelUpStat(IServerPlayer player, string category, string[] codes, float? overrideValue = null)
    {
        foreach (string code in codes)
            player.Entity.Stats.Remove(category, code);

        if (overrideValue.HasValue)
            player.Entity.Stats.Set(category, category, overrideValue.Value);
    }

    private TextCommandResult ResetPlayerLevels(string[] args)
    {
        //args:
        //1 => playerName to reset

        // To much arguments
        if (args.Length <= 1 || args.Length > 2) return TextCommandResult.Success($"Invalid arguments", "2");

        IServerPlayer player = GetPlayerByUsernameOrUID(args[1]);
        if (player == null) return TextCommandResult.Success($"Player {args[1]} not found or not online", "14");

        // Removing experience from all levels to this player
        Instance.ResetPlayerLevels(player, Instance.api, 0);

        // Refresh player levels
        Instance.UpdatePlayerLevels(player, Instance.api);
        Instance.RefreshStatus(player, args[1]);

        return TextCommandResult.Success($"{args[1]} levels has been reseted to 0", "13");
    }


    private IServerPlayer GetPlayerByUsernameOrUID(string usernameOrUID)
    {
        foreach (IPlayer player in Instance.api.World.AllOnlinePlayers)
        {
            if (player.PlayerName == usernameOrUID || player.PlayerUID == usernameOrUID) return player as IServerPlayer;
        }
        return null;
    }
}