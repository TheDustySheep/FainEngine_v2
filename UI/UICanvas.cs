using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.UI.FontRendering;
using FainEngine_v2.UI.Fss.Evaluators;
using FainEngine_v2.UI.Fss.StyleClasses;
using FainEngine_v2.UI.UIElements.Types;
using static FainEngine_v2.UI.Fss.Styling.Styles;

namespace FainEngine_v2.UI;

public class UICanvas
{
    public IClassList? ClassList { set => _drawer.ClassList = value; }
    public readonly UIElement Root;
    public float Priority { get; set; }

    private UIDrawer _drawer;

    FontController controller = new();
    public UICanvas(UIElement? element = null, IClassList? classList = null)
    {
        Root = CreateRoot();

        if (element != null)
            Root.AddChildren(element);

        controller.RequestFontAtlas("Tinos-Regular", 16, out var mapping);
        var uiMaterial = controller.UIMaterial;
        _drawer = new UIDrawer(Root, classList, uiMaterial);
    }

    public void Draw(ICamera cam)
    {
        _drawer.Draw(cam);
    }

    private static UIElement CreateRoot()
    {
        var elem = new UIElement();
        elem.Styles.LocalStyles.XSizeMode = SizeMode.Fixed;
        elem.Styles.LocalStyles.YSizeMode = SizeMode.Fixed;
        return elem;
    }
}