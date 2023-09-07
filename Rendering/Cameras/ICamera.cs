using Silk.NET.Windowing;
using System.Numerics;

namespace FainEngine_v2.Rendering.Cameras;
public interface ICamera
{
    public Matrix4x4 ViewMatrix { get; }
    public Matrix4x4 ProjectionMatrix { get; }

    public static ICamera Main { get; private set; } = new NullCamera();
    public static Vector2 WindowSize = new Vector2(100, 100);
    public static float WindowAspect => WindowSize.X / WindowSize.Y;

    public static void SetWindow(IWindow window)
    {
        WindowSize.X = window.Size.X;
        WindowSize.Y = window.Size.Y;
    }

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
