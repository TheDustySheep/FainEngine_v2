using System.Text;
using FainEngine_v2.Rendering.Materials;
using Silk.NET.OpenGL;
using System.Numerics;
using SixLabors.Fonts;
using SixLabors.Fonts.Unicode;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace FainEngine_v2.UI.FontRendering;

public class FontAtlas
{
    public float LineHeight => _fontSize;
    private Font _font;
    private int _padding;
    private int _atlasSize;
    private float _fontSize;

    private readonly Dictionary<char, GlyphData> glyphs = new();

    public Texture2D AtlasTexture { get; private set; }

    public FontAtlas(string fontPath, float fontSize, int atlasSize=1024, int padding=2)
    {
        _padding = padding;
        _atlasSize = atlasSize;
        _fontSize = fontSize;

        _font = LoadFont(fontPath);
        AtlasTexture = LoadAtlas();
    }

    private Font LoadFont(string fontPath)
    {
        var collection = new FontCollection();
        var family = collection.Add(fontPath);
        return family.CreateFont(_fontSize);
    }

    private Texture2D LoadAtlas()
    {
        var metrics = _font.FontMetrics;
        float scale = _fontSize / metrics.UnitsPerEm;

        Texture2D tex;

        using (var img = new Image<Rgba32>(_atlasSize, _atlasSize))
        {
            img.Mutate(mtx =>
            {
                int xPos = 0;
                int yPos = 0;
                int yMax = 0;

                foreach (var c in CharSet())
                {
                    if (!metrics.TryGetGlyphMetrics(new CodePoint(c), TextAttributes.None, TextDecorations.None, LayoutMode.HorizontalTopBottom, ColorFontSupport.None, out var gmList))
                        continue;

                    var gm = gmList[0];

                    var glyph = new GlyphData
                    {
                        Character = c,
                        AdvancePx = scale * new Vector2(gm.AdvanceWidth, gm.AdvanceHeight),
                        BearingPx = scale * new Vector2(gm.LeftSideBearing, gm.BottomSideBearing),
                        BoundsPx  = scale * new Vector2(gm.Width, gm.Height),
                    };

                    int width  = (int)MathF.Ceiling(glyph.AdvancePx.X);
                    int height = (int)MathF.Ceiling(glyph.AdvancePx.Y);

                    if (xPos + width > _atlasSize)
                    {
                        xPos = 0;
                        yPos += yMax + _padding;
                        yMax = 0;
                    }

                    if (yPos + height > _atlasSize)
                    {
                        // Exceeded texture atlas size
                        break;
                    }

                    mtx.DrawText(c.ToString(), _font, Color.White, new PointF(xPos, yPos));

                    glyph.UVMin = new Vector2
                    (
                        xPos,
                        yPos
                    ) / _atlasSize;

                    glyph.UVMax = new Vector2
                    (
                        xPos + glyph.AdvancePx.X,
                        yPos + glyph.AdvancePx.Y
                    ) / _atlasSize;

                    glyph.UVMin.Y = 1f - glyph.UVMin.Y;
                    glyph.UVMax.Y = 1f - glyph.UVMax.Y;

                    xPos += width + _padding;
                    yMax = int.Max(yMax, height);

                    glyphs[c] = glyph;
                }
            });

            tex = new Texture2D(img);
        }
        return tex;
    }

    public static string CharSet()
    {
        char min = (char)32;
        char max = (char)126;
        int count = max - min + 1;

        Span<char> chars = stackalloc char[count];

        for (int i = 0; i < count; i++)
        {
            chars[i] = (char)(i + min);
        }

        return chars.ToString();
    }

    public bool TryGetGlyph(char c, out GlyphData data)
    {
        return glyphs.TryGetValue(c, out  data);
    }

    public struct GlyphData
    {
        public char Character;
        public Vector2 BearingPx;
        public Vector2 AdvancePx;
        public Vector2 BoundsPx;

        public Vector2 UVMin;
        public Vector2 UVMax;

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.AppendLine($"Character {Character}");
            sb.AppendLine($"Bearing   {BearingPx}");
            sb.AppendLine($"Advance   {AdvancePx}");
            sb.AppendLine($"Bounds    {BoundsPx}");
            sb.AppendLine($"UV Min    {UVMin}");
            sb.AppendLine($"UV Max    {UVMax}");

            return sb.ToString();
        }
    }
}