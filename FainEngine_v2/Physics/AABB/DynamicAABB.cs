using System.Numerics;

namespace FainEngine_v2.Physics.AABB;

public struct DynamicAABB : IAABB
{
    public Vector3 Position;
    public Vector3 Size;
    public Vector3 Delta;

    public Vector3 Center => Position + (Size / 2);

    Vector3 IAABB.Position { get => Position; set => Position = value; }
    Vector3 IAABB.Size => Size;
}
