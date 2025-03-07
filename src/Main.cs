using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Gift Packages";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "exkludera";

    public static Plugin Instance = new();

    public override void Load(bool hotReload)
    {
        Instance = this;

        RegisterEvents();

        foreach (string command in Config.Command)
            Instance.AddCommand(command, "", Menu.Command);
    }

    public override void Unload(bool hotReload)
    {
        UnregisterEvents();

        foreach (string command in Config.Command)
            Instance.RemoveCommand(command, Menu.Command);
    }

    public Config Config { get; set; } = new();
    public void OnConfigParsed(Config config)
    {
        Config = config;
        Config.Prefix = StringExtensions.ReplaceColorTags(config.Prefix);
    }
}