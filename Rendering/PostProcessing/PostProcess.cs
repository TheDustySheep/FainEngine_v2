using FainCraft.Resources.Shaders.PostProcessing;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.Meshing;
using FainEngine_v2.Rendering.RenderObjects;
using Silk.NET.OpenGL;
using System.Numerics;
using Shader = FainEngine_v2.Rendering.Materials.Shader;

namespace FainEngine_v2.Rendering.PostProcessing
{
    public class PostProcess : IEntity
    {
        RenderTexture rt;
        CustomVertexMesh<Vertex, int> mesh;
        Material mat;

        public PostProcess(Shader shader)
        {
            rt = new RenderTexture(GameGraphics.GL, GameGraphics.Window.FramebufferSize.X, GameGraphics.Window.FramebufferSize.Y);
            mesh = new CustomVertexMesh<Vertex, int>(FULL_SCREEN_VERTS, TRIANGLES);
            mat = new PostProcessMaterial(shader, rt);

            GameGraphics.SetPostProcess(this);

            GameGraphics.OnResized += ResizeRenderTexture;
        }

        ~PostProcess()
        {
            GameGraphics.OnResized -= ResizeRenderTexture;
        }

        private void ResizeRenderTexture(int x, int y)
        {
            rt?.Dispose();
            rt = new RenderTexture(GameGraphics.GL, x, y);
        }

        static readonly Vertex[] FULL_SCREEN_VERTS = new Vertex[]
        {
            new() { Position = new Vector2(-1.0f,-1.0f), TexCoord = new Vector2(0.0f, 0.0f) },
            new() { Position = new Vector2(-1.0f, 1.0f), TexCoord = new Vector2(0.0f, 1.0f) },
            new() { Position = new Vector2( 1.0f, 1.0f), TexCoord = new Vector2(1.0f, 1.0f) },
            new() { Position = new Vector2( 1.0f,-1.0f), TexCoord = new Vector2(1.0f, 0.0f) },
        };

        static readonly int[] TRIANGLES =
        {
            0, 1, 2,
            2, 3, 0,
        };

        private struct Vertex
        {
            public Vector2 Position;
            public Vector2 TexCoord;
        }

        internal void Bind()
        {
            rt.Bind();
        }

        internal void Draw()
        {
            GameGraphics.GL.Disable(EnableCap.DepthTest);
            mat.Use();
            mat.SetUniforms();
            mesh.Bind();
            mesh.Draw();
        }
    }
}
