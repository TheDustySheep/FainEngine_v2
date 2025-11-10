using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.UI.Core;
using System.Numerics;

namespace FainEngine_v2.UI.Drawing;
public class UILayer
{
    public UIMaterial Material { get; }
    private UIMesh _mesh = new();

    readonly List<UIVertex> drawVerts = new();
    readonly List<uint> drawIndices = new();

    public UILayer(UIMaterial material)
    {
        Material = material;
    }

    public int VertexCount => drawVerts.Count;

    public void AddElementToMesh(Span<UIVertex> verts)
    {
        uint vertCount = (uint)drawVerts.Count;

        drawVerts.AddRange(verts);

        // Add each quad
        for (int i = 0; i < verts.Length / 4; i++)
        {
            drawIndices.Add(vertCount + 0);
            drawIndices.Add(vertCount + 1);
            drawIndices.Add(vertCount + 2);
            drawIndices.Add(vertCount + 2);
            drawIndices.Add(vertCount + 3);
            drawIndices.Add(vertCount + 0);
            vertCount += 4;
        }
    }

    public void Clear()
    {
        drawVerts.Clear();
        drawIndices.Clear();
        _mesh.Clear();
    }

    public void Apply()
    {
        _mesh.SetVertices(drawVerts.ToArray());
        _mesh.SetTriangles(drawIndices.ToArray());
        _mesh.Apply();
    }

    public void Draw(Matrix4x4 mat, ICamera cam)
    {
        if (drawVerts.Count == 0)
            return;

        Material.Use();
        Material.SetUniforms();
        Material.SetViewMatrix(mat, cam);

        _mesh.Draw();
    }
}
