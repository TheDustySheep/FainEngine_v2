using FainEngine_v2.Rendering.Meshing;
using System.Numerics;

namespace FainEngine_v2.Rendering;

internal struct RenderInstance
{
    public required IMesh Mesh;
    public required Matrix4x4 ModelMatrix;
}