using FainEngine_v2.UI.FontRendering;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace FainEngine_v2.UI.Elements;

public class UIText : UIElement
{
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            RecalculateBounds();
        }
    }

    private string _text;
    private FontAtlas _font;

    public override float XSize { get => base.XSize; set { } }
    public override float YSize { get => base.YSize; set { } }

    public override float XSizeMax 
    { 
        get => base.XSizeMax; 
        set
        {
            base.XSizeMax = value;
            RecalculateBounds();
        }
    }
    public override float YSizeMax
    {
        get => base.YSizeMax;
        set
        {
            base.YSizeMax = value;
            RecalculateBounds();
        }
    }

    public UIText(UICanvas canvas, string text)
    {
        _text = text;
        _font = canvas.Atlas;

        TextColour = Color.Black;

         RecalculateBounds();
    }

    private void EnumerateLines(float wrapLength, Action<ReadOnlySpan<char>> onLine)
    {
        var text = _text.AsSpan();

        int lineStart = 0;
        float xPos = 0f;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (c == '\r' || c == '\n')
            {
                int length = i - lineStart;
                onLine(text.Slice(lineStart, length));

                // Handle \r\n
                if (c == '\r' && i + 1 < _text.Length && _text[i + 1] == '\n')
                    i++;

                lineStart = i + 1;
                xPos = 0f;
                continue;
            }

            // Ignore missing glyphs
            if (!_font.TryGetGlyph(c, out var g))
                continue;

            float advance = g.AdvancePx.X;

            // Wrap on overflow
            if (xPos + advance > wrapLength)
            {
                var candidate = text.Slice(lineStart, i - lineStart + 1);

                // Search for space
                int spaceIdx = candidate.LastIndexOf(' ');

                // No space in previous string
                if (spaceIdx < 0)
                {
                    onLine(candidate);
                    lineStart = i + 1;
                }
                // Space found
                else
                {
                    // Omit the space
                    onLine(candidate.Slice(0, spaceIdx));
                    lineStart += spaceIdx + 1;
                }

                // Recompute xPos
                xPos = 0f;
                for (int j = lineStart; j <= i && j < text.Length; j++)
                {
                    if (_font.TryGetGlyph(text[j], out var gj))
                        xPos += gj.AdvancePx.X;
                }
            }
            else
            {
                xPos += advance;
            }
        }

        // 4) final tail‐piece
        if (lineStart < text.Length)
            onLine.Invoke(text.Slice(lineStart, text.Length - lineStart));
    }

    private void RecalculateBounds()
    {
        Vector2 bounds = CalculateBounds(XSizeMax);

        base.XSize = bounds.X;
        base.YSize = bounds.Y;
    }

    private Vector2 CalculateBounds(float wrapLength)
    {
        var str = _text.AsSpan();

        float xMax = 0f;
        float ySum = 0f;

        EnumerateLines(XSizeMax, line =>
        {
            float xPos = 0f;
            float yMax = 0f;

            for (int i = 0; i < line.Length; i++)
            {
                if (!_font.TryGetGlyph(line[i], out var g))
                    continue;

                xPos += g.AdvancePx.X;
                yMax = MathF.Max(yMax, g.AdvancePx.Y);
            }

            xMax = MathF.Max(xMax, xPos);
            ySum += yMax;
        });

        return new Vector2(xMax, ySum);
    }

    internal override IEnumerable<UIVertex> GenerateVerts(DrawNode node)
    {
        Vector2 min = new Vector2(node.XOffset, node.YOffset);
        Vector2 max = new Vector2(node.XSize, node.YSize) + min;

        List<UIVertex> verts = new();
        float xPos = 0f;
        float yPos = 0f;

        EnumerateLines(XSizeMax, line =>
        {
            float yHeight = 0f;
            for (int i = 0; i < line.Length; i++)
            {
                if (!_font.TryGetGlyph(line[i], out var g))
                    continue;

                Vector2 gMin = new Vector2(
                    min.X + xPos,
                    min.Y + yPos
                );

                Vector2 gMax = new Vector2(
                    gMin.X + g.AdvancePx.X,
                    gMin.Y + g.AdvancePx.Y
                );

                Vector2 uvMin = g.UVMin;
                Vector2 uvMax = g.UVMax;

                verts.Add(new UIVertex(gMin.X, gMin.Y, node.ZIndex, this, new Vector2(uvMin.X, uvMin.Y)));
                verts.Add(new UIVertex(gMax.X, gMin.Y, node.ZIndex, this, new Vector2(uvMax.X, uvMin.Y)));
                verts.Add(new UIVertex(gMax.X, gMax.Y, node.ZIndex, this, new Vector2(uvMax.X, uvMax.Y)));
                verts.Add(new UIVertex(gMin.X, gMax.Y, node.ZIndex, this, new Vector2(uvMin.X, uvMax.Y)));

                xPos += g.AdvancePx.X;
                yHeight = MathF.Max(yHeight, g.AdvancePx.Y);
            }
            yPos += yHeight;
            xPos = 0f;
        });

        return verts;
    }
}
