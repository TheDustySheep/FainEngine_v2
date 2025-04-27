using System.Drawing;

namespace FainEngine_v2.UI;

public class UIElement
{
    public Layout.Size XSize;
    public Layout.Size YSize;

    public Layout.Direction PrimaryAxis;

    public float PaddingTop;
    public float PaddingBottom;
    public float PaddingLeft;
    public float PaddingRight;

    public float ChildGap;

    public Color BackgroundColour;

    public UIElement? Parent;
    public List<UIElement> Children = new();

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
}
