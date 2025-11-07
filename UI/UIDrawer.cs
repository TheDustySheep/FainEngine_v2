using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.Rendering;
using FainEngine_v2.UI.Core;
using FainEngine_v2.UI.UIElements;
using System.Numerics;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.UI.UIElements.Types;
using FainEngine_v2.UI.Fss.StyleClasses;

namespace FainEngine_v2.UI;

public class UIDrawer : ALayoutDrawer
{
    private UIPipeline uiPipeline = new UIPipeline();

    private IRenderElement _root;
    public IClassList? ClassList;

    readonly List<UIVertex> drawVerts = new();
    readonly List<uint> drawIndices = new();

    readonly UIMesh _mesh;
    readonly Material _uiMaterial;

    public UIDrawer(IRenderElement root, IClassList? classList, Material material)
    {
        _root = root;
        ClassList = classList;
        _uiMaterial = material;
        _mesh = new UIMesh();
    }

    public void Draw(ICamera cam)
    {
        Vector2 screenSize = new Vector2(
            GameGraphics.Window.FramebufferSize.X,
            GameGraphics.Window.FramebufferSize.Y
        );

        _root.Styles.XSize = screenSize.X;
        _root.Styles.YSize = screenSize.Y;

        if (_mesh == null)
            return;

        // Clear Data
        drawVerts.Clear();
        drawIndices.Clear();

        _mesh.Clear();

        // Solve Layout
        uiPipeline.Render(_root, ClassList);

        // Generate Mesh
        GenerateVertices(_root);

        //Console.WriteLine("============================================");

        ((UIElement)_root).PrintTreeStylesAndLayout();

        //foreach (var vert in drawVerts)
        //{
        //    Console.WriteLine($"Vert: {vert.Position}");
        //}

        // Update _mesh
        _mesh.SetVertices(drawVerts.ToArray());
        _mesh.SetTriangles(drawIndices.ToArray());
        _mesh.Apply();

        // GenerateVertices Mesh
        _uiMaterial.Use();
        _uiMaterial.SetUniforms();
        _uiMaterial.SetViewMatrix(ViewMatrix(), cam);

        _mesh.Draw();
    }

    public override void AddElementToMesh(IRenderElement elem)
    {
        uint vertCount = (uint)drawVerts.Count;

        var verts = elem.GenerateVerts();

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

    internal Matrix4x4 ViewMatrix()
    {
        var scale = Matrix4x4.CreateScale(
            2f / GameGraphics.Window.FramebufferSize.X,
            2f / -GameGraphics.Window.FramebufferSize.Y,
            -0.0001f
        );
        var translation = Matrix4x4.CreateTranslation(-1f, 1f, 0f);

        return scale * translation;
        //return Matrix4x4.Identity;
    }
}
