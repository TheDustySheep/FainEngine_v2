using FainEngine_v2.Core;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Materials;

public abstract class Texture : TextureObject, IDisposable
{
    public const int MAX_TEXTURE_COUNT = 32;

    public WrappingModes WrapMode { get; init; } = WrappingModes.Clamp_To_Edge;
    public FilterModes FilterMode { get; init; } = FilterModes.Nearest;
    public MipMapModes MipMapMode { get; init; } = MipMapModes.None;

    protected Texture(WrappingModes wrapMode, FilterModes filterMode, MipMapModes mipMapMode) : base()
    {
        WrapMode = wrapMode;
        FilterMode = filterMode;
        MipMapMode = mipMapMode;

        Use(0);
    }

    public void Use(uint textureIndex)
    {
        if (textureIndex >= MAX_TEXTURE_COUNT)
            throw new Exception($"Texture out of range {textureIndex}/{MAX_TEXTURE_COUNT}");
    
        var textureSlot = TextureUnit.Texture0 + (int)textureIndex;
    
        _gl.ActiveTexture(textureSlot);
        _gl.BindTexture(Target, _handle);
    }

    protected void SetParameters()
    {
        _gl.TextureParameter(_handle, TextureParameterName.TextureWrapS, (int)WrapMode);
        _gl.TextureParameter(_handle, TextureParameterName.TextureWrapT, (int)WrapMode);
        _gl.TextureParameter(_handle, TextureParameterName.TextureMinFilter, (int)FilterMode);
        _gl.TextureParameter(_handle, TextureParameterName.TextureMagFilter, (int)FilterMode);
        _gl.TextureParameter(_handle, TextureParameterName.TextureBaseLevel, 0);
        _gl.TextureParameter(_handle, TextureParameterName.TextureMaxLevel, 8);
        _gl.GenerateTextureMipmap(_handle);
    }

    protected GLEnum GetFilterMode()
    {
        return MipMapMode switch
        {
            MipMapModes.None => FilterMode switch
            {
                FilterModes.Nearest => GLEnum.Nearest,
                FilterModes.Linear => GLEnum.Linear,
                _ => throw new Exception("Tex2D Unknown Filter Mode"),
            },
            MipMapModes.Nearest => FilterMode switch
            {
                FilterModes.Nearest => GLEnum.NearestMipmapNearest,
                FilterModes.Linear => GLEnum.NearestMipmapLinear,
                _ => throw new Exception("Tex2D Unknown Filter Mode"),
            },
            MipMapModes.Linear => FilterMode switch
            {
                FilterModes.Nearest => GLEnum.LinearMipmapNearest,
                FilterModes.Linear => GLEnum.LinearMipmapLinear,
                _ => throw new Exception("Tex2D Unknown Filter Mode"),
            },
            _ => throw new Exception("Tex2D Unknown MipMap Mode"),
        };
    }

    public enum WrappingModes
    {
        Repeat = GLEnum.Repeat,
        Mirrored_Repeat = GLEnum.MirroredRepeat,
        Clamp_To_Edge = GLEnum.ClampToEdge,
        Clamp_To_Border = GLEnum.ClampToBorder,
    }

    public enum FilterModes
    {
        Nearest = GLEnum.Nearest,
        Linear = GLEnum.Linear,
    }

    public enum MipMapModes
    {
        None,
        Nearest,
        Linear,
    }
}
