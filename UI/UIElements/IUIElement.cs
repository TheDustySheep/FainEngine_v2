namespace FainEngine_v2.UI.UIElements;

public interface IUIElement
{
    public RenderFragment GetMeshSegment(Rect maxBounds, float depth);
}