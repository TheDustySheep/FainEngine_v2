using FainEngine_v2.Core.GameObjects;
using FainEngine_v2.Utils;
using System.Numerics;

namespace FainEngine_v2.Rendering.Cameras;
public class Camera3D : ICamera
{
    readonly Transform transform;

    public Camera3D(Transform transform)
    {
        this.transform = transform;
    }

    public Matrix4x4 ViewMatrix => transform.ViewMatrix;

    private Matrix4x4 projectionMatrix = Matrix4x4.Identity;
    public Matrix4x4 ProjectionMatrix => projectionMatrix;

    public float FOV { get; set; } = 80f;
    public float Z_Near { get; set; } = 0.1f;
    public float Z_Far { get; set; } = 10_000f;

    public void Update()
    {
        UpdateMatrix();
    }

    private void UpdateMatrix()
    {
        projectionMatrix = Matrix4x4.CreateScale(1, 1, -1) * Matrix4x4.CreatePerspectiveFieldOfView(MathUtils.DegreesToRadians(FOV), ICamera.WindowAspect, Z_Near, Z_Far);
    }
}
