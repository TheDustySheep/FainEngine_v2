using FainEngine_v2.Utils;
using System.Numerics;

namespace FainEngine_v2.Physics.AABB;
public static class AABBResolver
{
    public static DynamicAABB ResolveCollision(DynamicAABB player, List<StaticAABB> colliders, CollisionMode mode)
    {
        // React to the closest collider until none left
        while (colliders.Any() && player.Delta != Vector3.Zero)
        {
            int closestIndex = -1;
            float closestTime = 1f;
            for (int i = 0; i < colliders.Count; i++)
            {
                var aabb = colliders[i];
                float colTime = SweptCollision(player, aabb, out var normal);

                if (colTime < closestTime && IsOverlapping(GetSweptBroadphaseBox(player), aabb))
                {
                    closestTime = colTime;
                    closestIndex = i;
                }
            }

            // No impending collisions
            if (closestIndex == -1)
                break;

            StaticAABB voxelAABB = colliders[closestIndex];
            colliders.RemoveAt(closestIndex);
            player = Collide(player, voxelAABB, mode);
        }

        return player;
    }

    public static bool IsOverlapping(IAABB a, IAABB b)
    {
        return
        !(
            a.Position.X + a.Size.X < b.Position.X ||
            a.Position.X > b.Position.X + b.Size.X ||
            a.Position.Y + a.Size.Y < b.Position.Y ||
            a.Position.Y > b.Position.Y + b.Size.Y ||
            a.Position.Z + a.Size.Z < b.Position.Z ||
            a.Position.Z > b.Position.Z + b.Size.Z
        );
    }

    public static StaticAABB GetSweptBroadphaseBox(DynamicAABB b)
    {
        StaticAABB broadphasebox = new();

        broadphasebox.Position.X = b.Delta.X > 0 ? b.Position.X : b.Position.X + b.Delta.X;
        broadphasebox.Position.Y = b.Delta.Y > 0 ? b.Position.Y : b.Position.Y + b.Delta.Y;
        broadphasebox.Position.Z = b.Delta.Z > 0 ? b.Position.Z : b.Position.Z + b.Delta.Z;

        broadphasebox.Size.X = b.Delta.X > 0 ? b.Delta.X + b.Size.X : b.Size.X - b.Delta.X;
        broadphasebox.Size.Y = b.Delta.Y > 0 ? b.Delta.Y + b.Size.Y : b.Size.Y - b.Delta.Y;
        broadphasebox.Size.Z = b.Delta.Z > 0 ? b.Delta.Z + b.Size.Z : b.Size.Z - b.Delta.Z;

        return broadphasebox;
    }

    public static OverlapAABB CalculateOverlap(StaticAABB a, StaticAABB b)
    {
        Vector3 min = new Vector3
        (
            Math.Max(a.Min.X, b.Min.X),
            Math.Max(a.Min.Y, b.Min.Y),
            Math.Max(a.Min.Z, b.Min.Z)
        );

        Vector3 max = new Vector3
        (
            Math.Min(a.Max.X, b.Max.X),
            Math.Min(a.Max.Y, b.Max.Y),
            Math.Min(a.Max.Z, b.Max.Z)
        );

        return new OverlapAABB()
        {
            Min = min,
            Max = max,
        };
    }

    public static Vector3 ResolveOverlap(StaticAABB a, OverlapAABB overlap)
    {
        if (overlap.Size.X < overlap.Size.Y &&
            overlap.Size.X < overlap.Size.Z)
        {
            // XPos_px is smallest overlap
            if (a.Center.X > overlap.Position.X)
                return Vector3.UnitX * overlap.Size.X;
            else
                return Vector3.UnitX * -overlap.Size.X;
        }
        else if (
            overlap.Size.Y < overlap.Size.X &&
            overlap.Size.Y < overlap.Size.Z)
        {
            // YPox_px is smallest overlap
            if (a.Center.Y > overlap.Position.Y)
                return Vector3.UnitY * overlap.Size.Y;
            else
                return Vector3.UnitY * -overlap.Size.Y;
        }
        else if (
            overlap.Size.Z < overlap.Size.X &&
            overlap.Size.Z < overlap.Size.Y)
        {
            if (a.Center.Z > overlap.Position.Z)
                return Vector3.UnitZ * overlap.Size.Z;
            else
                return Vector3.UnitZ * -overlap.Size.Z;
        }
        else
        {
            return Vector3.Zero;
        }
    }

