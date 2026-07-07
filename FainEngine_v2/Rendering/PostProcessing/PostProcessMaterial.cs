using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.RenderObjects;

namespace FainEngine_v2.Rendering.PostProcessing
{
    public class PostProcessMaterial : Material
    {
        public PostProcessMaterial(Shader shader, RenderTexture rt) : base(shader)
        {
            SetTexture(rt.ColorTexture, "colorTexture");
            SetTexture(rt.DepthTexture, "depthTexture");
        }
    }
}
