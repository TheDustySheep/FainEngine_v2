using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Materials;

public abstract class Texture : IDisposable
{
    protected uint _handle;
    protected GL _gl;
    protected abstract TextureTarget Target { get; }

    public const int MAX_TEXTURE_COUNT = 32;

    protected Texture(GL gl)
    {
        _gl = gl;
        _handle = _gl.GenTexture();
        Use();
    }

    public void Use(TextureUnit textureSlot = TextureUnit.Texture0)
    {
        _gl.ActiveTexture(textureSlot);
        _gl.BindTexture(Target, _handle);
    }

    public void Use(uint textureIndex)
    {
        if (textureIndex >= MAX_TEXTURE_COUNT)
            throw new Exception($"Texture out of range {textureIndex}/{MAX_TEXTURE_COUNT}");

        var textureSlot = TextureUnit.Texture0 + (int)textureIndex;

        _gl.ActiveTexture(textureSlot);
        _gl.BindTexture(Target, _handle);
    }

    public void Dispose()
    {
        _gl.DeleteTexture(_handle);
    }
}
