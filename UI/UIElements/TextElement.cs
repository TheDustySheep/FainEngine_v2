using System.Numerics;
using FainEngine_v2.Core;
using FainEngine_v2.UI.FontSystem;

namespace FainEngine_v2.UI.UIElements;

public class TextElement : IUIElement
{
    FontType font;
    string text = "Hello world";

    public TextElement(FontType font)
    {
        this.font = font;
    }

    public Vector4 Color { get; set; }
    public RenderFragment GetMeshSegment(Rect maxBounds, float depth)
    {
        //Face face = new Face(library, "./myfont.ttf");

        maxBounds.H = 0.1f;

        return new RenderFragment()
        {
            Segments = GetCharacters(maxBounds, depth)
        };
    }

    private List<MeshSegment> GetCharacters(Rect maxBounds, float depth)
    {
        List<MeshSegment> segments = new();

        float xOffset = maxBounds.X;
        foreach (char c in text)
        {
            var fontChar = font.GetCharacter(c);
            Rect bounds = new Rect(xOffset, maxBounds.Y, fontChar.Aspect * maxBounds.H, maxBounds.H);

            Console.WriteLine($"Char {c} Aspect {fontChar.Aspect * maxBounds.H}");

            var uvBounds = fontChar.UV;
            //uvBounds.X += MathF.Abs(MathF.Sin(GameTime.TotalTime));

            segments.Add(GetSegment(bounds, uvBounds, depth));

            xOffset += bounds.Width;
        }

        return segments;
    }

    private MeshSegment GetSegment(Rect bounds, Rect uv, float depth)
    {
        return new MeshSegment()
        {
            Vertices = new Vertex[]
            {
                new Vertex(depth, new Vector2(bounds.X,            bounds.Y           ), new Vector2(uv.X,        uv.Y       )),
                new Vertex(depth, new Vector2(bounds.X,            bounds.Y + bounds.H), new Vector2(uv.X,        uv.Y + uv.H)),
                new Vertex(depth, new Vector2(bounds.X + bounds.W, bounds.Y + bounds.H), new Vector2(uv.X + uv.W, uv.Y + uv.H)),
                new Vertex(depth, new Vector2(bounds.X + bounds.W, bounds.Y           ), new Vector2(uv.X + uv.W, uv.Y       )),
            },
            Triangles = new uint[] { 0, 1, 2, 2, 3, 0 }
        };
    }
}
