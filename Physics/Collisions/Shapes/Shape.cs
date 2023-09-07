using System.Numerics;

namespace FainEngine_v2.Physics.Collisions.Shapes;

public enum ShapeType
{
    Ray,
    AABB,
    Plane,
    Triangle
}

public interface ICollisionShape
{
    public ShapeType ShapeType { get; }
    public bool Intersects(ICollisionShape other, out Vector3 point);
}
