using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

public partial class Plugin
{
    void RegisterEvents()
    {
        RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);

        RegisterEventHandler<EventRoundStart>(EventRoundStart);

        HookEntityOutput("trigger_multiple", "OnStartTouch", trigger_multiple, HookMode.Pre);
    }

    void UnregisterEvents()
    {
        RemoveListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);

        DeregisterEventHandler<EventRoundStart>(EventRoundStart);

        UnhookEntityOutput("trigger_multiple", "OnStartTouch", trigger_multiple, HookMode.Pre);
    }

    void OnServerPrecacheResources(ResourceManifest manifest)
    {
        foreach (var gift in Config.Gifts)
            manifest.AddResource(gift.Model);
    }

    HookResult EventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        DroppedGifts.Clear();
        return HookResult.Continue;
    }

    HookResult trigger_multiple(CEntityIOOutput output, string name, CEntityInstance activator, CEntityInstance caller, CVariant value, float delay)
    {
        if (activator.DesignerName != "player")
            return HookResult.Continue;

        var pawn = activator.As<CCSPlayerPawn>();
        if (pawn == null || !pawn.IsValid)
            return HookResult.Continue;

        var player = pawn.OriginalController?.Value?.As<CCSPlayerController>();
        if (player == null || player.IsBot)
            return HookResult.Continue;

        if (DroppedGifts.TryGetValue(caller, out var package))
            GiftTouched(player, caller, package.entity, package.gift, package.gifter);

        return HookResult.Continue;
    }
}