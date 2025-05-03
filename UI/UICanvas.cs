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

        Atlas = new FontAtlas(@"Resources/Fonts/Tinos/Tinos-Regular.ttf", 16);
        _uiMaterial = new UIMaterial(ResourceLoader.LoadShader(@"Resources/UI"), Atlas.AtlasTexture);
    }

    public void Draw()
    {
        

        Vector2 screenSize = new Vector2(
            GameGraphics.Window.FramebufferSize.X,
            GameGraphics.Window.FramebufferSize.Y
        );

        Root.XSize = screenSize.X;
        Root.YSize = screenSize.Y;

        if (_mesh == null)
            return;

        // Clear Data
        drawVerts.Clear();
        drawIndices.Clear();

        _mesh.Clear();

        // Create Mesh
        _drawer.Process(Root);

        // Update _mesh
        _mesh.SetVertices(drawVerts.ToArray());
        _mesh.SetTriangles(drawIndices.ToArray());
        _mesh.Apply();

        // Draw Mesh
        _uiMaterial.Use();
        _uiMaterial.SetUniforms();
        _uiMaterial.SetViewMatrix(ViewMatrix());

        _mesh.Draw();
    }

    internal Matrix4x4 ViewMatrix()
    {
        var scale = Matrix4x4.CreateScale(
            2f / GameGraphics.Window.FramebufferSize.X,
            2f /-GameGraphics.Window.FramebufferSize.Y,
            0.0001f
        );
        var translation = Matrix4x4.CreateTranslation(-1f, 1f, 0f);
        
        return scale * translation;
        //return Matrix4x4.Identity;
    }

    internal void DrawElement(DrawNode node)
    {
        uint vertCount = (uint)drawVerts.Count;

        var elem = node.Element;
        var verts = elem.GenerateVerts(node);

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
