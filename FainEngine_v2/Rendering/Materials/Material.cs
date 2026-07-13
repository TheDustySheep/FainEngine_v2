using FainEngine_v2.Core;
using FainEngine_v2.Extensions;
using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.Rendering.Lighting;
using FainEngine_v2.Rendering.RenderObjects;
using FainEngine_v2.Utils;
using Silk.NET.Windowing;
using System.Numerics;

namespace FainEngine_v2.Rendering.Materials;

public class Material
{
    protected Shader _shader;

    protected readonly TextureSlot?[] Textures = new TextureSlot?[Texture.MAX_TEXTURE_COUNT];

    public RenderPass RenderPass = RenderPass.Opaque;

    private readonly IGameTime _gameTime;
    private readonly IGameGraphics _graphics;
    private readonly IWindow _window;

    public Material(Shader shader)
    {
        _shader = shader;
        _window   = DependencyInjector.Resolve<IWindow>();
        _gameTime = DependencyInjector.Resolve<IGameTime>();
        _graphics = DependencyInjector.Resolve<IGameGraphics>();
    }

    public void Use()
    {
        _shader.Use();
    }

    public void SetViewMatrix(Matrix4x4 mat, ICamera cam)
    {
        _shader.SetUniform("uView", mat);
        _shader.SetUniform("viewPos", mat.Inverse().Translation);
        _shader.SetUniform("screenSize", (Vector2)_window.Size);
        _shader.SetUniform("cam_near", cam.Z_Near);
        _shader.SetUniform("cam_far", cam.Z_Far);
    }

    public void SetProjectionMatrix(Matrix4x4 mat)
    {
        _shader.SetUniform("uProjection", mat);
    }

    public void SetModelMatrix(Matrix4x4 mat)
    {
        _shader.SetUniform("uModel", mat);
    }

    public virtual void SetRenderTexture(RenderTexture texture) { }

    public void SetUniforms()
    {
        Use();

        for (uint i = 0; i < Texture.MAX_TEXTURE_COUNT; i++)
        {
            var texSlot = Textures[i];

            if (texSlot is null)
                continue;

            texSlot.Texture.Use(i);
        }

        _shader.SetUniform("time", _gameTime.TotalTime);
        SetAdditionalUniforms();
    }

    internal void SetLighting(ILightingController lightingController)
    {
        lightingController.SetLights(_shader);
    }

    protected virtual void SetAdditionalUniforms() { }

    protected void SetTexture(Texture texture, string textureName)
    {
        Use();
        int index = FindTextureIndex(textureName);
        Textures[index] = new TextureSlot() { Texture = texture, Name = textureName };
        _shader.SetUniform(textureName, index);
    }

    private int FindTextureIndex(string textureName)
    {
        // Search for texture with same name
        for (int i = 0; i < Texture.MAX_TEXTURE_COUNT; i++)
        {
            var texSlot = Textures[i];
            if (texSlot is not null && texSlot.Name == textureName)
            {
                return i;
            }
        }

        // Find an empty texture
        for (int i = 0; i < Texture.MAX_TEXTURE_COUNT; i++)
        {
            if (Textures[i] is null)
            {
                return i;
            }
        }

        throw new IndexOutOfRangeException($"Texture limit of {Texture.MAX_TEXTURE_COUNT} exceeded");
    }

    public void Dispose()
    {
        _shader.Dispose();
    }
    
    protected class TextureSlot
    {
        public required Texture Texture;
        public required string Name;
    }
}
