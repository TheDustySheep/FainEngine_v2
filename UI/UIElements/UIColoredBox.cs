using System.Numerics;

namespace FainEngine_v2.UI.UIElements;
public class UIColoredBox : IUIElement
{
    public Vector4 Color { get; set; }
    public RenderFragment GetMeshSegment(Rect maxBounds, float depth)
    {
        return new RenderFragment()
        {
            Segments = new MeshSegment[]
            {
                new MeshSegment()
                {
                    Vertices = new Vertex[]
                    {
                        new Vertex(depth, new Vector2(maxBounds.X,               maxBounds.Y              ), Color),
                        new Vertex(depth, new Vector2(maxBounds.X,               maxBounds.Y + maxBounds.H), Color),
                        new Vertex(depth, new Vector2(maxBounds.X + maxBounds.W, maxBounds.Y + maxBounds.H), Color),
                        new Vertex(depth, new Vector2(maxBounds.X + maxBounds.W, maxBounds.Y              ), Color),
                    },
                    Triangles = new uint[] { 0, 1, 2, 2, 3, 0 }
                }
            }
        };
    }
}
