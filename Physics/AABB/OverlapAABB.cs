using System.Numerics;

namespace FainEngine_v2.Physics.AABB;
public struct OverlapAABB
{
    public Vector3 Min;
    public Vector3 Max;
    public readonly Vector3 Size => Max - Min;
    public readonly Vector3 Center => (Max + Min) / 2;
}
