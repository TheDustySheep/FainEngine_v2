using System.Numerics;

namespace FainEngine_v2.UI;
internal class UIElement
{
    public float Depth;
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }

    internal MeshSegment GetMeshSegment()
    {
        return new MeshSegment()
        {
            Vertices = new Vertex[]
            {
                new Vertex() { Position = new Vector3(Position.X,          Position.Y,          Depth) },
                new Vertex() { Position = new Vector3(Position.X,          Position.Y + Size.Y, Depth) },
                new Vertex() { Position = new Vector3(Position.X + Size.X, Position.Y + Size.Y, Depth) },
                new Vertex() { Position = new Vector3(Position.X + Size.X, Position.Y,          Depth) },
            },
            Triangles = new uint[] { 0, 1, 2, 2, 3, 0 }
        };
    }
}
