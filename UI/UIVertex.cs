using System.Drawing;
using System.Numerics;

namespace FainEngine_v2.UI
{
    internal struct UIVertex
    {
        public Vector3 Position;
        public Vector4 Colour;

        public UIVertex(float x, float y, float z, Color color=default)
        {
            Position = new Vector3(x, y, z);
            Colour = new Vector4(
                color.R / 255.0f,
                color.G / 255.0f,
                color.B / 255.0f,
                color.A / 255.0f
            );
        }
    }
}
