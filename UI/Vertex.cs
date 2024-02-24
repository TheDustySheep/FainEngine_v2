using System.Numerics;

namespace FainEngine_v2.UI;
public struct Vertex
{
    public Vector2 Position;
    public Vector4 Color;
    public Vector2 TexCoords;

    public Vertex(Vector2 position, Vector4 color, Vector2 texCoords)
    {
        Position = position;
        Color = color;
        TexCoords = texCoords;
    }

    public Vertex(Vector2 position, Color color, Vector2 texCoords)
    {
        Position = position;
        Color = (Vector4)color;
        TexCoords = texCoords;
    }
}
