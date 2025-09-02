using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;

public static class Utils
{
    private static Plugin instance = Plugin.Instance;
    private static Config config = instance.Config;

    public static bool HasPermission(CCSPlayerController player)
    {
        foreach (string perm in config.Permission)
        {
            if (perm.StartsWith("@") && AdminManager.PlayerHasPermissions(player, perm))
                return true;
            if (perm.StartsWith("#") && AdminManager.PlayerInGroup(player, perm))
                return true;
        }
        return false;
    }

    public static void PrintToChat(CCSPlayerController player, string message)
    {
        player.PrintToChat($" {config.Prefix} {message}");
    }

    public static void PrintToChatAll(string message)
    {
        Server.PrintToChatAll($" {config.Prefix} {message}");
    }

    public static string Replace(string input, CCSPlayerController player, CCSPlayerController gifter)
    {
        return input
        .Replace("{NAME}", player.PlayerName)
        .Replace("{STEAMID}", player.AuthorizedSteamID?.SteamId2.ToString() ?? "0")
        .Replace("{STEAMID3}", player.AuthorizedSteamID?.SteamId3.ToString() ?? "0")
        .Replace("{STEAMID64}", player.AuthorizedSteamID?.SteamId64.ToString() ?? "0")
        .Replace("{USERID}", player.UserId?.ToString() ?? "-1")
        .Replace("{SLOT}", player.Slot.ToString())
        .Replace("{GIFTER_NAME}", gifter.PlayerName)
        .Replace("{GIFTER_STEAMID}", gifter.AuthorizedSteamID?.SteamId2.ToString() ?? "0")
        .Replace("{GIFTER_STEAMID3}", gifter.AuthorizedSteamID?.SteamId3.ToString() ?? "0")
        .Replace("{GIFTER_STEAMID64}", gifter.AuthorizedSteamID?.SteamId64.ToString() ?? "0")
        .Replace("{GIFTER_USERID}", gifter.UserId?.ToString() ?? "-1")
        .Replace("{GIFTER_SLOT}", gifter.Slot.ToString());
    }
}