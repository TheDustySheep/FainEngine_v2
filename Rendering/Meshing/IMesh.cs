using FainEngine_v2.Rendering.BoundingShapes;

namespace FainEngine_v2.Rendering.Meshing;

public interface IMesh : IDisposable
{
    public BoundingBox Bounds { get; }
    public void Apply();
    public void Bind();
    public void Draw();
}