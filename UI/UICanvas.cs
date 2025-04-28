using FainEngine_v2.Core;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Resources;
using System.Numerics;

namespace FainEngine_v2.UI;

public class UICanvas
{
    public UIElement Root { get; init; }
    public float Priority { get; set; }

    private UIDrawer  _drawer;

    readonly UIMesh _mesh;
    readonly List<UIVertex> drawVerts = new();
    readonly List<uint> drawIndices = new();

    readonly Material _uiMaterial;

    public UICanvas()
    {
        _mesh = new UIMesh();
        _uiMaterial = new Material(ResourceLoader.LoadShader(@"Resources/UI"));
        _drawer = new UIDrawer(this);
        Root = CreateRoot();
    }

    public void Draw()
    {
        Root.XSize = GameGraphics.Window.FramebufferSize.X;
        Root.YSize = GameGraphics.Window.FramebufferSize.X;

        if (_mesh == null)
            return;

        // Clear Data
        drawVerts.Clear();
        drawIndices.Clear();

        _mesh.Clear();

        // Create Mesh
        _drawer.Process(Root);

        // Update mesh
        _mesh.SetVertices(drawVerts.ToArray());
        _mesh.SetTriangles(drawIndices.ToArray());
        _mesh.Apply();

        // Draw Mesh
        _uiMaterial.Use();
        _mesh.Draw();
    }

    internal void DrawElement(Vector2 pos, Vector2 size, UIElement elem, int z)
    {
        uint vertCount = (uint)drawVerts.Count;
        Span<UIVertex> verts =
        [
            new UIVertex((pos.X         ) * 2 - 1, (1 - pos.Y - size.Y) * 2 - 1, z*0.0001f, elem.BackgroundColour),
            new UIVertex((pos.X         ) * 2 - 1, (1 - pos.Y         ) * 2 - 1, z*0.0001f, elem.BackgroundColour),
            new UIVertex((pos.X + size.X) * 2 - 1, (1 - pos.Y         ) * 2 - 1, z*0.0001f, elem.BackgroundColour),
            new UIVertex((pos.X + size.X) * 2 - 1, (1 - pos.Y - size.Y) * 2 - 1, z*0.0001f, elem.BackgroundColour),
        ];
        drawVerts.AddRange(verts);

        drawIndices.Add(vertCount + 0);
        drawIndices.Add(vertCount + 1);
        drawIndices.Add(vertCount + 2);
        drawIndices.Add(vertCount + 2);
        drawIndices.Add(vertCount + 3);
        drawIndices.Add(vertCount + 0);
    }

    private static UIElement CreateRoot()
    {
        // Fixed to the size of the screen
        return new UIElement()
        {
            XSizeMode = Layout.SizeMode.Fixed,
            YSizeMode = Layout.SizeMode.Fixed,
            IsVisible = false
        };
    }
}
