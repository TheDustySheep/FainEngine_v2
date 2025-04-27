using FainEngine_v2.Rendering.Meshing;

namespace FainEngine_v2.UI;
internal class UIMesh : CustomVertexMesh<UIVertex, uint>
{
    public UIMesh()
    {
        ClipBounds = false;
    }
}
