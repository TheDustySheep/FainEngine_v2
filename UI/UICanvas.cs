using FainEngine_v2.Core;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Resources;
using FainEngine_v2.UI.Elements;
using FainEngine_v2.UI.FontRendering;
using System.Drawing;
using System.Numerics;

namespace FainEngine_v2.UI;

public class UICanvas
{
    Vector2 invScreenSize;
    public FontAtlas Atlas { get; private set; }
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
        _drawer = new UIDrawer(this);
        Root = CreateRoot();

        Atlas = new FontAtlas(GameGraphics.GL, @"Resources/Fonts/lemon_milk/LEMONMILK-Regular.otf", 72);
        _uiMaterial = new UIMaterial(ResourceLoader.LoadShader(@"Resources/UI"), Atlas.AtlasTexture);

        Root.AddChild
        (
            new UIElement()
            {
                XSize = 256,
                YSize = 256,
                BackgroundColour = Color.Red
            }.AddChild
            (
                new UIText(this, "test")
            )
        );
    }

    public void Draw()
    {
        Root.XSize = GameGraphics.Window.FramebufferSize.X;
        Root.YSize = GameGraphics.Window.FramebufferSize.Y;

        invScreenSize = new Vector2
        (
            1f / GameGraphics.Window.FramebufferSize.X,
            1f / GameGraphics.Window.FramebufferSize.Y
        );

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
        _uiMaterial.SetUniforms();
        _uiMaterial.Use();
        _mesh.Draw();
    }

    internal void DrawElement(DrawNode node)
    {
        uint vertCount = (uint)drawVerts.Count;

        var elem = node.Element;
        var verts = elem.GenerateVerts(node, invScreenSize);

        drawVerts.AddRange(verts);

        // If multiple quads returned then add them all
        for (int i = 0; i < verts.Count() / 4; i++)
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
