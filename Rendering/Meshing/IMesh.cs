using FainEngine_v2.Core;
using FainEngine_v2.Rendering.BoundingShapes;
using Silk.NET.OpenGL;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FainEngine_v2.Rendering.Meshing;

public interface IMesh : IDisposable
{
    public bool ClipBounds { get; set; }
    public BoundingBox Bounds { get; }
    public void Draw();
}