using System.Numerics;

namespace FainEngine_v2.Rendering.Meshing;

public struct Vertex
{
    public Vector3 Position;
    [VertexSettings(Normalized = true)]
    public Vector3 Normal;
    [VertexSettings(Normalized = true)]
    public Vector3 Tangent;
    public Vector2 TexCoords;
    public Vector3 Bitangent;
}
