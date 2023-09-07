using Silk.NET.Maths;
using System.Numerics;

namespace FainEngine_v2.Physics.Obsolete;

public struct OldAABB
{
    public float CenterX;
    public float CenterY;
    public float CenterZ;

    public float HalfWidthX;
    public float HalfWidthY;
    public float HalfWidthZ;

    public readonly float MinX => CenterX - HalfWidthX;
    public readonly float MinY => CenterY - HalfWidthY;
    public readonly float MinZ => CenterZ - HalfWidthZ;
    public readonly float MaxX => CenterX + HalfWidthX;
    public readonly float MaxY => CenterY + HalfWidthY;
    public readonly float MaxZ => CenterZ + HalfWidthZ;

    public readonly float WidthX => HalfWidthX * 2;
    public readonly float WidthY => HalfWidthY * 2;
    public readonly float WidthZ => HalfWidthZ * 2;

    public Vector3 Center
    {
        readonly get => new(CenterX, CenterY, CenterZ);
        set
        {
            CenterX = value.X;
            CenterY = value.Y;
            CenterZ = value.Z;
        }
    }

    public Vector3 Size
    {
        readonly get => new Vector3(HalfWidthX, HalfWidthY, HalfWidthZ) * 2;
        set
        {
            HalfWidthX = value.X / 2;
            HalfWidthY = value.Y / 2;
            HalfWidthZ = value.Z / 2;
        }
    }

    public Vector3 HalfWidth => new Vector3(HalfWidthX, HalfWidthY, HalfWidthZ);
    public Vector3 Width => HalfWidth * 2;
    public Vector3 Min => new Vector3(MinX, MinY, MinZ);
    public Vector3 Max => new Vector3(MaxX, MaxY, MaxZ);

    public static OldAABB FromCenterExtents(Vector3 center, Vector3 extents)
    {
        return new OldAABB()
        {
            CenterX = center.X,
            CenterY = center.Y,
            CenterZ = center.Z,
            HalfWidthX = extents.X,
            HalfWidthY = extents.Y,
            HalfWidthZ = extents.Z,
        };
    }

    public static OldAABB FromCenterSize(Vector3 center, Vector3 size)
    {
        return new OldAABB()
        {
            CenterX = center.X,
            CenterY = center.Y,
            CenterZ = center.Z,
            HalfWidthX = size.X / 2,
            HalfWidthY = size.Y / 2,
            HalfWidthZ = size.Z / 2,
        };
    }

    public static OldAABB FromCenterHalfWidth(float centerX, float centerY, float centerZ, float halfWidthX, float halfWidthY, float halfWidthZ)
    {
        return new OldAABB()
        {
            CenterX = centerX,
            CenterY = centerY,
            CenterZ = centerZ,
            HalfWidthX = halfWidthX,
            HalfWidthY = halfWidthY,
            HalfWidthZ = halfWidthZ,
        };
    }

    public static OldAABB FromMinMax(float xMin, float yMin, float zMin, float xMax, float yMax, float zMax)
    {
        OldAABB result = new()
        {
            CenterX = (xMin + xMax) * 0.5f,
            CenterY = (yMin + yMax) * 0.5f,
            CenterZ = (zMin + zMax) * 0.5f
        };

        result.HalfWidthX = result.CenterX - xMin;
        result.HalfWidthY = result.CenterY - yMin;
        result.HalfWidthZ = result.CenterZ - zMin;

        return result;
    }

    public static OldAABB FromVoxel(int x, int y, int z)
    {
        OldAABB result = new()
        {
            CenterX = x + 0.5f,
            CenterY = y + 0.5f,
            CenterZ = z + 0.5f,
            HalfWidthX = 0.5f,
            HalfWidthY = 0.5f,
            HalfWidthZ = 0.5f,
        };

        return result;
    }

    public static OldAABB FromVoxel(Vector3D<int> voxPos)
    {
        OldAABB result = new()
        {
            CenterX = voxPos.X + 0.5f,
            CenterY = voxPos.Y + 0.5f,
            CenterZ = voxPos.Z + 0.5f,
            HalfWidthX = 0.5f,
            HalfWidthY = 0.5f,
            HalfWidthZ = 0.5f,
        };

        return result;
    }

    public override string ToString()
    {
        return $"AABB Center: {Center} Size: {Size}";
    }
}
