using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;
using System.Runtime.InteropServices;
using System.Numerics;

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

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate bool TraceShapeDelegate(
        nint GameTraceManager,
        nint vecStart,
        nint vecEnd,
        nint skip,
        ulong mask,
        byte a6,
        GameTrace* pGameTrace
    );

    private static TraceShapeDelegate? _traceShape;

    private static nint TraceFunc = NativeAPI.FindSignature(Addresses.ServerPath, GameData.GetSignature("TraceFunc"));

    private static nint GameTraceManager = NativeAPI.FindSignature(Addresses.ServerPath, GameData.GetSignature("GameTraceManager"));

    public static unsafe Vector? TraceShape(Vector _origin, QAngle _viewangles, nint skip = 0, ulong mask = 12289UL, bool drawResult = false)
    {
        var _forward = new Vector();

        NativeAPI.AngleVectors(_viewangles.Handle, _forward.Handle, 0, 0);
        var _endOrigin = new Vector(_origin.X + _forward.X * 8192, _origin.Y + _forward.Y * 8192, _origin.Z + _forward.Z * 8192);

        if (_origin == null)
            return null;

        if (_endOrigin == null)
            return null;

        return TraceShape(_origin, _endOrigin, skip, mask, drawResult);
    }

    public static unsafe Vector? TraceShape(Vector? start, Vector end, nint skip = 0, ulong mask = 12289UL, bool show = false)
    {
        var _gameTraceManagerAddress = Address.GetAbsoluteAddress(GameTraceManager, 3, 7);

        _traceShape = Marshal.GetDelegateForFunctionPointer<TraceShapeDelegate>(TraceFunc);

        var _trace = stackalloc GameTrace[1];

        if (_traceShape == null)
            throw new InvalidOperationException("TraceShape delegate is not initialized.");

        if (_gameTraceManagerAddress == IntPtr.Zero)
            throw new InvalidOperationException("GameTraceManager address is null.");

        if (start == null)
            throw new ArgumentNullException(nameof(start));

        var result = _traceShape(*(nint*)_gameTraceManagerAddress, start.Handle, end.Handle, skip, mask, 4, _trace);

        var endPos = new Vector(_trace->EndPos.X, _trace->EndPos.Y, _trace->EndPos.Z);

        if (result)
            return endPos;

        return null;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0xB8)]
    public unsafe struct GameTrace
    {
        [FieldOffset(0)] public void* Surface;
        [FieldOffset(0x8)] public void* HitEntity;
        [FieldOffset(0x10)] public TraceHitboxData* HitboxData;
        [FieldOffset(0x50)] public uint Contents;
        [FieldOffset(0x78)] public Vector3 StartPos;
        [FieldOffset(0x84)] public Vector3 EndPos;
        [FieldOffset(0x90)] public Vector3 Normal;
        [FieldOffset(0x9C)] public Vector3 Position;
        [FieldOffset(0xAC)] public float Fraction;
        [FieldOffset(0xB6)] public bool AllSolid;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x44)]
    public unsafe struct TraceHitboxData
    {
        [FieldOffset(0x38)] public int HitGroup;
        [FieldOffset(0x40)] public int HitboxId;
    }

    internal static class Address
    {
        static unsafe public nint GetAbsoluteAddress(nint addr, nint offset, int size)
        {
            if (addr == IntPtr.Zero)
            {
                throw new Exception("Failed to find RayTrace signature.");
            }

            int code = *(int*)(addr + offset);
            return addr + code + size;
        }

        static public nint GetCallAddress(nint a)
        {
            return GetAbsoluteAddress(a, 1, 5);
        }
    }
}