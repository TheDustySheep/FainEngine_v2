using FainEngine_v2.Resources;
using FainEngine_v2.UI.FontRendering;
using FainEngine_v2.UI.Fonts;
using FainEngine_v2.UI.UIElements.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.UI.Drawing;
public class UIMaterialManager
{
    private Dictionary<FontKey, FontData> _fonts = new();

    private FontController _fontController;

    public UIMaterialManager(FontController fontController)
    {
        _fontController = fontController;
    }

    public UIMaterial RequestMaterial(string fontName, int fontSize)
    {
        var key = new FontKey() { FontName = fontName, FontSize = fontSize };

        if (_fonts.TryGetValue(key, out var fontData))
            return fontData.Material;

        if (!_fontController.RequestFontAtlas(fontName, fontSize, out var atlas))
            throw new Exception($"Could not load requested font: {fontName} {fontSize} px");

        var material = new UIMaterial(ResourceLoader.LoadShader(@"Resources/UI"), atlas.AtlasTexture);
        _fonts[key] = new FontData()
        {
            Material = material,
            Atlas = atlas
        };

        return material;
    }

    private struct FontKey
    {
        public string FontName;
        public int FontSize;
    }

    private class FontData
    {
        public required UIMaterial Material;
        public required FontAtlas Atlas;
    }
}
