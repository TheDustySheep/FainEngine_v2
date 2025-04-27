using FainEngine_v2.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Materials;
using NuklearDotNet;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;

namespace FainEngine_v2.UI.Obsolete;
public class UICanvas : IEntity
{
    readonly UIMesh uiMesh;
    readonly Material material;
    Matrix4x4 model = Matrix4x4.Identity;

    public UICanvas()
    {
        //FontLoader fontLoader = new FontLoader();
        //var font = fontLoader.LoadFont();

        //uiMesh = new UIMesh();

        //material = new UIMaterial(ResourceLoader.LoadShader("Resources/UI"), font.Texture);

    }

    public void Update()
    {

    }
}

unsafe class SilkDevice : NuklearDeviceTex<Texture2D>
{
    GL _gl;
    NkVertex[] Verts;
    ushort[] Inds;

    public SilkDevice(GL gL)
    {
        _gl = gL;
        Verts = new NkVertex[0];
        Inds = new ushort[0];
    }

    public override Texture2D CreateTexture(int W, int H, nint Data)
    {
        Image<Rgba32> img = new Image<Rgba32>(W, H);

        return new Texture2D(GameGraphics.GL, img);
    }
    public override void SetBuffer(NkVertex[] VertexBuffer, ushort[] IndexBuffer)
    {
        Verts = VertexBuffer;
        Inds = IndexBuffer;
    }

    public override void Render(NkHandle Userdata, Texture2D Texture, NkRect ClipRect, uint Offset, uint Count)
    {
        Vertex[] SilkVerts = new Vertex[Count];

        for (int i = 0; i < Count; i++)
        {
            NkVertex V = Verts[Inds[Offset + i]];
            SilkVerts[i] = new Vertex(new Vector2(V.Position.X, V.Position.Y), new Vector4(V.Color.R, V.Color.G, V.Color.B, V.Color.A), new Vector2(V.UV.X, V.UV.Y));
        }

        Texture.Bind();
        _gl.Enable(EnableCap.ScissorTest);
        _gl.Scissor((int)ClipRect.X, (int)ClipRect.Y, (uint)ClipRect.W, (uint)ClipRect.H);

        //RWind.Draw(SfmlVerts, PrimitiveType.Triangles);
        //RT.Draw(SfmlVerts, PrimitiveType.Triangles);

        _gl.Disable(EnableCap.ScissorTest);
    }
}

