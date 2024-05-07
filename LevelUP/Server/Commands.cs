using System;
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

        // Update experience
        if (levels.TryGetValue(args[2], out ulong _)) levels[args[2]] = ulong.Parse(args[3]);
        else levels[args[2]] = ulong.Parse(args[3]);

        // Save it
        instance.api.WorldManager.SaveGame.StoreData($"LevelUPData_{args[1]}", JsonSerializer.Serialize(levels));

        return TextCommandResult.Success($"Changed experience from {args[2]} to {args[3]} on level {args[1]}", "10");
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

        // Update experience
        if (levels.TryGetValue(args[2], out ulong _)) levels[args[2]] += ulong.Parse(args[3]);
        else levels[args[2]] = ulong.Parse(args[3]);

        // Save it
        instance.api.WorldManager.SaveGame.StoreData($"LevelUPData_{args[1]}", JsonSerializer.Serialize(levels));

        return TextCommandResult.Success($"Added {args[2]} experience to {args[3]} on level {args[1]}", "11");
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

        // Update experience
        if (levels.TryGetValue(args[2], out ulong _)) levels[args[2]] -= ulong.Parse(args[3]);
        else levels[args[2]] = 0;

        // Negative experience treatment
        if (levels[args[2]] < 0) levels[args[2]] = 0;

        // Save it
        instance.api.WorldManager.SaveGame.StoreData($"LevelUPData_{args[1]}", JsonSerializer.Serialize(levels));

        return TextCommandResult.Success($"Reduced {args[2]} experience to {args[3]} on level {args[1]}", "12");
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

        IServerPlayer playerToBeReseted = null;
        // Getting the player instance
        instance.api.World.AllPlayers.Foreach((player) =>
        {
            if (player.PlayerName == args[1]) playerToBeReseted = player as IServerPlayer;
        });

        // Check if the player is online
        if (playerToBeReseted == null) return TextCommandResult.Success($"Player {args[0]} not found or not online", "14");

        // Specific status
        if (args.Length == 3)
        {
            switch (args[2])
            {
                case "oreDropRate": playerToBeReseted.Entity.Stats.Set("oreDropRate", "oreDropRate", 0.5f); break;
                case "animalLootDropRate": playerToBeReseted.Entity.Stats.Set("animalLootDropRate", "animalLootDropRate", 0.5f); break;
                case "aimingAccuracy": playerToBeReseted.Entity.Attributes.SetFloat("aimingAccuracy", 0.7f); break;
                default: return TextCommandResult.Success($"Invalid status", "16");
            }
            return TextCommandResult.Success($"{args[1]} {args[2]} has been reseted to vanilla default", "17");
        }
        // Specific status + specific quantity
        else if (args.Length > 3)
        {
            switch (args[2])
            {
                case "oreDropRate": playerToBeReseted.Entity.Stats.Set("oreDropRate", "oreDropRate", float.Parse(args[3])); break;
                case "animalLootDropRate": playerToBeReseted.Entity.Stats.Set("animalLootDropRate", "animalLootDropRate", float.Parse(args[3])); break;
                case "aimingAccuracy": playerToBeReseted.Entity.Attributes.SetFloat("aimingAccuracy", float.Parse(args[3])); break;
                default: return TextCommandResult.Success($"Invalid status", "16");
            }
            return TextCommandResult.Success($"{args[1]} {args[2]} has been reseted to {args[3]}", "18");
        }

        // Nothing specific change everthing to default value
        playerToBeReseted.Entity.Stats.Set("oreDropRate", "oreDropRate", 0.5f);
        playerToBeReseted.Entity.Stats.Set("animalLootDropRate", "animalLootDropRate", 0.5f);
        playerToBeReseted.Entity.Attributes.SetFloat("aimingAccuracy", 0.7f);
        return TextCommandResult.Success($"{args[1]} status has been reseted to vanilla default", "13");
    }
}