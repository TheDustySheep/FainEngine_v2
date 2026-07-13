using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.BoundingShapes;
using FainEngine_v2.Utils;
using System.Numerics;
using Plane = FainEngine_v2.Rendering.BoundingShapes.Plane;

namespace FainEngine_v2.Rendering.Cameras;
public class Camera3D : ICamera
{
    readonly Transform _transform;
    readonly IGameGraphics _gameGraphics;

    public Camera3D(Transform transform)
    {
        _transform = transform;
        _gameGraphics = DependencyInjector.Resolve<IGameGraphics>();
    }

    public Matrix4x4 ViewMatrix => _transform.ViewMatrix;

    private Matrix4x4 projectionMatrix = Matrix4x4.Identity;
    public Matrix4x4 ProjectionMatrix => projectionMatrix;

    public float FOV { get; set; } = 80f;
    public float Z_Near { get; set; } = 0.1f;
    public float Z_Far { get; set; } = 1_024f;

    public Frustum Frustum
    {
        get
        {
            float fovY = MathUtils.DegreesToRadians(FOV);
            float zFar = Z_Far;
            float zNear = Z_Near;
            float aspect = _gameGraphics.WindowAspect;

            float halfVSide = zFar * MathF.Tan(fovY * .5f);
            float halfHSide = halfVSide * aspect;
            Vector3 frontMultFar = zFar * _transform.Forward;

            Vector3 globalPos = _transform.GlobalPosition;
            Vector3 forward = _transform.Forward;
            Vector3 right = _transform.Right;
            Vector3 up = _transform.Up;

            Frustum frustum = new Frustum
            {
                NearPlane = new Plane(globalPos + zNear * forward, forward),
                FarPlane = new Plane(globalPos + frontMultFar, -forward),
                RightPlane = new Plane(globalPos, Vector3.Cross(frontMultFar + right * halfHSide, up)),
                LeftPlane = new Plane(globalPos, Vector3.Cross(up, frontMultFar - right * halfHSide)),
                TopPlane = new Plane(globalPos, Vector3.Cross(right, frontMultFar + up * halfVSide)),
                BottomPlane = new Plane(globalPos, Vector3.Cross(frontMultFar - up * halfVSide, right))
            };

            return frustum;
        }
    }

    public void Update()
    {
        UpdateMatrix();
    }

    private void UpdateMatrix()
    {
        projectionMatrix = Matrix4x4.CreateScale(1, 1, -1) * Matrix4x4.CreatePerspectiveFieldOfView(MathUtils.DegreesToRadians(FOV), _gameGraphics.WindowAspect, Z_Near, Z_Far);
    }
}
