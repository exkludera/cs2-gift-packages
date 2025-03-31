using CounterStrikeSharp.API.Core;

public class GiftPackage
{
    public string Name { get; set; } = "";
    public string Model { get; set; } = "";
    public bool Announce { get; set; }
    public bool Quiet { get; set; }
    public string SoundEvent { get; set; } = "";
    public List<string> Command { get; set; } = [];
}

public class Config : BasePluginConfig
{
    public string Prefix { get; set; } = "{Magenta}[Gifts]{bluegrey}";
    public List<string> Permission { get; set; } = [ "@css/root", "#css/admin" ];
    public List<string> Command { get; set; } = [ "css_giftpackage", "css_giftmenu" ];
    public List<GiftPackage> Gifts { get; set; } = new List<GiftPackage>
    {
        new GiftPackage
        {
            Name = "Example",
            Model = "models/therazu/props/gift/gift.vmdl",
            Announce = true,
            Quiet = false,
            SoundEvent = "UIPanorama.gift_claim",
            Command = [ "css_example {STEAMID64} 1" ]
        }
    };
}