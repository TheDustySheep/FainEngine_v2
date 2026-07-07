using FainEngine_v2.Rendering.Materials;

namespace FainEngine_v2.Rendering.Sprites;
internal class SpriteMaterial : Material
{
    public SpriteMaterial(Shader shader, Texture baseTexture) : base(shader)
    {
        SetTexture(baseTexture, "albedoTexture");
    }
}
