using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LevelUP;
using LevelUP.Server;
using Vintagestory.API.Common;

partial class Utils
{
    // Non authenticated server can let players use whatever ID he wants
    // if a player set their UID as "../../../../Whatever" he can save their experience
    // outside the folder, so in that case we will stop that using this validator
    // that will throw any exception and disconnect the player
    static public bool ValidatePlayerUID(IPlayer player)
    {
        if (Instance.api != null && Configuration.enableLevelUPUIDSecurity)
        {
            if (player.PlayerUID.Length > 24)
            {
                Debug.LogError($"[LEVELUP SECURITY] player.PlayerUID invalid, UID is too big: {player.PlayerUID}, levelup is bloking the user {player.PlayerName} from saving or loading experience, you are probably using a dedicated server with authentication disabled, and the player is using any invalid UID");
                return false;
            }

            if (player.PlayerUID.Split('/').Length - 1 > 1)
            {
                Debug.LogError($"[LEVELUP SECURITY] player.PlayerUID invalid, more than one bar: {player.PlayerUID}, levelup is bloking the user {player.PlayerName} from saving or loading experience, you are probably using a dedicated server with authentication disabled, and the player is using any invalid UID");
                return false;
            }

            if (!NumbersAndLetters().IsMatch(player.PlayerUID) || player.PlayerUID.Contains('.'))
            {
                Debug.LogError($"[LEVELUP SECURITY] player.PlayerUID contains invalid characters: {player.PlayerUID}, levelup is bloking the user {player.PlayerName} from saving or loading experience, you are probably using a dedicated server with authentication disabled, and the player is using any invalid UID");
                return false;
            }
        }
        return true;
    }

    static readonly Dictionary<char, string> map = new()
    {
        { '/', "$1" },
        { '$', "$2" },
        { '@', "$3" },
        { '!', "$4" },
        { '#', "$5" },
        { '%', "$6" },
        { '&', "$7" },
        { '*', "$8" },
        { '_', "$9" },
        { '-', "$10" },
        { '+', "$11" },
        { '.', "$12" },
        { '=', "$13" },
        { ',', "$14" },
        { ';', "$15" },
        { '?', "$16" },
        { '~', "$17" },
        { '^', "$18" },
        { '[', "$19" },
        { ']', "$20" },
        { '(', "$21" },
        { ')', "$22" },
        { '{', "$23" },
        { '}', "$24" }
    };

    /// <summary>
    /// Automatically converts the player uid to a safer folder system
    /// </summary>
    static public string ConvertPlayerUID(string uid)
    {
        if (string.IsNullOrEmpty(uid))
            return "INVALID_UID";

        var result = new System.Text.StringBuilder();

        foreach (char c in uid)
        {
            if (map.TryGetValue(c, out string replacement))
                result.Append(replacement);
            else
                result.Append(c);
        }

        return result.ToString();
    }

    [GeneratedRegex(@"^[a-zA-Z0-9!@#$%&_+\-/]+$")]
    private static partial Regex NumbersAndLetters();

    /// <summary>
    /// Gets the base value, checks the new value returns the difference between then
    /// </summary>
    /// <param name="primary"></param>
    /// <param name="secondary"></param>
    /// <returns></returns>
    static public float GetDifferenceBetweenTwoFloats(float primary, float secondary)
    {
        // Both positive
        if (primary > 0 && secondary > 0)
        {
            // New value is increasing the base
            // so difference must be positive
            if (primary < secondary)
            {
                float difference = secondary - primary;
                return difference;
            }
            // New value is decreasing the base
            // so difference must be negative
            else if (primary > secondary)
            {
                float difference = primary - secondary;
                return -difference;
            }
            // No difference
            else
            {
                return 0f;
            }
        }
        // Negative and positive
        else if ((primary < 0 && secondary > 0) || (primary > 0 && secondary < 0))
        {
            return secondary + primary;
        }
        // Both Negative
        else if (primary < 0 && secondary < 0)
        {
            return secondary - primary;
        }
        // Only primary is different
        else if (primary != 0)
        {
            return primary;
        }
        // Only secondary is different
        else if (secondary != 0)
        {
            return secondary;
        }
        // Both is 0
        else
        {
            return 0f;
        }
    }
}