using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FainEngine_v2.Rendering.Materials;
public class RenderTexture : Texture
{
    FrameBufferObject fbo;
    protected override TextureTarget Target => TextureTarget.Texture2D;

    public readonly int Height;
    public readonly int Width;

    public unsafe RenderTexture(GL gl, int width, int height) : base(gl, WrappingModes.Clamp_To_Edge, FilterModes.Nearest, MipMapModes.None)
    {
        Width = width;
        Height = height;

        fbo = new FrameBufferObject(gl);
        fbo.Bind();

        gl.TexImage2D(Target, 0, InternalFormat.Rgba8, (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
        SetParameters();

        Rgba32 rgba32 = Rgba32.ParseHex("#ff0000");

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                gl.TexSubImage2D(Target, 0, x, y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, rgba32);

            }
        }

        gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, Target, _handle, 0);
    }
}
