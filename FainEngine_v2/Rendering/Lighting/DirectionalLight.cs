using FainEngine_v2.Extensions;
using FainEngine_v2.Rendering.Materials;
using System.Numerics;

namespace FainEngine_v2.Rendering.Lighting
{
    public struct DirectionalLight
    {
        public Vector3 Direction;
        public Vector3 Ambient;
        public Vector3 Diffuse;
        public Vector3 Specular;
        public float Intensity;

        // Helper function to send to shader
        public void ApplyToShader(Shader shader)
        {
            shader.SetUniform($"light.direction", Direction);
            shader.SetUniform($"light.ambient", Ambient);
            shader.SetUniform($"light.diffuse", Diffuse);
            shader.SetUniform($"light.specular", Specular);
            shader.SetUniform($"light.intensity", Intensity);
        }

        public static DirectionalLight DefaultSun = new DirectionalLight
        {
            Direction = new Vector3(-0.30f, -1.00f, -0.50f).Normalized(),
            Ambient = Vector3.One * 0.8f,
            Diffuse = Vector3.One * 0.2f,
            Specular = Vector3.One * 0.1f,
            Intensity = 1.0f
        };
    }
}
