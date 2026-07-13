using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.Meshing;
using FainEngine_v2.Rendering.PostProcessing;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System.Numerics;

namespace FainEngine_v2.Rendering;
public interface IGameGraphics
{
    float WindowAspect { get; }

    event Action<int, int>? OnResized;

    void DrawMesh(IMesh mesh, Material mat, Matrix4x4 model);
    void OnResize(Vector2D<int> newSize);
    void Render();
    void SetPostProcess(PostProcess postProcess);
    void ThrowOnGLError(string details);
}