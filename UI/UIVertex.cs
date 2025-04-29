using FainEngine_v2.UI.Elements;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FainEngine_v2.UI
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct UIVertex
    {
        public Vector3 Position;
        public Vector4 Colour;

        public Vector2  TextUV;
        public Vector4 TextColour;

        public UIVertex(float x, float y, float z, UIElement element, Vector2 textUV=default)
        {
            Position = new Vector3(x, y, z);

            Colour     = element.BackgroundColour;
            TextColour = element.TextColour;

            TextUV = textUV;
        }
    }
}
