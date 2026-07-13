using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.Meshing;
using FainEngine_v2.Rendering.RenderObjects;
using FainEngine_v2.Utils;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;
using Shader = FainEngine_v2.Rendering.Materials.Shader;

namespace FainEngine_v2.Rendering.PostProcessing
{
    public class PostProcess : IEntity
    {
        public RenderTexture rt;
        readonly AMesh<Vertex, int> _mesh;
        readonly Material _material;

        readonly IGameGraphics _graphics;
        readonly IWindow _window;
        readonly GL _GL;

        public PostProcess(Shader shader)
        {
            _GL = DependencyInjector.Resolve<GL>();
            _window = DependencyInjector.Resolve<IWindow>();
            _graphics = DependencyInjector.Resolve<IGameGraphics>();

            rt = new RenderTexture(_window.FramebufferSize.X, _window.FramebufferSize.Y);
            _mesh = new AMesh<Vertex, int>();
            _mesh.SetData(FULL_SCREEN_VERTS, TRIANGLES);
            _material = new PostProcessMaterial(shader, rt);

            _graphics.SetPostProcess(this);
            _graphics.OnResized += ResizeRenderTexture;
        }

        ~PostProcess()
        {
            _graphics.OnResized -= ResizeRenderTexture;
        }

        private void ResizeRenderTexture(int x, int y)
        {
            rt?.Dispose();
            rt = new RenderTexture(x, y);
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
            _GL.Disable(EnableCap.DepthTest);
            _material.Use();
            _material.SetUniforms();
            _mesh.Draw();
        }

        public void Dispose()
        {
            rt.Dispose();
            _mesh.Dispose();
            _material.Dispose();
        }
    }
}
