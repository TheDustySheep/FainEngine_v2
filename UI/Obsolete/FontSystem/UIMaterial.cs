using FainEngine_v2.Rendering.Materials;

namespace FainEngine_v2.UI.Obsolete.FontSystem;
internal class UIMaterial : Material
{
    public UIMaterial(Shader shader, Texture fontTexture) : base(shader)
    {
        SetTexture(fontTexture, "textAtlas");
    }
}
