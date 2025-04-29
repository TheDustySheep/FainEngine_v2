using FainEngine_v2.UI.FontRendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.UI.Elements
{
    public class UIText : UIElement
    {
        private string _text;
        private FontAtlas _font;

        public UIText(UICanvas canvas, string text)
        {
            _text = text;
            _font = canvas.Atlas;

            TextColour = Color.Black;

            ReadOnlySpan<char> characters = _text.AsSpan();
            for (int i = 0; i < characters.Length; i++)
            {
                char c = characters[i];
                var g = _font.Glyphs[c];

                YSize = MathF.Max(g.YSize_px, YSize);
                XSize += g.XSize_px;
            }
        }

        internal override IEnumerable<UIVertex> GenerateVerts(DrawNode node, Vector2 invScreenSize)
        {
            Vector2 min = new Vector2(node.XOffset, node.YOffset) * invScreenSize;
            Vector2 max = (new Vector2(node.XSize, node.YSize) + min) * invScreenSize;

            ReadOnlySpan<char> characters = _text.AsSpan();

            List<UIVertex> verts = new();
            float advance = 0f;
            for (int i = 0; i < characters.Length; i++)
            {
                char c = characters[i];
                var g = _font.Glyphs[c];

                Vector2 uvMin = g.UVMin;
                Vector2 uvMax = g.UVMax;

                Vector2 gSize = g.Size_px * invScreenSize;

                verts.Add(new UIVertex(advance + ((min.X          ) * 2 - 1), (1 - max.Y) * 2 - 1, node.ZIndex * 0.0001f, this, new Vector2(uvMin.X, uvMax.Y)));
                verts.Add(new UIVertex(advance + ((min.X          ) * 2 - 1), (1 - min.Y) * 2 - 1, node.ZIndex * 0.0001f, this, new Vector2(uvMin.X, uvMin.Y)));
                verts.Add(new UIVertex(advance + ((min.X + gSize.X) * 2 - 1), (1 - min.Y) * 2 - 1, node.ZIndex * 0.0001f, this, new Vector2(uvMax.X, uvMin.Y)));
                verts.Add(new UIVertex(advance + ((min.X + gSize.X) * 2 - 1), (1 - max.Y) * 2 - 1, node.ZIndex * 0.0001f, this, new Vector2(uvMax.X, uvMax.Y)));

                advance += gSize.X;
            }
            return verts;
        }
    }
}
