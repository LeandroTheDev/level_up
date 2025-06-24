using System.Text.RegularExpressions;
using LevelUP;
using LevelUP.Server;
using Vintagestory.API.Common;

partial class Utils
{
    // Non authenticated server can let players use whatever ID he wants
    // if a player set their UID as "../../../../Whatever" he can save their experience]
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

    [GeneratedRegex(@"^[a-zA-Z0-9!@#$%&_+\-/]+$")]
    private static partial Regex NumbersAndLetters();
}