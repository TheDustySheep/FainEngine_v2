using System.Drawing;
using System.Numerics;

namespace FainEngine_v2.UI.Elements;

public class UIElement
{
    #region Tree

    private UIElement? _parent;
    private List<UIElement> _children = new();

    public UIElement? Parent => _parent;
    public IReadOnlyList<UIElement> Children => _children;

    #endregion

    #region Drawing Properties

    public virtual UIColour BackgroundColour { get; set; }
    public virtual UIColour TextColour {get; set; }

    public virtual bool IsVisible {get; set; } = true;

    #endregion

    #region Layout Data

    // Sizing
    public virtual Layout.SizeMode XSizeMode { get; set; } = Layout.SizeMode.Fit;
    public virtual float XSize    { get; set; }
    public virtual float XSizeMin { get; set; } = 0f;
    public virtual float XSizeMax { get; set; } = float.PositiveInfinity;

    public virtual Layout.SizeMode YSizeMode { get; set; } = Layout.SizeMode.Fit;
    public virtual float YSize    { get; set; }
    public virtual float YSizeMin { get; set; } = 0f;
    public virtual float YSizeMax { get; set; } = float.PositiveInfinity;

    // Padding
    public virtual float PaddingTop    { get; set; }
    public virtual float PaddingBottom { get; set; }
    public virtual float PaddingLeft   { get; set; }
    public virtual float PaddingRight  { get; set; }
    public virtual float Padding
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
    public virtual Layout.Justify Justify { get; set; }
    public virtual Layout.Align   Align   { get; set; }

    // _children
    public virtual Layout.Axis LayoutAxis { get; set; }

    public virtual float ChildGap { get; set; }

    #endregion


    #region Accessors

    internal virtual Layout.SizeMode GetSizeMode(Layout.Axis axis) => axis == Layout.Axis.X ? XSizeMode : YSizeMode;

    internal virtual float GetSize   (Layout.Axis axis) => axis == Layout.Axis.X ? XSize    : YSize;
    internal virtual float GetSizeMin(Layout.Axis axis) => axis == Layout.Axis.X ? XSizeMin : YSizeMin;
    internal virtual float GetSizeMax(Layout.Axis axis) => axis == Layout.Axis.X ? XSizeMax : YSizeMax;

    internal virtual float GetPaddingStart(Layout.Axis axis) => axis == Layout.Axis.X ? PaddingLeft  : PaddingTop;
    internal virtual float GetPaddingEnd  (Layout.Axis axis) => axis == Layout.Axis.X ? PaddingRight : PaddingBottom;

    #endregion

    public UIElement SetParent(UIElement parent)
    {
        if (_parent != null)
            parent._children.Remove(this);
        _parent = null;
        return this;
    }

    public UIElement AddChild(UIElement child)
    {
        _children.Add(child);
        child._parent = this;
        return this;
    }

    public UIElement RemoveChild(UIElement child)
    {
        _children.Remove(child);
        child._parent = null;
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

    internal virtual IEnumerable<UIVertex> GenerateVerts(DrawNode node)
    {
        Vector2 min = new Vector2(node.XOffset, node.YOffset);
        Vector2 max = new Vector2(node.XSize, node.YSize) + min;

        return [
            new UIVertex(min.X, min.Y, node.ZIndex, this),
            new UIVertex(max.X, min.Y, node.ZIndex, this),
            new UIVertex(max.X, max.Y, node.ZIndex, this),
            new UIVertex(min.X, max.Y, node.ZIndex, this),
        ];
    }
}
