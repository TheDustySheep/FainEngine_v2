using Silk.NET.Maths;
using System.Numerics;

namespace FainEngine_v2.Extensions;
public static class Matrix4x4Extensions
{
    public static Matrix4x4 CreateTRS(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        return
            Matrix4x4.CreateScale(scale) *
            Matrix4x4.CreateFromQuaternion(rotation) *
            Matrix4x4.CreateTranslation(position);
    }

    public static Matrix4x4 Inverse(this Matrix4x4 mat)
    {
        Matrix4x4.Invert(mat, out mat);
        return mat;
    }

    public static Matrix4x4 Transpose(this Matrix4x4 mat)
    {
        return Matrix4x4.Transpose(mat);
    }
}
