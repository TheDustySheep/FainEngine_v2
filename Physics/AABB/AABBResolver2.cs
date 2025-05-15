using FainEngine_v2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.Physics.AABB;

public static class AABBResolver2
{/// <summary>
 /// Resolve collisions by reacting to the closest collider until movement delta is exhausted.
 /// </summary>
    public static DynamicAABB ResolveCollision(DynamicAABB player, List<StaticAABB> colliders, CollisionMode mode)
    {
        while (player.Delta != Vector3.Zero && colliders.Count > 0)
        {
            var broadphase = GetSweptBroadphaseBox(player);
            int closestIndex = -1;
            float closestTime = 1f;

            for (int i = 0; i < colliders.Count; i++)
            {
                var aabb = colliders[i];
                if (!IsOverlapping(broadphase, aabb))
                    continue;

                var time = SweptCollision(player, aabb, out _);
                if (time < closestTime)
                {
                    closestTime = time;
                    closestIndex = i;
                }
            }

            if (closestIndex < 0)
                break;

            var hit = colliders[closestIndex];
            colliders.RemoveAt(closestIndex);
            player = Collide(player, hit, mode);
        }

        return player;
    }

    /// <summary>Axis-aligned overlap check using Min/Max points.</summary>
    public static bool IsOverlapping(StaticAABB a, StaticAABB b)
    {
        return !(a.Max.X < b.Min.X || a.Min.X > b.Max.X ||
                 a.Max.Y < b.Min.Y || a.Min.Y > b.Max.Y ||
                 a.Max.Z < b.Min.Z || a.Min.Z > b.Max.Z);
    }

    /// <summary>Compute broadphase box that encloses the swept volume.</summary>
    public static StaticAABB GetSweptBroadphaseBox(DynamicAABB b)
    {
        var min = Vector3.Min(b.Position, b.Position + b.Delta);
        var max = Vector3.Max(b.Position + b.Size, b.Position + b.Size + b.Delta);

        var box = new StaticAABB
        {
            Position = min,
            Size = max - min
        };

        return box;
    }

    /// <summary>Calculate overlapping AABB between two boxes.</summary>
    public static OverlapAABB CalculateOverlap(StaticAABB a, StaticAABB b)
    {
        var min = Vector3.Max(a.Min, b.Min);
        var max = Vector3.Min(a.Max, b.Max);
        return new OverlapAABB { Min = min, Max = max };
    }

    /// <summary>Resolve static overlap by pushing out along smallest axis.</summary>
    public static Vector3 ResolveOverlap(StaticAABB a, OverlapAABB overlap)
    {
        var size = overlap.Size;
        if (size.X < size.Y && size.X < size.Z)
            return (a.Center.X > overlap.Position.X ? Vector3.UnitX : -Vector3.UnitX) * size.X;
        if (size.Y < size.X && size.Y < size.Z)
            return (a.Center.Y > overlap.Position.Y ? Vector3.UnitY : -Vector3.UnitY) * size.Y;
        if (size.Z < size.X && size.Z < size.Y)
            return (a.Center.Z > overlap.Position.Z ? Vector3.UnitZ : -Vector3.UnitZ) * size.Z;

        return Vector3.Zero;
    }

    /// <summary>Perform collision resolution based on mode.</summary>
    public static DynamicAABB Collide(DynamicAABB entity, StaticAABB aabb, CollisionMode mode)
    {
        if (mode == CollisionMode.NoCollisions)
            return entity;

        var t = SweptCollision(entity, aabb, out var normal);
        if (t >= 1f || normal == Vector3.Zero)
            return entity;

        entity.Position += entity.Delta * t + normal * 0.001f;
        var remain = 1f - t;

        if (mode == CollisionMode.Stop)
        {
            entity.Delta = Vector3.Zero;
        }
        else if (mode == CollisionMode.Slide)
        {
            var proj = Vector3.Dot(entity.Delta, normal) * normal;
            entity.Delta = (entity.Delta - proj) * remain;
        }

        return entity;
    }

    private const float INF = 1e20f;

    /// <summary>Continuous swept AABB collision detection.</summary>
    public static float SweptCollision(DynamicAABB b1, StaticAABB b2, out Vector3 normal)
    {
        var invEntry = new Vector3();
        var invExit = new Vector3();

        var b1Max = b1.Position + b1.Size;
        var b2Max = b2.Position + b2.Size;

        // X axis
        if (b1.Delta.X > 0)
        {
            invEntry.X = b2.Position.X - b1Max.X;
            invExit.X = b2Max.X - b1.Position.X;
        }
        else
        {
            invEntry.X = b2Max.X - b1.Position.X;
            invExit.X = b2.Position.X - b1Max.X;
        }

        // Y axis
        if (b1.Delta.Y > 0)
        {
            invEntry.Y = b2.Position.Y - b1Max.Y;
            invExit.Y = b2Max.Y - b1.Position.Y;
        }
        else
        {
            invEntry.Y = b2Max.Y - b1.Position.Y;
            invExit.Y = b2.Position.Y - b1Max.Y;
        }

        // Z axis
        if (b1.Delta.Z > 0)
        {
            invEntry.Z = b2.Position.Z - b1Max.Z;
            invExit.Z = b2Max.Z - b1.Position.Z;
        }
        else
        {
            invEntry.Z = b2Max.Z - b1.Position.Z;
            invExit.Z = b2.Position.Z - b1Max.Z;
        }

        // Compute entry and exit times
        var entry = new Vector3(
            b1.Delta.X == 0 ? -INF : invEntry.X / b1.Delta.X,
            b1.Delta.Y == 0 ? -INF : invEntry.Y / b1.Delta.Y,
            b1.Delta.Z == 0 ? -INF : invEntry.Z / b1.Delta.Z
        );

        var exit = new Vector3(
            b1.Delta.X == 0 ? INF : invExit.X / b1.Delta.X,
            b1.Delta.Y == 0 ? INF : invExit.Y / b1.Delta.Y,
            b1.Delta.Z == 0 ? INF : invExit.Z / b1.Delta.Z
        );

        var entryTime = MathUtils.Max(entry.X, entry.Y, entry.Z);
        var exitTime = MathUtils.Min(exit.X, exit.Y, exit.Z);

        // No collision
        if (entryTime > exitTime || (entry.X < 0 && entry.Y < 0 && entry.Z < 0) ||
            entry.X > 1 || entry.Y > 1 || entry.Z > 1)
        {
            normal = Vector3.Zero;
            return 1f;
        }

        // Determine collision normal
        if (entryTime == entry.X)
            normal = invEntry.X < 0 ? Vector3.UnitX : -Vector3.UnitX;
        else if (entryTime == entry.Y)
            normal = invEntry.Y < 0 ? Vector3.UnitY : -Vector3.UnitY;
        else
            normal = invEntry.Z < 0 ? Vector3.UnitZ : -Vector3.UnitZ;

        return entryTime;
    }
}
