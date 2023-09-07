using System.Numerics;

namespace FainEngine_v2.Physics.Obsolete;

public static class AABBSolver
{
    public static bool AreOverlapping(OldAABB a, OldAABB b)
    {
        bool x = Math.Abs(a.CenterX - b.CenterX) <= a.HalfWidthX + b.HalfWidthX;
        bool y = Math.Abs(a.CenterY - b.CenterY) <= a.HalfWidthY + b.HalfWidthY;
        bool z = Math.Abs(a.CenterZ - b.CenterZ) <= a.HalfWidthZ + b.HalfWidthZ;

        return x && y && z;
    }

    public static OldAABB CalculateOverlap(OldAABB a, OldAABB b)
    {
        return OldAABB.FromMinMax
        (
            Math.Max(a.MinX, b.MinX),
            Math.Max(a.MinY, b.MinY),
            Math.Max(a.MinZ, b.MinZ),
            Math.Min(a.MaxX, b.MaxX),
            Math.Min(a.MaxY, b.MaxY),
            Math.Min(a.MaxZ, b.MaxZ)
        );
    }

    public static Vector3 ResolveOverlap(OldAABB a, OldAABB overlap)
    {
        if (overlap.WidthX < overlap.WidthY)
        {
            if (overlap.WidthX < overlap.WidthZ)
            {
                if (a.CenterX > overlap.CenterX)
                    return new Vector3(overlap.WidthX, 0, 0);
                else
                    return new Vector3(-overlap.WidthX, 0, 0);
            }
            else
            {
                if (a.CenterZ > overlap.CenterZ)
                    return new Vector3(0, 0, overlap.WidthZ);
                else
                    return new Vector3(0, 0, -overlap.WidthZ);
            }
        }
        else
        {
            if (overlap.WidthY < overlap.WidthZ)
            {
                if (a.CenterY > overlap.CenterY)
                    return new Vector3(0, overlap.WidthY, 0);
                else
                    return new Vector3(0, -overlap.WidthY, 0);
            }
            else
            {
                if (a.CenterZ > overlap.CenterZ)
                    return new Vector3(0, 0, overlap.WidthZ);
                else
                    return new Vector3(0, 0, -overlap.WidthZ);
            }
        }
    }
}