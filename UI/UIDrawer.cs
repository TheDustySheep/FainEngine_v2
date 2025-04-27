using FainEngine_v2.Core;
using System.Numerics;

namespace FainEngine_v2.UI
{
    internal class UIDrawer
    {
        UIManager _UIManager;

        int screenX;
        int screenY;
        float invScreenX;
        float invScreenY;

        public UIDrawer(UIManager uiManager)
        {
            _UIManager = uiManager;
        }

        internal void Process(UIElement root)
        {
            screenX = GameGraphics.Window.FramebufferSize.X;
            screenY = GameGraphics.Window.FramebufferSize.Y;
            invScreenX = 1f / screenX;
            invScreenY = 1f / screenY;

            var drawRoot = new DrawNode(root);

            Size(drawRoot);
            Position(drawRoot);
            Draw(drawRoot);
        }

        private void Size(DrawNode node)
        {
            foreach (var child in node.Children)
            {
                Size(child);
            }

            var elem = node.Element;
            
            // Size X
            switch (elem.XSize.Mode)
            {
                case Layout.SizeMode.Fixed:
                    node.XSize = elem.XSize.Value;
                    break;
                case Layout.SizeMode.Fit:
                    if (elem.PrimaryAxis == Layout.Direction.Horizontal)
                    {
                        // Sum width
                        float total = 
                            elem.PaddingLeft + 
                            elem.PaddingRight + 
                            elem.ChildGap * (node.Children.Length - 1);

                        foreach (var child in node.Children)
                            total += child.XSize;
                        node.XSize = total;
                    }
                    else
                    {
                        // Max width
                        float total = elem.PaddingLeft + elem.PaddingRight;
                        total += node.Children.Max(i => i.XSize);
                        node.XSize = total;
                    }
                    break;
                default:
                    break;
            }

            // Size Y
            switch (elem.YSize.Mode)
            {
                case Layout.SizeMode.Fixed:
                    node.YSize = elem.YSize.Value;
                    break;
                case Layout.SizeMode.Fit:
                    if (elem.PrimaryAxis == Layout.Direction.Vertical)
                    {
                        // Sum width
                        float total = 
                            elem.PaddingTop + 
                            elem.PaddingBottom + 
                            elem.ChildGap * (node.Children.Length - 1);

                        foreach (var child in node.Children)
                            total += child.YSize;
                        node.YSize = total;
                    }
                    else
                    {
                        // Max width
                        float total = elem.PaddingTop + elem.PaddingBottom;
                        total += node.Children.Max(i => i.YSize);
                        node.YSize = total;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Position(DrawNode node)
        {
            var elem = node.Element;
            float xOffset = node.XOffset + elem.PaddingLeft;
            float yOffset = node.YOffset + elem.PaddingTop;

            if (elem.PrimaryAxis == Layout.Direction.Horizontal)
            {
                foreach (var child in node.Children)
                {
                    child.XOffset = xOffset;
                    child.YOffset = yOffset;
                    xOffset += child.XSize + elem.ChildGap;

                    Position(child);
                }
            }
            else
            {
                foreach (var child in node.Children)
                {
                    child.XOffset = xOffset;
                    child.YOffset = yOffset;
                    yOffset += child.YSize + elem.ChildGap;

                    Position(child);
                }
            }
        }

        private void Draw(DrawNode node, int z=0)
        {
            _UIManager.DrawElement(
                new Vector2(
                    node.XOffset * invScreenX,
                    node.YOffset * invScreenY
                ),
                new Vector2(
                    node.XSize * invScreenX, 
                    node.YSize * invScreenY
                ),
                node.Element,
                z
            );

            foreach (var child in node.Children)
            {
                Draw(child, z+1);
            }
        }

        private class DrawNode
        {
            public UIElement Element { get; }
            public DrawNode? Parent { get; }
            public DrawNode[] Children { get; }

            public DrawNode(UIElement element, DrawNode? parent = null)
            {
                Element = element ?? throw new ArgumentNullException(nameof(element));
                Parent = parent;

                // Recursively build child DrawNodes
                Children = element.Children
                                  .Select(child => new DrawNode(child, this))
                                  .ToArray();
            }

            public float XSize;
            public float YSize;

            public float XOffset;
            public float YOffset;
        }
    }
}
