using System.Numerics;

namespace FainEngine_v2.UI;
public struct Vertex
{
    public Vector2 Position;
    public float Depth;
    public Vector2 TextUVCoords;
    public Vector4 Color;

    public Vertex(float depth, Vector2 position, Vector2 textUVCoords)
    {
        Position = position;
        Depth = depth;
        TextUVCoords = textUVCoords;
    }

    public Vertex(float depth, Vector2 position, Vector4 color)
    {
        Position = position;
        Depth = depth;
        Color = color;
    }
}
