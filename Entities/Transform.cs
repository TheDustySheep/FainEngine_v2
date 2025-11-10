using FainEngine_v2.Extensions;
using System.Numerics;

namespace FainEngine_v2.Entities;

public class Transform
{
    public Transform? Parent { get; private set; }

    public void SetParent(Transform? parent)
    {
        if (parent is null)
        {
            Parent = null;
            return;
        }

        Transform? parentNode = parent;

        while (parentNode is not null)
        {
            if (parentNode == this)
            {
                Console.WriteLine("Cannot set self as parent of self transform");
                return;
            }
            parentNode = parentNode.Parent;
        }

        Parent = parent;
    }

    public Vector3 Forward => Vector3.TransformNormal(Vector3.UnitZ, ModelMatrix);
    public Vector3 Up => Vector3.TransformNormal(Vector3.UnitY, ModelMatrix);
    public Vector3 Right => Vector3.TransformNormal(Vector3.UnitX, ModelMatrix);

    public Vector3 GlobalPosition => Vector3.Transform(Vector3.Zero, ModelMatrix);
    public Vector3 LocalPosition { get; set; } = new Vector3(0, 0, 0);
    public float Scale { get; set; } = 1f;
    public Quaternion LocalRotation { get; set; } = Quaternion.Identity;

    private Matrix4x4 RotationMatrix
    {
        get
        {
            Matrix4x4 matrix = Matrix4x4.CreateFromQuaternion(LocalRotation);

            return Parent is null ? matrix : matrix * Parent.RotationMatrix;
        }
    }

    public Matrix4x4 ModelMatrix
    {
        get
        {
            Matrix4x4 matrix = Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(LocalRotation) * Matrix4x4.CreateTranslation(LocalPosition);

            return Parent is null ? matrix : matrix * Parent.ModelMatrix;
        }
    }

    public Matrix4x4 ViewMatrix => ModelMatrix.Inverse();
}
