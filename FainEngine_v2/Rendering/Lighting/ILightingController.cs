using FainEngine_v2.Rendering.Materials;

namespace FainEngine_v2.Rendering.Lighting
{
    internal interface ILightingController
    {
        public void SetLights(Shader shader);
    }
}
