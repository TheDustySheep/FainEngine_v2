using FainEngine_v2.Core;
using FainEngine_v2.Rendering.Materials;

namespace FainEngine_v2.Rendering.Lighting
{
    internal class LightingController : ILightingController
    {
        DirectionalLight Light = DirectionalLight.DefaultSun;

        public void SetLights(Shader shader)
        {
            Light.ApplyToShader(shader);
        }
    }
}
