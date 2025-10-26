using FainEngine_v2.Rendering.Materials;

namespace FainEngine_v2.UI.Fonts;
public interface IFontAtlas
{
    Texture2D AtlasTexture { get; }
    FontMapping Glyphs { get; }
}