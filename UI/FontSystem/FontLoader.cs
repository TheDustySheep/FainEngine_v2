using FainEngine_v2.Rendering.Materials;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FainEngine_v2.UI.FontSystem;
internal class FontLoader
{
    public FontType LoadFont()
    {
        string characters = AllCharacters();

        FontCollection collection = new();
        FontFamily family = collection.Add("Resources/Fonts/Montserrat/Montserrat-Regular.ttf");
        Font font = family.CreateFont(64, FontStyle.Italic);

        TextOptions options = new(font)
        {
            FallbackFontFamilies = new[] { family },
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        FontRectangle fullMeasure = TextMeasurer.MeasureBounds(characters, options);

        Texture2D texture;
        using (Image<Rgba32> image = new Image<Rgba32>((int)fullMeasure.Width, (int)fullMeasure.Height))
        {
            image.Mutate(x => x
                .BackgroundColor(Rgba32.ParseHex("FFFFFF"))
                .DrawText(characters, font, Rgba32.ParseHex("000000"), new PointF(0, 0))
            );
            image.Save(@"C:\Users\Sean\source\repos\FainEngine_v2\Resources\Fonts\Montserrat\out.png");
            texture = new Texture2D(Core.GameGraphics.GL, image);
        }

        ReadOnlySpan<char> chars = characters.AsSpan();
        Dictionary<char, FontType.FontTypeChar> uvLookup = new();

        for (int i = 0; i < chars.Length; i++)
        {
            char c = chars[i];

            ReadOnlySpan<char> before = i == 0 ? ReadOnlySpan<char>.Empty : chars.Slice(0, i - 1);
            ReadOnlySpan<char> include = chars.Slice(0, i);

            var beforeMeasure = TextMeasurer.MeasureBounds(before, options);
            var includeMeasure = TextMeasurer.MeasureBounds(include, options);

            var charMeasureWidth = includeMeasure.Width - beforeMeasure.Width;

            var aspect = charMeasureWidth / fullMeasure.Height;
            var uvWidth = charMeasureWidth / fullMeasure.Width;
            var beforeUVWidth = beforeMeasure.Width / fullMeasure.Width;

            uvLookup[c] = new FontType.FontTypeChar()
            {
                Aspect = aspect,
                c = c,
                UV = new Rect(beforeUVWidth, 0f, uvWidth, 1f),
            };

            //Console.WriteLine($"Loaded Char: {c} UV Before: {beforeUVWidth:0.000} UV Width: {uvWidth:0.000} Aspect: {aspect:0.000}");
        }
        return new FontType(uvLookup, texture);
    }

    private static string AllCharacters()
    {
        return new string(
            Enumerable.Range(0, char.MaxValue + 1)
                      .Select(i => (char)i)
                      .Where(c => !char.IsControl(c) && char.IsAscii(c))
                      .ToArray());
    }
}
