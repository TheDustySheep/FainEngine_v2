using System.Drawing;

namespace FainEngine_v2.UI;

public class UIElement
{
    #region Tree

    public UIElement? Parent;
    public List<UIElement> Children = new();

    #endregion

    #region Drawing Properties

    public Color BackgroundColour;
    public bool IsVisible = true;

    #endregion

    #region Layout Data

    // Sizing
    public Layout.SizeMode XSizeMode = Layout.SizeMode.Fit;
    public float XSize;
    public float XSizeMin = 0f;
    public float XSizeMax = float.PositiveInfinity;

    public Layout.SizeMode YSizeMode = Layout.SizeMode.Fit;
    public float YSize;
    public float YSizeMin = 0f;
    public float YSizeMax = float.PositiveInfinity;

    // Padding
    public float PaddingTop;
    public float PaddingBottom;
    public float PaddingLeft;
    public float PaddingRight;
    public float Padding
    {
        set
        {
            PaddingTop    = value;
            PaddingBottom = value;
            PaddingLeft   = value;
            PaddingRight  = value;
        }
    }

    // Positioning
    public Layout.Justify Justify;
    public Layout.Align   Align;

    // Children
    public Layout.Axis LayoutAxis;

    public float ChildGap;

    #endregion


    #region Accessors

    internal Layout.SizeMode GetSizeMode(Layout.Axis axis) => axis == Layout.Axis.X ? XSizeMode : YSizeMode;

    internal float GetSize   (Layout.Axis axis) => axis == Layout.Axis.X ? XSize    : YSize;
    internal float GetSizeMin(Layout.Axis axis) => axis == Layout.Axis.X ? XSizeMin : YSizeMin;
    internal float GetSizeMax(Layout.Axis axis) => axis == Layout.Axis.X ? XSizeMax : YSizeMax;

    internal float GetPaddingStart(Layout.Axis axis) => axis == Layout.Axis.X ? PaddingLeft  : PaddingTop;
    internal float GetPaddingEnd  (Layout.Axis axis) => axis == Layout.Axis.X ? PaddingRight : PaddingBottom;

    #endregion

    public UIElement AddChild(UIElement child)
    {
        Children.Add(child);
        child.Parent = this;
        return this;
    }

    public UIElement AddChildren(params UIElement[] children)
    {
        foreach (UIElement child in children)
            AddChild(child);
        return this;
    }

    public UIElement AddChildren(IEnumerable<UIElement> children)
    {
        foreach (UIElement child in children)
            AddChild(child);
        return this;
    }
}
