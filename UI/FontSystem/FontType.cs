using FainEngine_v2.Rendering.Materials;

namespace FainEngine_v2.UI.FontSystem;
public class FontType
{
    public Dictionary<char, FontTypeChar> UVLookup;
    public Texture2D Texture;

    public FontType(Dictionary<char, FontTypeChar> uVLookup, Texture2D texture)
    {
        UVLookup = uVLookup;
        Texture = texture;
    }

    public FontTypeChar GetCharacter(char c)
    {
        UVLookup.TryGetValue(c, out FontTypeChar rect);
        return rect;
    }

    public struct FontTypeChar
    {
        public char c;
        public Rect UV;
        public float Aspect;
    }
}
