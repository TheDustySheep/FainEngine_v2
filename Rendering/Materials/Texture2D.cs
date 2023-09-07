using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Materials;

public class Texture2D : Texture
{
    protected override TextureTarget Target => TextureTarget.Texture2D;

    public WrappingModes WrapMode { get; init; } = WrappingModes.Clamp_To_Edge;
    public FilterModes FilterMode { get; init; } = FilterModes.Nearest;
    public MipMapModes MipMapMode { get; init; } = MipMapModes.None;

    public unsafe Texture2D(GL gl, string path) : base(gl)
    {
        _gl = gl;
        _handle = _gl.GenTexture();
        Use();

        using (var img = Image.Load<Rgba32>(path))
        {
            gl.TexImage2D(Target, 0, InternalFormat.Rgba8, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

            img.Mutate(i => i.RotateFlip(RotateMode.Rotate180, FlipMode.Horizontal));

            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    fixed (void* data = accessor.GetRowSpan(y))
                    {
                        gl.TexSubImage2D(Target, 0, 0, y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                    }
                }
            });
        }

        SetParameters();
    }

    protected void SetParameters()
    {
        _gl.TexParameter(Target, TextureParameterName.TextureWrapS, (int)WrapMode); // X
        _gl.TexParameter(Target, TextureParameterName.TextureWrapT, (int)WrapMode); // Y
        _gl.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)FilterMode);
        _gl.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)FilterMode);
        _gl.TexParameter(Target, TextureParameterName.TextureBaseLevel, 0);
        _gl.TexParameter(Target, TextureParameterName.TextureMaxLevel, 8);
        _gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    private GLEnum GetFilterMode()
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
