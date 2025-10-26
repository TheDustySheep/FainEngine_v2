using FainEngine_v2.Resources;
using FainEngine_v2.UI.Fonts;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FainEngine_v2.UI.FontRendering;

public class FontController : IFontController
{
    public FontController()
    {
        IFontController.Instance = this;
    }

    private Dictionary<string, string> _filePaths = new Dictionary<string, string>()
    {
        { "default", @"Resources/Fonts/Tinos/Tinos-Regular.ttf" },
        { "tinos-regular", @"Resources/Fonts/Tinos/Tinos-Regular.ttf" }
    };

    private Dictionary<(string, int), FontAtlas> _fonts = new();
    public UIMaterial? UIMaterial = null;

    public bool RequestFontAtlas(string fontName, int fontSize, [NotNullWhen(true)] out FontAtlas? atlas)
    {
        // Search for already loaded font
        if (_fonts.TryGetValue((fontName.ToLower(), fontSize), out atlas))
            return true;

        // Check if the name has a valid filepath
        if (!_filePaths.TryGetValue(fontName.ToLower(), out var filePath))
        {
            Debug.WriteLine($"Font File not Found: {fontName}");
            return false;
        }
        
        // Load a new atlas
        atlas = new FontAtlas(filePath, fontSize);
        _fonts[(fontName.ToLower(), fontSize)] = atlas;
        UIMaterial = new UIMaterial(ResourceLoader.LoadShader(@"Resources/UI"), atlas.AtlasTexture);
        return true;
    }

    public bool RequestFontMapping(string fontName, int fontSize, [NotNullWhen(true)] out FontMapping? mapping)
    {
        if (RequestFontAtlas(fontName.ToLower(), fontSize, out var atlas))
        {
            mapping = atlas.Glyphs;
            return true;
        }

        mapping = null;
        return false;
    }
}
