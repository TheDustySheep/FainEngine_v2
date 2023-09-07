using FainEngine_v2.Extensions;
using System.Numerics;

namespace FainEngine_v2.Physics.Collisions.Shapes;

[Serializable]
public struct AABB : ICollisionShape
{
    public readonly ShapeType ShapeType => ShapeType.AABB;
    public readonly Vector3 Min => _min;
    public readonly Vector3 Max => _max;
    public readonly Vector3 Center => (_min + _max) * 0.5f;

    public readonly Vector3 Size => (_max - _min) * 0.5f;

    Vector3 _min;
    Vector3 _max;

    public AABB(Vector3 min, Vector3 max)
    {
        _min = Vector3.Min(min, max);
        _max = Vector3.Max(min, max);
    }

    public Vector3 NormalAt(Vector3 point)
    {
        var normal = Vector3.Zero;
        var localPoint = point - Center;
        float min = float.MaxValue;
        var size = Size;

        float distance = MathF.Abs(size.X - MathF.Abs(localPoint.X));
        if (distance < min)
        {
            min = distance;
            normal = new Vector3(1, 0, 0);
            normal *= MathF.Sign(localPoint.X);
        }
        distance = MathF.Abs(size.Y - MathF.Abs(localPoint.Y));
        if (distance < min)
        {
            min = distance;
            normal = new Vector3(0, 1, 0);
            normal *= MathF.Sign(localPoint.Y);
        }
        distance = MathF.Abs(size.Z - MathF.Abs(localPoint.Z));
        if (distance < min)
        {
            min = distance;
            normal = new Vector3(0, 0, 1);
            normal *= MathF.Sign(localPoint.Z);
        }
        return normal;
    }

    public Vector3 BoxNormal(Vector3 point)
    {
        var pc = point - Center;
        var normal = Vector3.Zero;
        normal += new Vector3(MathF.Sign(pc.X), 0f, 0f) * Step(MathF.Abs(MathF.Abs(pc.X) - Size.X), float.Epsilon);
        normal += new Vector3(0f, MathF.Sign(pc.Y), 0f) * Step(MathF.Abs(MathF.Abs(pc.Y) - Size.Y), float.Epsilon);
        normal += new Vector3(0f, 0f, MathF.Sign(pc.Z)) * Step(MathF.Abs(MathF.Abs(pc.Z) - Size.Z), float.Epsilon);
        return normal.Normalized();
    }

    static float Step(float edge, float x)
    {
        return x < edge ? 0 : 1;
    }

    public bool Intersects(ICollisionShape other, out Vector3 point)
    {
        throw new NotImplementedException();
    }

    public AABB AddToOrigin(Vector3 offset)
    {
        return new AABB(_min + offset, _max + offset);
    }
}
