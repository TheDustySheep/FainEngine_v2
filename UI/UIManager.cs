using FainEngine_v2.Core;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Resources;
using System.Drawing;
using System.Numerics;

namespace FainEngine_v2.UI;

public class UIManager
{
    private UIElement _root;
    private UIDrawer  _drawer;

    readonly UIMesh _mesh;
    readonly List<UIVertex> drawVerts = new();
    readonly List<uint> drawIndices = new();

    readonly Material _uiMaterial;

    public UIManager()
    {
        _mesh = new UIMesh();
        _uiMaterial = new Material(ResourceLoader.LoadShader(@"Resources/UI"));
        _root = CreateRoot();
        _drawer = new UIDrawer(this);
    }

    public void Draw()
    {
        if (_mesh == null)
            return;

        // Clear Data
        drawVerts.Clear();
        drawIndices.Clear();

        _mesh.Clear();

        // Create Mesh
        _drawer.Process(_root);

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

    private UIElement CreateRoot()
    {
        return new UIElement()
        {
            PaddingTop = 10,
            PaddingBottom = 10,
            PaddingLeft = 10,
            PaddingRight = 10,
            ChildGap = 25,
            BackgroundColour = Color.DarkSalmon,
        }
        .AddChildren
        (
            new UIElement()
            {
                PaddingTop = 10,
                PaddingBottom = 10,
                PaddingLeft = 10,
                PaddingRight = 10,

                XSize = new Layout.Size(Layout.SizeMode.Fixed, 100),
                YSize = new Layout.Size(Layout.SizeMode.Fixed, 200),

                BackgroundColour = Color.BlanchedAlmond,
            }
            .AddChildren
            (
                new UIElement()
                {
                    XSize = new Layout.Size(Layout.SizeMode.Fixed, 20),
                    YSize = new Layout.Size(Layout.SizeMode.Fixed, 20),

                    BackgroundColour = Color.Red,
                }
            ),
            new UIElement()
            {
                XSize = new Layout.Size(Layout.SizeMode.Fixed, 100),
                YSize = new Layout.Size(Layout.SizeMode.Fixed, 25),
            
                BackgroundColour = Color.Snow,
            }           
        );
    }
}
