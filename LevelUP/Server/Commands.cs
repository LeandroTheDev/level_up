using System.Collections.Generic;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace LevelUP.Server;

class Commands
{
    Instance instance;
    public void Init(Instance _instance)
    {
        instance = _instance;
        // Create register command
        instance.api.ChatCommands.Create("levelup")
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
            _ => TextCommandResult.Success($"Invalid command {handler}", "1"),
        };
    }

    private TextCommandResult ChangeExperience(string[] args)
    {
        //args:
        //1 => LevelType
        //2 => PlayerName to be changed
        //3 => Experience quantity to change
        if (args.Length != 4) return TextCommandResult.Success($"Invalid arguments", "2");

        // Check if experience is a valid decimal number
        if (!long.TryParse(args[3], out _)) return TextCommandResult.Success($"Invalid experience value, use only decimal numbers", "3");

        // Get levels
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData($"LevelUPData_{args[1]}");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

        IServerPlayer player = GetPlayerByUsernameOrUID(args[2]);
        if (player == null) return TextCommandResult.Success($"Player {args[2]} not found or not online", "14");

        // Update experience
        if (levels.TryGetValue(player.PlayerUID, out ulong _)) levels[player.PlayerUID] = ulong.Parse(args[3]);
        else levels[player.PlayerUID] = ulong.Parse(args[3]);

        // Save it
        instance.api.WorldManager.SaveGame.StoreData($"LevelUPData_{args[1]}", JsonSerializer.Serialize(levels));

        return TextCommandResult.Success($"Changed experience from {player.PlayerName} to {args[3]} on level {args[1]}", "10");
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

        // Get levels
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData($"LevelUPData_{args[1]}");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

        IServerPlayer player = GetPlayerByUsernameOrUID(args[2]);
        if (player == null) return TextCommandResult.Success($"Player {args[2]} not found or not online", "14");

        // Update experience
        if (levels.TryGetValue(player.PlayerUID, out ulong _)) levels[player.PlayerUID] += ulong.Parse(args[3]);
        else levels[player.PlayerUID] = ulong.Parse(args[3]);

        // Save it
        instance.api.WorldManager.SaveGame.StoreData($"LevelUPData_{args[1]}", JsonSerializer.Serialize(levels));

        return TextCommandResult.Success($"Added {player.PlayerName} experience to {args[3]} on level {args[1]}", "11");
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

        // Get levels
        byte[] dataBytes = instance.api.WorldManager.SaveGame.GetData($"LevelUPData_{args[1]}");
        string data = dataBytes == null ? "{}" : SerializerUtil.Deserialize<string>(dataBytes);
        Dictionary<string, ulong> levels = JsonSerializer.Deserialize<Dictionary<string, ulong>>(data);

        IServerPlayer player = GetPlayerByUsernameOrUID(args[2]);
        if (player == null) return TextCommandResult.Success($"Player {args[2]} not found or not online", "14");

        // Update experience
        if (levels.TryGetValue(player.PlayerUID, out ulong _)) levels[player.PlayerUID] -= ulong.Parse(args[3]);
        else levels[player.PlayerUID] = 0;

        // Negative experience treatment
        if (levels[player.PlayerUID] < 0) levels[player.PlayerUID] = 0;

        // Save it
        instance.api.WorldManager.SaveGame.StoreData($"LevelUPData_{args[1]}", JsonSerializer.Serialize(levels));

        return TextCommandResult.Success($"Reduced {player.PlayerName} experience to {args[3]} on level {args[1]}", "12");
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
                case "oreDropRate": player.Entity.Stats.Set("oreDropRate", "oreDropRate", 0.5f); break;
                case "animalLootDropRate": player.Entity.Stats.Set("animalLootDropRate", "animalLootDropRate", 0.5f); break;
                case "aimingAccuracy": player.Entity.Attributes.SetFloat("aimingAccuracy", 0.7f); break;
                case "forageDropRate": player.Entity.Stats.Set("forageDropRate", "forageDropRate", 1.0f); break;
                case "regenSpeed": player.Entity.WatchedAttributes.SetFloat("regenSpeed", 1.0f); break;
                default: return TextCommandResult.Success($"Invalid status", "16");
            }
            return TextCommandResult.Success($"{args[1]} {args[2]} has been reseted to vanilla default", "17");
        }
        // Specific status + specific quantity
        else if (args.Length > 3)
        {
            switch (args[2])
            {
                case "oreDropRate": player.Entity.Stats.Set("oreDropRate", "oreDropRate", float.Parse(args[3])); break;
                case "animalLootDropRate": player.Entity.Stats.Set("animalLootDropRate", "animalLootDropRate", float.Parse(args[3])); break;
                case "aimingAccuracy": player.Entity.Attributes.SetFloat("aimingAccuracy", float.Parse(args[3])); break;
                case "forageDropRate": player.Entity.Stats.Set("forageDropRate", "forageDropRate", float.Parse(args[3])); break;
                case "regenSpeed": player.Entity.WatchedAttributes.SetFloat("regenSpeed", float.Parse(args[3])); break;
                default: return TextCommandResult.Success($"Invalid status", "16");
            }
            return TextCommandResult.Success($"{args[1]} {args[2]} has been reseted to {args[3]}", "18");
        }

        // Nothing specific change everthing to default value
        player.Entity.Stats.Set("oreDropRate", "oreDropRate", 0.5f);
        player.Entity.Stats.Set("animalLootDropRate", "animalLootDropRate", 0.5f);
        player.Entity.Attributes.SetFloat("aimingAccuracy", 0.7f);
        player.Entity.Stats.Set("forageDropRate", "forageDropRate", 1.0f);
        player.Entity.WatchedAttributes.SetFloat("regenSpeed", 1.0f);
        return TextCommandResult.Success($"{args[1]} status has been reseted to vanilla default", "13");
    }

    private IServerPlayer GetPlayerByUsernameOrUID(string usernameOrUID)
    {
        foreach (IPlayer player in instance.api.World.AllOnlinePlayers)
        {
            if (player.PlayerName == usernameOrUID || player.PlayerUID == usernameOrUID) return player as IServerPlayer;
        }
        return null;
    }
}