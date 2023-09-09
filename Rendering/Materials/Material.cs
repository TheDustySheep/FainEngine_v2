using FainEngine_v2.Core;
using System.Numerics;

namespace FainEngine_v2.Rendering.Materials;

public class Material
{
    protected Shader shader;

    protected readonly TextureSlot?[] Textures = new TextureSlot?[Texture.MAX_TEXTURE_COUNT];

    public Material(Shader shader)
    {
        this.shader = shader;
    }

    public void Use()
    {
        shader.Use();
    }

    public void SetViewMatrix(Matrix4x4 mat)
    {
        shader.SetUniform("uView", mat);
    }

    public void SetProjectionMatrix(Matrix4x4 mat)
    {
        shader.SetUniform("uProjection", mat);
    }

    public void SetModelMatrix(Matrix4x4 mat)
    {
        shader.SetUniform("uModel", mat);
    }

    public void UpdateAdditionalUniforms()
    {
        shader.SetUniform("time", GameTime.TotalTime);
    }

    public void SetUniforms()
    {
        Use();

        for (uint i = 0; i < Texture.MAX_TEXTURE_COUNT; i++)
        {
            var texSlot = Textures[i];

            if (texSlot is null)
                continue;

            texSlot.Texture.Use();
        }

        SetAdditionalUniforms();
    }

    protected virtual void SetAdditionalUniforms() { }

    protected void SetTexture(Texture texture, string textureName)
    {
        int index = FindTextureIndex(textureName);
        Textures[index] = new TextureSlot() { Texture = texture, Name = textureName };
        shader.SetUniform(textureName, index);
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

    protected class TextureSlot
    {
        public required Texture Texture;
        public required string Name;
    }
}
