﻿using FainCraft.Resources.Shaders.PostProcessing;
using FainEngine_v2.Core;
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
            rt = new RenderTexture(GameGraphics.GL, GameGraphics.Window.Size.X, GameGraphics.Window.Size.Y);
            mesh = new CustomVertexMesh<Vertex, int>(FULL_SCREEN_VERTS, TRIANGLES);
            mat = new PostProcessMaterial(shader, rt);

            GameGraphics.SetPostProcess(this);
        }

        static readonly Vertex[] FULL_SCREEN_VERTS = new Vertex[]
        {
            new() { Position = new Vector2(-1.0f,-1.0f), TexCoord = new Vector2(0.0f, 0.0f) },
            new() { Position = new Vector2(-1.0f, 1.0f), TexCoord = new Vector2(0.0f, 1.0f) },
            new() { Position = new Vector2( 1.0f, 1.0f), TexCoord = new Vector2(1.0f, 1.0f) },
            new() { Position = new Vector2( 1.0f,-1.0f), TexCoord = new Vector2(1.0f, 0.0f) },
        };

        static readonly int[] TRIANGLES = new int[]
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
            rt.BindFBO();
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
