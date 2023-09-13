using FainEngine_v2.Rendering.BoundingShapes;
using System.Numerics;

namespace FainEngine_v2.Rendering.Cameras;
public class NullCamera : ICamera
{
    public Matrix4x4 ViewMatrix => throw new NotImplementedException();

    public Matrix4x4 ProjectionMatrix => throw new NotImplementedException();

    public Frustum Frustum => default;
}
