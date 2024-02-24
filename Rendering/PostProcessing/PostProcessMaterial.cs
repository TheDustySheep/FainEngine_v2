using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.RenderObjects;

namespace FainCraft.Resources.Shaders.PostProcessing
{
    public class PostProcessMaterial : Material
    {
        public PostProcessMaterial(Shader shader, RenderTexture rt) : base(shader)
        {
            SetTexture(rt.Texture, "screenTexture");
        }
    }
}
