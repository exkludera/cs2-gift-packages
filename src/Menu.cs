using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;

public static partial class Menu
{
    private static Plugin Instance = Plugin.Instance;
    private static Config Config = Instance.Config;

    public static void Command(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null || !Utils.HasPermission(player))
            return;

        Open(player);
    }

    public static void Open(CCSPlayerController? player)
    {
        if (player == null) return;

        CenterHtmlMenu Menu = new("Gift Packages", Instance);

        foreach (var gift in Config.Gifts)
        {
            Menu.AddMenuOption(gift.Name, (player, option) =>
            {
                Options(player, gift);
            });
        }

        MenuManager.OpenCenterHtmlMenu(Instance, player, Menu);
    }

    public static void Options(CCSPlayerController? player, GiftPackage gift)
    {
        if (player == null) return;

        CenterHtmlMenu Menu = new("Spawn Option", Instance);

        Menu.AddMenuOption("Throw", (player, option) =>
        {
            Instance.CreateGift(player, gift, true);
            Open(player);
        });

        Menu.AddMenuOption("Place", (player, option) =>
        {
            Instance.CreateGift(player, gift, false);
            Open(player);
        });

        MenuManager.OpenCenterHtmlMenu(Instance, player, Menu);
    }
}