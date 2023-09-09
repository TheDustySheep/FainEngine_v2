using System.Numerics;

namespace FainEngine_v2.Physics.AABB;

public struct DynamicAABB
{
    public Vector3 Position;
    public Vector3 Size;
    public Vector3 Delta;

    public Vector3 Center => Position + (Size / 2);
}
