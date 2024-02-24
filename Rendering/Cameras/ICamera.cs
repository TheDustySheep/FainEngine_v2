using FainEngine_v2.Rendering.BoundingShapes;
using System.Numerics;

namespace FainEngine_v2.Rendering.Cameras;
public interface ICamera
{
    public Matrix4x4 ViewMatrix { get; }
    public Matrix4x4 ProjectionMatrix { get; }

    public static ICamera Main { get; private set; } = new NullCamera();
    Frustum Frustum { get; }

    public static void SetMainCamera(ICamera? camera)
    {
        Main = camera ?? new NullCamera();
    }
}

public static class ICameraExtensions
{
    public static void SetMainCamera(this ICamera cam)
    {
        ICamera.SetMainCamera(cam);
    }
}
