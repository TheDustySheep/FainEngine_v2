using FainEngine_v2.Physics;
using Silk.NET.Maths;
using System.Numerics;

namespace FainEngine_v2.Utils;
public static class VoxelRaycaster
{
    private enum VoxelFace
    {
        PositiveX,
        NegativeX,
        PositiveY,
        NegativeY,
        PositiveZ,
        NegativeZ,
    }

    public static bool RaycastBlock(Ray ray, float maxDistance, Predicate<Vector3D<int>> selectBlock, out VoxelHit hit)
    {
        return RaycastBlock(ray.Origin, ray.Origin + ray.Direction * maxDistance, selectBlock, out hit);
    }

    public static bool RaycastBlock(Vector3 origin, Vector3 direction, float maxDistance, Predicate<Vector3D<int>> selectBlock, out VoxelHit hit)
    {
        return RaycastBlock(origin, direction * maxDistance, selectBlock, out hit);
    }

    public static bool RaycastBlock(Vector3 _start, Vector3 end, Predicate<Vector3D<int>> selectBlock, out VoxelHit hit)
    {
        Vector3 start = _start;
        int startX = (int)MathF.Floor(start.X);
        int startY = (int)MathF.Floor(start.Y);
        int startZ = (int)MathF.Floor(start.Z);

        int endX = (int)MathF.Floor(end.X);
        int endY = (int)MathF.Floor(end.Y);
        int endZ = (int)MathF.Floor(end.Z);

        int count = 200;

        while ((startX != endX || startY != endY || startZ != endZ) && (count-- > 0))
        {
            float newX = startX;
            float newY = startY;
            float newZ = startZ;

            if (endX > startX)
            {
                newX += 1;
            }

            if (endY > startY)
            {
                newY += 1;
            }

            if (endZ > startZ)
            {
                newZ += 1;
            }

            float xt = float.PositiveInfinity;
            float yt = float.PositiveInfinity;
            float zt = float.PositiveInfinity;

            float dx = end.X - start.X;
            float dy = end.Y - start.Y;
            float dz = end.Z - start.Z;

            if (endX != startX)
            {
                xt = (newX - start.X) / dx;
            }

            if (endY != startY)
            {
                yt = (newY - start.Y) / dy;
            }

            if (endZ != startZ)
            {
                zt = (newZ - start.Z) / dz;
            }

            VoxelFace direction;

            if (xt < yt && xt < zt)
            {
                start.X = newX;
                start.Y += dy * xt;
                start.Z += dz * xt;

                direction = endX > startX ? VoxelFace.NegativeX : VoxelFace.PositiveX;
            }
            else if (yt < zt)
            {
                start.X += dx * yt;
                start.Y = newY;
                start.Z += dz * yt;

                direction = endY > startY ? VoxelFace.NegativeY : VoxelFace.PositiveY;
            }
            else
            {
                start.X += dx * zt;
                start.Y += dy * zt;
                start.Z = newZ;

                direction = endZ > startZ ? VoxelFace.NegativeZ : VoxelFace.PositiveZ;
            }

            startX = (int)MathF.Floor(start.X);
            startY = (int)MathF.Floor(start.Y);
            startZ = (int)MathF.Floor(start.Z);

            switch (direction)
            {
                case VoxelFace.PositiveX:
                    startX--;
                    break;
                case VoxelFace.PositiveY:
                    startY--;
                    break;
                case VoxelFace.PositiveZ:
                    startZ--;
                    break;
            }

            Vector3D<int> pos = new Vector3D<int>(startX, startY, startZ);

            if (!selectBlock.Invoke(pos))
            {
                continue;
            }

            Vector3D<int> normal = direction switch
            {
                VoxelFace.NegativeX => new Vector3D<int>(-1, 0, 0),
                VoxelFace.PositiveX => new Vector3D<int>(1, 0, 0),
                VoxelFace.NegativeY => new Vector3D<int>(0, -1, 0),
                VoxelFace.PositiveY => new Vector3D<int>(0, 1, 0),
                VoxelFace.NegativeZ => new Vector3D<int>(0, 0, -1),
                VoxelFace.PositiveZ => new Vector3D<int>(0, 0, 1),
                _ => default
            };

            hit = new VoxelHit
            {
                Point = start,
                VoxelNormal = normal,
                VoxelPosition = pos,
                Distance = Vector3.Distance(_start, start)
            };

            return true;
        }

        hit = default;
        return false;
    }
}

public struct VoxelHit
{
    public Vector3 Point;
    public Vector3D<int> VoxelNormal;
    public Vector3D<int> VoxelPosition;
    public float Distance;
}