using FainEngine_v2.Rendering.BoundingShapes;
using System.Numerics;

namespace FainEngine_v2.Rendering.Cameras;
public class NullCamera : ICamera
{
    public Matrix4x4 ViewMatrix => Matrix4x4.Identity;

    public Matrix4x4 ProjectionMatrix => Matrix4x4.Identity;

    public Frustum Frustum => Frustum.Cube;
}