    public static DynamicAABB Collide(DynamicAABB entity, StaticAABB aabb, CollisionMode mode)
    {
        // Collisions disabled
        if (mode == CollisionMode.NoCollisions)
            return entity;

        // Sweep collision
        float collisiontime = SweptCollision(entity, aabb, out Vector3 normal);

        // No collision
        if (collisiontime == 1f || normal == Vector3.Zero)
            return entity;

        // Move to collision point
        entity.Position += entity.Delta * collisiontime;
        entity.Position += normal * 0.001f;
        float remainingtime = 1.0f - collisiontime;

        // Stop at collision point
        if (mode == CollisionMode.Stop)
        {
            entity.Delta = Vector3.Zero;
            return entity;
        }

        // Stop at collision point
        if (mode == CollisionMode.Slide)
        {
            float dotProd = Vector3.Dot(entity.Delta, normal);
            Vector3 normRemove = dotProd * normal;
            entity.Delta -= normRemove;
            entity.Delta *= remainingtime;
            return entity;
        }

        return entity;
    }

    const float INF = 1e20f;
    public static float SweptCollision(DynamicAABB b1, StaticAABB b2, out Vector3 normal)
    {
        // find the distance between the objects on the near and far sides for both x and y 
        Vector3 InvEntry;
        Vector3 InvExit;

        if (b1.Delta.X > 0.0f)
        {
            InvEntry.X = b2.Position.X - (b1.Position.X + b1.Size.X);
            InvExit.X = b2.Position.X + b2.Size.X - b1.Position.X;
        }
        else
        {
            InvEntry.X = b2.Position.X + b2.Size.X - b1.Position.X;
            InvExit.X = b2.Position.X - (b1.Position.X + b1.Size.X);
        }

        if (b1.Delta.Y > 0.0f)
        {
            InvEntry.Y = b2.Position.Y - (b1.Position.Y + b1.Size.Y);
            InvExit.Y = b2.Position.Y + b2.Size.Y - b1.Position.Y;
        }
        else
        {
            InvEntry.Y = b2.Position.Y + b2.Size.Y - b1.Position.Y;
            InvExit.Y = b2.Position.Y - (b1.Position.Y + b1.Size.Y);
        }

        if (b1.Delta.Z > 0.0f)
        {
            InvEntry.Z = b2.Position.Z - (b1.Position.Z + b1.Size.Z);
            InvExit.Z = b2.Position.Z + b2.Size.Z - b1.Position.Z;
        }
        else
        {
            InvEntry.Z = b2.Position.Z + b2.Size.Z - b1.Position.Z;
            InvExit.Z = b2.Position.Z - (b1.Position.Z + b1.Size.Z);
        }

        // find time of collision and time of leaving for each axis (if statement is to prevent divide by zero) 
        Vector3 Entry;
        Vector3 Exit;

        if (b1.Delta.X == 0.0f)
        {
            Entry.X = -INF;
            Exit.X  =  INF;
        }
        else
        {
            Entry.X = InvEntry.X / b1.Delta.X;
            Exit.X = InvExit.X / b1.Delta.X;
        }

        if (b1.Delta.Y == 0.0f)
        {
            Entry.Y = -INF;
            Exit.Y  =  INF;
        }
        else
        {
            Entry.Y = InvEntry.Y / b1.Delta.Y;
            Exit.Y = InvExit.Y / b1.Delta.Y;
        }

        if (b1.Delta.Z == 0.0f)
        {
            Entry.Z = -INF;
            Exit.Z  =  INF;
        }
        else
        {
            Entry.Z = InvEntry.Z / b1.Delta.Z;
            Exit.Z = InvExit.Z / b1.Delta.Z;
        }

        // find the earliest/latest times of collisionfloat 
        float entryTime = MathUtils.Max(Entry.X, Entry.Y, Entry.Z);
        float exitTime = MathUtils.Min(Exit.X, Exit.Y, Exit.Z);

        // if there was no collision
        if (entryTime > exitTime ||
            Entry.X < 0.0f && Entry.Y < 0.0f && Entry.Z < 0.0f ||
            Entry.X > 1.0f || Entry.Y > 1.0f || Entry.Z > 1.0f)
        {
            normal = Vector3.Zero;
            return 1.0f;
        }

        // if there was a collision 
        else
        {
            if (Entry.Z > Entry.X)
            {
                // calculate normal of collided surface
                if (Entry.Z > Entry.Y)
                {
                    normal = InvEntry.Z < 0.0f ? Vector3.UnitZ : -Vector3.UnitZ;
                }
                else
                {
                    normal = InvEntry.Y < 0.0f ? Vector3.UnitY : -Vector3.UnitY;
                }
            }
            else
            {
                // calculate normal of collided surface
                if (Entry.X > Entry.Y)
                {
                    normal = InvEntry.X < 0.0f ? Vector3.UnitX : -Vector3.UnitX;
                }
                else
                {
                    normal = InvEntry.Y < 0.0f ? Vector3.UnitY : -Vector3.UnitY;
                }
            }

            return entryTime;
        }
    }
}
