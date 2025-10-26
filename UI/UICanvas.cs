using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.Resources;
using FainEngine_v2.UI.FontRendering;
using FainEngine_v2.UI.Fonts;
using FainEngine_v2.UI.Stylesheets;
using FainEngine_v2.UI.UIElements;
using System.Drawing;

namespace FainEngine_v2.UI;

public class UICanvas
{
    public readonly UIElement Root;
    public float Priority { get; set; }

    private UIDrawer _drawer;

    FontController controller = new();
    public UICanvas(UIElement? element = null)
    {
        Root = CreateRoot();

        if (element != null)
            Root.AddChildren(element);

        controller.RequestFontAtlas("Tinos-Regular", 16, out var mapping);
        var uiMaterial = controller.UIMaterial;
        _drawer = new UIDrawer(Root, uiMaterial);
    }

    public void Draw(ICamera cam)
    {
        _drawer.Draw(cam);
    }

    private static UIElement CreateRoot()
    {
        var elem = new UIElement();
        elem.Styles.XSizeMode = IStyles.SizeMode.Fixed;
        elem.Styles.YSizeMode = IStyles.SizeMode.Fixed;
        return elem;
    }
}