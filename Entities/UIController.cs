using FainEngine_v2.Rendering;
using FainEngine_v2.UI;
using FainEngine_v2.UI.UIElements.Types;

namespace FainEngine_v2.Entities;

public class UIController : IEntity
{
    public UIElement Root => Canvas.Root;
    public UICanvas Canvas { get; private set; }

    public UIController()
    {
        Canvas = new UICanvas();
        GameGraphics.RegisterCanvas(Canvas);
    }

    ~UIController()
    {
        GameGraphics.UnregisterCanvas(Canvas);
    }
}
