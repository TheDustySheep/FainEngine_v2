using FainEngine_v2.Rendering;
using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.UI;
using FainEngine_v2.UI.Drawing;

namespace FainEngine_v2.Entities;

public class UIController : IEntity
{
    private List<CanvasData> CanvasDatas = new();

    public UIController()
    {
        GameGraphics.RegisterController(this);
    }

    ~UIController()
    {
        GameGraphics.UnregisterController(this);
    }

    public void AddCanvas(ICanvas canvas)
    {
        CanvasDatas.Add(new CanvasData() 
        { 
            Canvas = canvas, 
            UIDrawer = new UIDrawer(canvas)
        });
    }

    public void RemoveCanvas(ICanvas canvas)
    {
        CanvasDatas.RemoveAll(cd => cd.Canvas == canvas);
    }

    public void Draw(ICamera cam)
    {
        foreach (var canvasData in CanvasDatas)
        {
            canvasData.UIDrawer.Draw(cam);
        }
    }

    private class CanvasData
    {
        public required ICanvas Canvas;
        public required UIDrawer UIDrawer;
    }
}
