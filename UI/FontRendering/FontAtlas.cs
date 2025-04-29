using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Text;
using FainEngine_v2.Rendering.Materials;
using Silk.NET.OpenGL;
using System.Numerics;

namespace FainEngine_v2.UI.FontRendering;

public class FontAtlas
{
    private readonly Font font;
    private readonly int _padding;
    private readonly int _atlasX;
    private readonly int _atlasY;

    private readonly Dictionary<char, GlyphData> glyphs;
    public IReadOnlyDictionary<char, GlyphData> Glyphs => glyphs;

    public Texture2D AtlasTexture { get; private set; }

    public FontAtlas(GL gl, string fontPath, float fontSize, int width=1024, int height=1024, int padding=1)
    {
        if (!File.Exists(fontPath))
            throw new FileNotFoundException($"Font file not found: {fontPath}", fontPath);

        _padding = padding;
        _atlasX = width;
        _atlasY = height;

        font = LoadFont(fontPath, fontSize);
        glyphs = GenerateGlyphs();
        AtlasTexture = BuildTexture(gl);
    }

    private static Font LoadFont(string fontPath, float fontSize)
    {
        var collection = new FontCollection();
        var family = collection.Add(fontPath);
        return family.CreateFont(fontSize);
    }

    private Dictionary<char, GlyphData> GenerateGlyphs()
    {
        var glyphs = new Dictionary<char, GlyphData>();
        foreach (char c in GetDefaultCharacters())
        {
            var size = TextMeasurer.MeasureAdvance(c.ToString(), new TextOptions(font));
            glyphs.Add(
                c,
                new GlyphData(
                    c,
                    (int)Math.Ceiling(size.Width),
                    (int)Math.Ceiling(size.Height)
                )
            );
        }

        int x = 0, y = 0, rowHeight = 0;

        foreach (var g in glyphs.Values)
        {
            if (x + g.XSize_px > _atlasX)
            {
                x = 0;
                y += rowHeight + _padding;
                rowHeight = 0;
            }
            if (y + g.YSize_px > _atlasY)
                throw new InvalidOperationException("Atlas size too small for all glyphs.");

            g.XPos_px = x;
            g.YPox_px = y;

            x += g.XSize_px + _padding;
            rowHeight = Math.Max(rowHeight, g.YSize_px);
        }

        foreach (var g in glyphs.Values)
        {
            g.UVMin = new Vector2
            (
                (float)g.XPos_px / _atlasX,
                1 - ((float)g.YPox_px / _atlasY)
            );

            g.UVMax = new Vector2
            (
                g.UVMin.X + ((float)g.XSize_px / _atlasX),
                g.UVMin.Y - ((float)g.YSize_px / _atlasY)
            );
        }

        return glyphs;
    }

    private Texture2D BuildTexture(GL gl)
    {
        using var image = new Image<Rgba32>(_atlasX, _atlasY);
        image.Mutate(ctx =>
        {
            foreach (var g in glyphs.Values)
            {
                ctx.DrawText(
                    g.Character.ToString(),
                    font,
                    Color.White,
                    new PointF(g.XPos_px, g.YPox_px));
            }
        });

        return new Texture2D(gl, image);
    }

    private static string GetDefaultCharacters()
    {
        var sb = new StringBuilder();
        for (int i = 32; i < 127; i++)
            sb.Append((char)i);
        return sb.ToString();
    }

    public class GlyphData
    {
        public char Character { get; }
        internal int XPos_px { get; set; }
        internal int YPox_px { get; set; }
        internal int XSize_px { get; }
        internal int YSize_px { get; }

        public Vector2 Size_px => new Vector2(XSize_px, YSize_px);

        public Vector2 UVMin { get; set; }
        public Vector2 UVMax { get; set; }

        public GlyphData(char character, int width, int height)
        {
            Character = character;
            XSize_px = width;
            YSize_px = height;
        }
    }
}