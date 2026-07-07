using FainEngine_v2.Rendering.BoundingShapes;

namespace FainEngine_v2.Rendering.Meshing;

public interface IMesh : IDisposable
{
    public bool ClipBounds { get; set; }
    public BoundingBox Bounds { get; }
    public void Draw();
}