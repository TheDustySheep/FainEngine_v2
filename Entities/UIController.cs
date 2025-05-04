using FainEngine_v2.Rendering;
using FainEngine_v2.UI;

namespace FainEngine_v2.Entities
{
    public class UIController : IEntity
    {
        public UICanvas Canvas;
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
}
