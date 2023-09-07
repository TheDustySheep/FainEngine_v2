using FainEngine_v2.Utils;
using System.Numerics;

namespace FainEngine_v2.Physics;
public struct StaticAABB
{
    public Vector3 Position;
    public Vector3 Size;
}

public struct DynamicAABB
{
    public Vector3 Position;
    public Vector3 Size;
    public Vector3 Velocity;
}

public static class AABBSolver
{
    public static void Collide(ref DynamicAABB b1, StaticAABB b2, CollisionMode mode)
    {
        // Collisions disabled
        if (mode == CollisionMode.NoCollisions)
            return;

        // Sweep collision
        float collisiontime = SweptCollision(b1, b2, out Vector3 normal);

        // No collision
        if (collisiontime == 1f || normal == Vector3.Zero)
            return;

        // Move to collision point
        b1.Position += b1.Velocity * collisiontime;
        float remainingtime = 1.0f - collisiontime;

        // Stop at collision point
        if (mode == CollisionMode.Stop)
        {
            b1.Velocity = Vector3.Zero;
            return;
        }

        // Stop at collision point
        if (mode == CollisionMode.Slide)
        {
            b1.Velocity *= remainingtime;

            Console.WriteLine($"Collided face {normal}");

            float dotprod = Vector3.Dot(b1.Velocity, normal);
            Vector3 normalVel = dotprod * normal * 1.00001f;
            b1.Velocity -= normalVel;
            return;
        }
    }

    public static float SweptCollision(DynamicAABB b1, StaticAABB b2, out Vector3 normal)
    {
        // find the distance between the objects on the near and far sides for both x and y 
        Vector3 InvEntry;
        Vector3 InvExit;

        if (b1.Velocity.X > 0.0f)
        {
            InvEntry.X = b2.Position.X - (b1.Position.X + b1.Size.X);
            InvExit.X = b2.Position.X + b2.Size.X - b1.Position.X;
        }
        else
        {
            InvEntry.X = b2.Position.X + b2.Size.X - b1.Position.X;
            InvExit.X = b2.Position.X - (b1.Position.X + b1.Size.X);
        }

        if (b1.Velocity.Y > 0.0f)
        {
            InvEntry.Y = b2.Position.Y - (b1.Position.Y + b1.Size.Y);
            InvExit.Y = b2.Position.Y + b2.Size.Y - b1.Position.Y;
        }
        else
        {
            InvEntry.Y = b2.Position.Y + b2.Size.Y - b1.Position.Y;
            InvExit.Y = b2.Position.Y - (b1.Position.Y + b1.Size.Y);
        }

        if (b1.Velocity.Z > 0.0f)
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

        if (b1.Velocity.X == 0.0f)
        {
            Entry.X = float.NegativeInfinity;
            Exit.X = float.PositiveInfinity;
        }
        else
        {
            Entry.X = InvEntry.X / b1.Velocity.X;
            Exit.X = InvExit.X / b1.Velocity.X;
        }

        if (b1.Velocity.Y == 0.0f)
        {
            Entry.Y = float.NegativeInfinity;
            Exit.Y = float.PositiveInfinity;
        }
        else
        {
            Entry.Y = InvEntry.Y / b1.Velocity.Y;
            Exit.Y = InvExit.Y / b1.Velocity.Y;
        }

        if (b1.Velocity.Z == 0.0f)
        {
            Entry.Z = float.NegativeInfinity;
            Exit.Z = float.PositiveInfinity;
        }
        else
        {
            Entry.Z = InvEntry.Z / b1.Velocity.Z;
            Exit.Z = InvExit.Z / b1.Velocity.Z;
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

    public enum CollisionMode
    {
        NoCollisions,
        Stop,
        Slide,
    }
}
