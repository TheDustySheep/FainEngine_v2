namespace FainEngine_v2.UI.Elements;

public class AUIElement
{
    public Layout.Align Align;



    public UIColour BackgroundColour;

    public float ChildGap;
    public List<UIElement> Children = new();

    public bool IsVisible = true;

    // Positioning
    public Layout.Justify Justify;

    // _children
    public Layout.Axis LayoutAxis;
    public float PaddingBottom;
    public float PaddingLeft;
    public float PaddingRight;

    // Padding
    public float PaddingTop;

    public UIElement? Parent;
    public UIColour TextColour;
    public float XSize;
    public float XSizeMax = float.PositiveInfinity;
    public float XSizeMin = 0f;



    // Sizing
    public Layout.SizeMode XSizeMode = Layout.SizeMode.Fit;
    public float YSize;
    public float YSizeMax = float.PositiveInfinity;
    public float YSizeMin = 0f;

    public Layout.SizeMode YSizeMode = Layout.SizeMode.Fit;
    public float Padding
    {
        set
        {
            PaddingTop = value;
            PaddingBottom = value;
            PaddingLeft = value;
            PaddingRight = value;
        }
    }
}