using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.UI.Fonts;

namespace FainEngine_v2.UI.FontRendering;
public interface IFontAtlas
{
    Texture2D AtlasTexture { get; }
    FontMapping Glyphs { get; }
}