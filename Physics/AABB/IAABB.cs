using System.Numerics;

namespace FainEngine_v2.Physics.AABB
{
    public interface IAABB
    {
        public Vector3 Position { get; set; }
        public Vector3 Size { get; }
    }
}
