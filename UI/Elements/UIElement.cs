using System.Drawing;
using System.Numerics;

namespace FainEngine_v2.UI.Elements;

public class UIElement
{
    #region Tree

    public UIElement? Parent;
    public List<UIElement> Children = new();

    #endregion

    #region Drawing Properties

    public UIColour BackgroundColour;
    public UIColour TextColour;

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

    internal virtual IEnumerable<UIVertex> GenerateVerts(DrawNode node, Vector2 invScreenSize)
    {
        Vector2 min = new Vector2(node.XOffset, node.YOffset) * invScreenSize;
        Vector2 max = (new Vector2(node.XSize, node.YSize) + min) * invScreenSize;

        return [
            new UIVertex(min.X * 2 - 1, (1 - max.Y) * 2 - 1, node.ZIndex*0.0001f, this),
            new UIVertex(min.X * 2 - 1, (1 - min.Y) * 2 - 1, node.ZIndex*0.0001f, this),
            new UIVertex(max.X * 2 - 1, (1 - min.Y) * 2 - 1, node.ZIndex*0.0001f, this),
            new UIVertex(max.X * 2 - 1, (1 - max.Y) * 2 - 1, node.ZIndex*0.0001f, this),
        ];
    }
}
