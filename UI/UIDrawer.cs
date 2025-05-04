using FainEngine_v2.Core;
using FainEngine_v2.UI.Elements;
using System.Numerics;

namespace FainEngine_v2.UI
{
    internal class UIDrawer
    {
        UICanvas _UIManager;

        public UIDrawer(UICanvas uiManager)
        {
            _UIManager = uiManager;
        }

        internal void Process(UIElement root)
        {
            var drawRoot = new DrawNode(root);

            FitSize(drawRoot, Layout.Axis.X);
            ResizeChildren(drawRoot, Layout.Axis.X);

            FitSize(drawRoot, Layout.Axis.Y);
            ResizeChildren(drawRoot, Layout.Axis.Y);

            Position(drawRoot, Layout.Axis.X);
            Position(drawRoot, Layout.Axis.Y);

            Draw(drawRoot);
        }

        private static void FitSize(DrawNode node, Layout.Axis axis)
        {
            foreach (var child in node.Children)
            {
                FitSize(child, axis);
            }

            var elem = node.Element;

            switch (elem.GetSizeMode(axis))
            {
                case Layout.SizeMode.Fixed:
                    {
                        node.SetSize(axis, elem.GetSize(axis));
                    }
                    break;
                case Layout.SizeMode.Fit:
                    {
                        float total =
                                elem.GetPaddingStart(axis) +
                                elem.GetPaddingEnd(axis);

                        if (elem.LayoutAxis == axis)
                        {
                            // Sum width
                            total += elem.ChildGap * (node.Children.Length - 1);

                            foreach (var child in node.Children)
                                total += child.GetSize(axis);

                        }
                        else
                        {
                            // UVMax of children
                            total += node.Children.Length == 0 ? 0 : node.Children.Max(i => i.GetSize(axis));
                        }

                        // Let it be the desired size if requested
                        total = MathF.Max(elem.GetSize(axis), total);
                        node.SetSize(axis, total);
                    }
                    break;
                case Layout.SizeMode.Grow:
                    {
                        node.SetSize(axis, elem.GetSize(axis));
                    }
                    break;
                case Layout.SizeMode.Shrink:
                    {
                        node.SetSize(axis, elem.GetSize(axis));
                    }
                    break;
                case Layout.SizeMode.Flexible:
                    {
                        node.SetSize(axis, elem.GetSize(axis));
                    }
                    break;
                default:
                    throw new NotImplementedException();

            }
        }

        private static void ResizeChildren(DrawNode node, Layout.Axis axis)
        {
            var elem = node.Element;

            if (axis == elem.LayoutAxis)
            {
                float remainingSize =
                    node.GetSize(axis) -
                    elem.GetPaddingStart(axis) -
                    elem.GetPaddingEnd(axis);

                foreach (var child in node.Children)
                {
                    remainingSize -= child.GetSize(axis);
                }

                remainingSize -= (node.Children.Length - 1) * elem.ChildGap;

                GrowChildren(node, axis, ref remainingSize);
                ShrinkChildren(node, axis, ref remainingSize);
            }
            else
            {
                float remainingSize =
                    node.GetSize(axis) -
                    elem.GetPaddingStart(axis) -
                    elem.GetPaddingEnd(axis);

                foreach (var child in node.Children)
                {
                    var childSizeMode = child.Element.GetSizeMode(axis);
                    if (childSizeMode == Layout.SizeMode.Grow)
                    {
                        child.SetSize(axis,
                            MathF.Max
                            (
                                child.GetSize(axis),
                                remainingSize
                            )
                        );
                    }
                    else if (childSizeMode == Layout.SizeMode.Shrink)
                    {
                        child.SetSize(axis,
                            MathF.Min
                            (
                                child.GetSize(axis),
                                remainingSize
                            )
                        );
                    }
                }
            }
        }

        private static void GrowChildren(DrawNode node, Layout.Axis axis, ref float remainingSize)
        {
            // Growable children
            List<DrawNode> growable = node
                .Children
                .Where(i =>
                    i.Element.GetSizeMode(axis) == Layout.SizeMode.Grow ||
                    i.Element.GetSizeMode(axis) == Layout.SizeMode.Flexible
                )
                .ToList();

            while (remainingSize > 0 && growable.Count > 0)
            {
                float beginSize = remainingSize;

                // Calculate child sizes
                float smallest = growable[0].GetSize(axis);
                float secondSmallest = float.PositiveInfinity;
                float addSize = remainingSize;

                foreach (var child in growable)
                {
                    float childSize = child.GetSize(axis);

                    if (childSize < smallest)
                    {
                        secondSmallest = smallest;
                        smallest = childSize;
                    }
                    else if (childSize > smallest)
                    {
                        secondSmallest = MathF.Min(secondSmallest, childSize);
                        addSize = secondSmallest - smallest;
                    }
                }

                addSize = MathF.Min(addSize, remainingSize / growable.Count);

                for (int i = 0; i < growable.Count; i++)
                {
                    var child = growable[i];

                    float prevWidth = child.GetSize(axis);
                    if (prevWidth == smallest)
                    {
                        child.SetSize(axis, prevWidth + addSize);

                        float maxSize = child.Element.GetSizeMax(axis);
                        if (child.GetSize(axis) >= maxSize)
                        {
                            child.SetSize(axis, maxSize);
                            growable.RemoveAt(i);
                            i--;
                        }

                        remainingSize -= (child.GetSize(axis) - prevWidth);
                    }
                }

                // No more expansion can happen
                if (remainingSize == beginSize)
                    break;
            }
        }

