using FainEngine_v2.Rendering;
using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.UI.Core;
using System.Numerics;

namespace FainEngine_v2.UI.Drawing;

public class UIDrawer
{
    private MeshHandler _meshHandler = new MeshHandler();

    readonly ICanvas _canvas;
    
    public UIDrawer(ICanvas canvas)
    {
        _canvas = canvas;
    }

    public void Draw(ICamera cam)
    {
        Vector2 screenSize = new Vector2(
            GameGraphics.Window.FramebufferSize.X,
            GameGraphics.Window.FramebufferSize.Y
        );

        _canvas.Root.Styles.Values.XSize = screenSize.X;
        _canvas.Root.Styles.Values.YSize = screenSize.Y;

        // Clear Layers
        _meshHandler.Clear();

        // Solve UI Layout
        LayoutSolver.UpdateLayout(_canvas.Root, _canvas.ClassList);

        // Prepare Mesh Data
        foreach (var node in _canvas.Root.Descendants())
            node.GenerateMesh(_meshHandler);
        _meshHandler.Apply();

        // Drawing
        _meshHandler.Draw(ViewMatrix(), cam);
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
    }
}
