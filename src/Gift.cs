using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

public partial class Plugin
{
    public Dictionary<CEntityInstance, (CBaseProp entity, GiftPackage gift, CCSPlayerController gifter)> DroppedGifts = new();

    public void CreateGift(CCSPlayerController gifter, GiftPackage gift, bool Throw)
    {
        var pawn = gifter.PlayerPawn.Value;
        if (pawn == null) return;

        var entity = Utilities.CreateEntityByName<CPhysicsPropOverride>("prop_physics_override")!;

        entity.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(uint)(1 << 2);

        entity.SetModel(gift.Model);
        entity.DispatchSpawn();

        entity.Collision.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_DISSOLVING;
        entity.Collision.CollisionAttribute.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_DISSOLVING;
        Utilities.SetStateChanged(entity, "CCollisionProperty", "m_CollisionGroup");
        Utilities.SetStateChanged(entity, "VPhysicsCollisionAttribute_t", "m_nCollisionGroup");

        if (Throw)
        {
            float angleYaw = pawn.EyeAngles.Y * (float)Math.PI / 180f;
            float anglePitch = pawn.EyeAngles.X * (float)Math.PI / 180f;

            Vector forward = new Vector(
                (float)(Math.Cos(anglePitch) * Math.Cos(angleYaw)),
                (float)(Math.Cos(anglePitch) * Math.Sin(angleYaw)),
                (float)(-Math.Sin(anglePitch))
            );

            float spawnDistance = 64.0f;
            Vector spawnPosition = new Vector(
                pawn.AbsOrigin!.X + forward.X * spawnDistance,
                pawn.AbsOrigin.Y + forward.Y * spawnDistance,
                pawn.AbsOrigin.Z + forward.Z * spawnDistance + 32.0f
            );

            float throwStrength = 500.0f;
            Vector velocity = new Vector(
                forward.X * throwStrength,
                forward.Y * throwStrength,
                forward.Z * throwStrength + 300.0f
            );

            entity.Teleport(spawnPosition, pawn.AbsRotation, velocity);
        }
        else
        {
            Vector position = new Vector(pawn.AbsOrigin!.X, pawn.AbsOrigin.Y, pawn.AbsOrigin.Z + pawn.CameraServices?.OldPlayerViewOffsetZ);

            var hitPoint = Utils.TraceShape(position, pawn.EyeAngles);
            if (hitPoint == null)
            {
                Utils.PrintToChat(gifter, $"{ChatColors.Red}Could not find a valid location to spawn gift");
                return;
            }

            entity.Teleport(hitPoint, gifter.AbsRotation);
        }

        var trigger = CreateTrigger(entity);
        if (trigger != null)
        {
            DroppedGifts.Add(trigger, (entity, gift, gifter));
            if (gift.Announce) Utils.PrintToChatAll(Localizer["Message3", gifter.PlayerName, gift.Name]);
        }
    }

    CTriggerMultiple CreateTrigger(CBaseProp pack)
    {
        var trigger = Utilities.CreateEntityByName<CTriggerMultiple>("trigger_multiple")!;

        trigger.Entity!.Name = pack.Entity!.Name + "_trigger";
        trigger.Spawnflags = 1;
        trigger.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(uint)(1 << 2);
        trigger.Collision.SolidType = SolidType_t.SOLID_VPHYSICS;
        trigger.Collision.SolidFlags = 0;
        trigger.Collision.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_TRIGGER;

        trigger.SetModel(pack.CBodyComponent!.SceneNode!.GetSkeletonInstance().ModelState.ModelName);
        trigger.Teleport(pack.AbsOrigin, pack.AbsRotation);
        trigger.DispatchSpawn();
        trigger.AcceptInput("FollowEntity", pack, trigger, "!activator");
        trigger.AcceptInput("Enable");

        return trigger;
    }

    public void GiftTouched(CCSPlayerController player, CEntityInstance trigger, CBaseProp entity, GiftPackage gift, CCSPlayerController gifter)
    {
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return;

        DroppedGifts.Remove(trigger);

        if (trigger != null && trigger.IsValid)
            trigger.Remove();

        if (entity != null && entity.IsValid)
            entity.Remove();

        if (!gift.Quiet)
        {
            Utils.PrintToChat(player, Localizer["Message1", gift.Name, gifter.PlayerName]);

            if (gift.Announce) Utils.PrintToChat(gifter, Localizer["Message4", gift.Name, player.PlayerName]);
            else Utils.PrintToChatAll(Localizer["Message2", gift.Name, player.PlayerName]);

            player.EmitSound(gift.SoundEvent);
        }

        foreach (string command in gift.Command)
            Server.ExecuteCommand(Utils.Replace(command, player, gifter));
    }
}