        private static void ShrinkChildren(DrawNode node, Layout.Axis axis, ref float remainingSize)
        {
            List<DrawNode> shrinkable = node
                .Children
                .Where(i => 
                    i.Element.GetSizeMode(axis) == Layout.SizeMode.Shrink ||
                    i.Element.GetSizeMode(axis) == Layout.SizeMode.Flexible
                )
                .ToList();

            while (remainingSize < 0 && shrinkable.Count > 0)
            {
                float beginSize = remainingSize;

                float largest = shrinkable[0].GetSize(axis);
                float secondLargest = 0f;
                float addSize = remainingSize;

                foreach (var child in shrinkable)
                {
                    float childSize = child.GetSize(axis);

                    if (childSize > largest)
                    {
                        secondLargest = largest;
                        largest = childSize;
                    }
                    else if (childSize < largest)
                    {
                        secondLargest = MathF.Max(secondLargest, childSize);
                        addSize = secondLargest - largest;
                    }
                }

                addSize = MathF.Max(addSize, remainingSize / shrinkable.Count);

                for (int i = 0; i < shrinkable.Count; i++)
                {
                    var child = shrinkable[i];
                    float prevSize = child.GetSize(axis);

                    if (prevSize == largest)
                    {
                        float minSize = child.Element.GetSizeMin(axis);

                        child.SetSize(axis, prevSize + addSize);

                        if (child.GetSize(axis) <= minSize)
                        {
                            child.SetSize(axis, minSize);
                            shrinkable.RemoveAt(i);
                            i--;
                        }

                        // Account for what we actually removed
                        remainingSize += (prevSize - child.GetSize(axis));
                    }
                }

                // If nothing changed this pass, bail out to avoid an infinite loop
                if (remainingSize == beginSize)
                    break;
            }
        }

        private static void Position(DrawNode node, Layout.Axis axis)
        {
            var elem = node.Element;

            float offset = 
                node.GetOffset(axis) +
                elem.GetPaddingStart(axis);

            if (elem.LayoutAxis == axis)
            {
                float remainingSize =
                    node.GetSize        (axis) -
                    elem.GetPaddingStart(axis) -
                    elem.GetPaddingEnd  (axis) -
                    (elem.ChildGap * (node.Children.Length - 1));

                foreach (var child in node.Children)
                    remainingSize -= child.GetSize(axis);

                switch (elem.Justify)
                {
                    case Layout.Justify.Start:
                        {
                            foreach (var child in node.Children)
                            {
                                child.SetOffset(axis, offset);
                                offset += child.GetSize(axis) + elem.ChildGap;

                                Position(child, axis);
                            }
                        }
                        break;
                    case Layout.Justify.Center:
                        {
                            offset += remainingSize / 2f;
                            foreach (var child in node.Children)
                            {
                                child.SetOffset(axis, offset);
                                offset += child.GetSize(axis) + elem.ChildGap;

                                Position(child, axis);
                            }
                        }
                        break;
                    case Layout.Justify.End:
                        {
                            offset += remainingSize;
                            foreach (var child in node.Children)
                            {
                                child.SetOffset(axis, offset);
                                offset += child.GetSize(axis) + elem.ChildGap;

                                Position(child, axis);
                            }
                        }
                        break;
                    case Layout.Justify.SpaceBetween:
                        {
                            float gap = 0f;
                            if (node.Children.Length > 1)
                                gap = remainingSize / (node.Children.Length - 1);

                            foreach (var child in node.Children)
                            {
                                child.SetOffset(axis, offset);
                                offset += child.GetSize(axis) + elem.ChildGap + gap;

                                Position(child, axis);
                            }
                        }
                        break;
                    case Layout.Justify.SpaceEvenly:
                        {
                            float gap = remainingSize / (node.Children.Length + 1);

                            foreach (var child in node.Children)
                            {
                                offset += gap;
                                child.SetOffset(axis, offset);
                                offset += child.GetSize(axis) + elem.ChildGap;

                                Position(child, axis);
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }


            }
            else
            {
                float parentSpace =
                    node.GetSize        (axis) -
                    elem.GetPaddingStart(axis) -
                    elem.GetPaddingEnd  (axis);

                foreach (var child in node.Children)
                {
                    float remainingSize = parentSpace - child.GetSize(axis);

                    switch (elem.Align)
                    {
                        case Layout.Align.Start:
                            {
                                child.SetOffset(axis, offset);
                            }
                            break;
                        case Layout.Align.Center:
                            {
                                child.SetOffset(axis, offset + remainingSize / 2f);
                            }
                            break;
                        case Layout.Align.End:
                            {
                                child.SetOffset(axis, offset + remainingSize);
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    Position(child, axis);
                }
            }
        }

        private void Draw(DrawNode node, int z=0)
        {
            if (node.Element.IsVisible)
            {
                _UIManager.DrawElement(node);
            }

            foreach (var child in node.Children)
            {
                Draw(child, z+1);
            }
        }

    }

    internal class DrawNode
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

        internal float GetSize(Layout.Axis axis) => axis == Layout.Axis.X ? XSize : YSize;
        internal void SetSize(Layout.Axis axis, float size)
        {
            if (axis == Layout.Axis.X)
                XSize = size;
            else
                YSize = size;
        }

        internal float GetOffset(Layout.Axis axis) => axis == Layout.Axis.X ? XOffset : YOffset;
        internal void SetOffset(Layout.Axis axis, float offset)
        {
            if (axis == Layout.Axis.X)
                XOffset = offset;
            else
                YOffset = offset;
        }

        public float XSize;
        public float YSize;

        public float XOffset;
        public float YOffset;

        public int ZIndex = 0;
    }
}
