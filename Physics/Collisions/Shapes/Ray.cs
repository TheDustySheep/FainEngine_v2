using System.Numerics;

namespace FainEngine_v2.Physics.Collisions.Shapes;

public struct Ray : ICollisionShape
{
    public ShapeType ShapeType => ShapeType.Ray;

    public readonly Vector3 Point => _point;
    public readonly Vector3 Dir => _dir;
    public readonly Vector3 InvDir => _invDir;
    public readonly Vector3 Sign => _sign;

    Vector3 _point;
    Vector3 _dir;
    Vector3 _invDir;
    Vector3 _sign;

    public Ray(Vector3 point, Vector3 dir)
    {
        _point = point;

        _dir = dir;
        _invDir = new Vector3(
            1f / dir.X,
            1f / dir.Y,
            1f / dir.Z
        );
        _sign = new Vector3(
            _invDir.X < 0f ? 1 : 0,
            _invDir.Y < 0f ? 1 : 0,
            _invDir.Z < 0f ? 1 : 0
        );
    }


    public bool Intersects(ICollisionShape other, out Vector3 point)
    {
        throw new NotImplementedException();
    }
}
