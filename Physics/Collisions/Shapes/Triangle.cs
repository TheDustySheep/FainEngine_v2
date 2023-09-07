using FainEngine_v2.Physics.Collisions.Shapes;
using System.Numerics;

namespace Voxels.Utils.Collisions
{

    public struct Triangle : ICollisionShape
    {
        public ShapeType ShapeType => ShapeType.Triangle;

        public readonly Vector3 A => _a;
        public readonly Vector3 B => _b;
        public readonly Vector3 C => _c;

        public readonly Vector3 AB => _ab;
        public readonly Vector3 BC => _bc;
        public readonly Vector3 CA => _ca;

        public readonly Vector3 Normal => _normal;

        Vector3 _a, _b, _c, _normal;
        Vector3 _ab, _bc, _ca;

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            _a = a;
            _b = b;
            _c = c;
            _ab = b - a;
            _bc = c - b;
            _ca = a - c;

            var cross = Vector3.Cross(_ab, _bc);
            _normal = cross / cross.Length();
        }

        public bool Intersects(ICollisionShape other, out Vector3 point)
        {
            throw new NotImplementedException();
        }
    }
}
