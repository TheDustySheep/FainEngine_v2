using System.Numerics;

namespace FainEngine_v2.Physics.Collisions.Shapes;

public struct Plane : ICollisionShape
{
    public readonly ShapeType ShapeType => ShapeType.Plane;
    public readonly Vector3 Normal => _normal;
    public readonly float Distance => _distance;

    Vector3 _normal;
    readonly float _distance;

    public Plane(Vector3 normal, float distance)
    {
        _normal = normal;
        _distance = distance;
    }

    public bool Intersects(ICollisionShape other, out Vector3 point)
    {
        throw new NotImplementedException();
    }
}
