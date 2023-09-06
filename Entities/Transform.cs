using FainEngine_v2.Extensions;
using System.Numerics;

namespace FainEngine_v2.Core.GameObjects;

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

    public Vector3 Forward => Vector3.TransformNormal(Vector3.UnitZ, Matrix4x4.CreateFromQuaternion(Rotation));
    public Vector3 Up => Vector3.TransformNormal(Vector3.UnitY, Matrix4x4.CreateFromQuaternion(Rotation));
    public Vector3 Right => Vector3.TransformNormal(Vector3.UnitX, Matrix4x4.CreateFromQuaternion(Rotation));

    public Vector3 Position { get; set; } = new Vector3(0, 0, 0);
    public float Scale { get; set; } = 1f;
    public Quaternion Rotation { get; set; } = Quaternion.Identity;

    public Matrix4x4 ModelMatrix
    {
        get
        {
            Matrix4x4 matrix = Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position);

            return Parent is null ? matrix : matrix * Parent.ModelMatrix;
        }
    }

    public Matrix4x4 ViewMatrix => ModelMatrix.Inverse();
}
