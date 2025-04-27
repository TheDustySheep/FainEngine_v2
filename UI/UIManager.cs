using System.Numerics;

namespace FainEngine_v2.UI;

internal class UIManager
{
    private UIElement elem = new()
    {
        xMin = 0.2f,
        xMax = 0.5f,

        yMin = 0.5f,
        yMax = 0.75f
    };

    readonly UIMesh mesh;
    readonly List<UIVertex> drawVerts = new();
    readonly List<uint> drawIndices = new();

    public UIManager()
    {
        mesh = new UIMesh();
    }

    public void Draw()
    {
        if (mesh == null)
            return;

        // Clear Data
        drawVerts.Clear();
        drawIndices.Clear();

        mesh.Clear();

        // Create Mesh
        DrawElement(elem);

        // Update mesh
        mesh.SetVertices(drawVerts.ToArray());
        mesh.SetTriangles(drawIndices.ToArray());
        mesh.Apply();

        // Draw Mesh
        mesh.Draw();
    }

    private void DrawElement(UIElement elem)
    {
        uint vertCount = (uint)drawVerts.Count;
        Span<UIVertex> verts =
        [
            new UIVertex() { Position = new Vector2(elem.xMin * 2 - 1, elem.yMin * 2 - 1) },
            new UIVertex() { Position = new Vector2(elem.xMin * 2 - 1, elem.yMax * 2 - 1) },
            new UIVertex() { Position = new Vector2(elem.xMax * 2 - 1, elem.yMax * 2 - 1) },
            new UIVertex() { Position = new Vector2(elem.xMax * 2 - 1, elem.yMin * 2 - 1) }
        ];
        drawVerts.AddRange(verts);

        drawIndices.Add(vertCount + 0);
        drawIndices.Add(vertCount + 1);
        drawIndices.Add(vertCount + 2);
        drawIndices.Add(vertCount + 2);
        drawIndices.Add(vertCount + 3);
        drawIndices.Add(vertCount + 0);
    }
}
