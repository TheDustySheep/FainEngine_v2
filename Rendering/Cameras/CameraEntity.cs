using FainEngine_v2.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.Extensions;
using FainEngine_v2.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace FainEngine_v2.Rendering.Cameras;

public class CameraEntity : GameObject, ICamera, IEntity
{
    private Matrix4x4 viewMatrix = Matrix4x4.Identity;
    public Matrix4x4 ViewMatrix => viewMatrix;

    private Matrix4x4 projectionMatrix = Matrix4x4.Identity;
    public Matrix4x4 ProjectionMatrix => projectionMatrix;

    //Setup the camera's location, directions, and movement speed
    private Vector3 CameraPosition = new Vector3(-1.0f, 36.0f,-1.0f);
    private Vector3 CameraFront = new Vector3(0.0f, 0.0f, -1.0f);
    private Vector3 CameraUp = Vector3.UnitY;
    private Vector3 CameraDirection = Vector3.Zero;
    private float CameraYaw = 0f;
    private float CameraPitch = 0f;
    private float CameraZoom = 45f;

    //Used to track change in mouse movement to allow for moving of the Camera
    private static Vector2 LastMousePosition;

    public void Update()
    {
        UpdatePosition();
        UpdateRotation();
        UpdateZoom();
        UpdateMatrix();
    }

    private void UpdateMatrix()
    {
        viewMatrix = Matrix4x4.CreateLookAt(CameraPosition, CameraPosition + CameraFront, CameraUp);
        projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(CameraZoom), ICamera.WindowAspect, 0.1f, 10_000.0f);
    }

    private void UpdatePosition()
    {
        Vector3 targetDelta = Vector3.Zero;
        var moveSpeed = 10f * GameTime.DeltaTime;

        if (GameInputs.IsKeyHeld(Key.W))
        {
            //Move forwards
            targetDelta += CameraFront;
        }
        if (GameInputs.IsKeyHeld(Key.S))
        {
            //Move backwards
            targetDelta -= CameraFront;
        }
        if (GameInputs.IsKeyHeld(Key.A))
        {
            //Move left
            targetDelta -= Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp));
        }
        if (GameInputs.IsKeyHeld(Key.D))
        {
            //Move right
            targetDelta += Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp));
        }

        targetDelta.Y = 0f;
        if (targetDelta != default)
        {
            targetDelta = targetDelta.Normalize();
        }

        if (GameInputs.IsKeyHeld(Key.Space))
        {
            targetDelta += Vector3.UnitY;
        }
        if (GameInputs.IsKeyHeld(Key.ControlLeft))
        {
            targetDelta -= Vector3.UnitY;
        }

        CameraPosition += targetDelta * moveSpeed;

        if (GameInputs.IsKeyDown(Key.Escape))
            GameInputs.ExitProgram();
    }

    private void UpdateRotation()
    {
        var position = GameInputs.MousePosition;

        var lookSensitivity = 0.1f;
        if (LastMousePosition == default)
        {
            LastMousePosition = position;
        }
        else
        {
            var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
            var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
            LastMousePosition = position;

            CameraYaw += xOffset;
            CameraPitch -= yOffset;

            //We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
            CameraPitch = Math.Clamp(CameraPitch, -89.0f, 89.0f);

            CameraDirection.X = MathF.Cos(MathHelper.DegreesToRadians(CameraYaw)) * MathF.Cos(MathHelper.DegreesToRadians(CameraPitch));
            CameraDirection.Y = MathF.Sin(MathHelper.DegreesToRadians(CameraPitch));
            CameraDirection.Z = MathF.Sin(MathHelper.DegreesToRadians(CameraYaw)) * MathF.Cos(MathHelper.DegreesToRadians(CameraPitch));
            CameraFront = Vector3.Normalize(CameraDirection);
        }
    }

    private void UpdateZoom()
    {
        //We don't want to be able to zoom in too close or too far away so clamp to these values
        CameraZoom = Math.Clamp(CameraZoom - GameInputs.ScrollDelta.Y, 1.0f, 45f);
    }
}
