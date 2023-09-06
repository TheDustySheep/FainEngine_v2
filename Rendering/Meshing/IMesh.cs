namespace FainEngine_v2.Rendering.Meshing;

public interface IMesh : IDisposable
{
    public void Apply();
    public void Bind();
    void Draw();
}