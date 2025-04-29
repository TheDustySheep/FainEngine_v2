using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;


namespace FainEngine_v2.UI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UIColour
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public UIColour(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static implicit operator UIColour(Color c) => new
        (
            c.R / 255f, 
            c.G / 255f, 
            c.B / 255f, 
            1f
        );

        public static implicit operator UIColour(Vector4 c) => new
        (
            c.X,
            c.Y,
            c.Z,
            c.W
        );

        public static implicit operator Vector4(UIColour c) => new
        (
            c.R,
            c.G,
            c.B,
            c.A
        );
    }
}
