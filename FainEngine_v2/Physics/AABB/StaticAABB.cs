using Silk.NET.Maths;
using System.Numerics;

namespace FainEngine_v2.Physics.AABB;

public struct StaticAABB : IAABB
{
    public Vector3 Position;
    public Vector3 Size;

    public Vector3 Min => Position;
    public Vector3 Max => Position + Size;
    public Vector3 Center => Position + (Size / 2);

    Vector3 IAABB.Position { get => Position; set => Position = value; }
    Vector3 IAABB.Size => Size;

    public StaticAABB(Vector3D<int> voxelCoord)
    {
        Position = (Vector3)voxelCoord;
        Size = Vector3.One;
    }
}
