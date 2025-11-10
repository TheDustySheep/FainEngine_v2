using FainEngine_v2.Rendering.Materials;

namespace FainEngine_v2.UI
{
    public class UIMaterial : Material
    {
        public UIMaterial(Shader shader, Texture2D atlas) : base(shader)
        {
            SetTexture(atlas, "fontAtlas");
        }
    }
}
