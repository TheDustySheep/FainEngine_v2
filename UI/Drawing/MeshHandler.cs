using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.UI.Core;
using FainEngine_v2.UI.UIElements.Rendering;
using FainEngine_v2.Utils;
using System.Numerics;

namespace FainEngine_v2.UI.Drawing;
public class MeshHandler : IMeshHandler
{
    UIMaterialManager _materialManager;

    private readonly Dictionary<LayerData, UILayer> _uiLayers = new();

    public MeshHandler()
    {
        _materialManager = DependencyInjector.Resolve<UIMaterialManager>();
    }

    public void AddMeshData(Span<UIVertex> verts, LayerData layer)
    {
        if (!_uiLayers.TryGetValue(layer, out var uiLayer))
        {
            var mat = _materialManager.RequestMaterial(layer.FontName, layer.FontSize);
            uiLayer = new UILayer(mat);
            _uiLayers.Add(layer, uiLayer);
        }

        uiLayer.AddElementToMesh(verts);
    }

    public void Clear()
    {
        foreach (var layer in _uiLayers.Values)
        {
            layer.Clear();
        }
    }

    public void Apply()
    {
        foreach (var layer in _uiLayers.Values)
        {
            layer.Apply();
        }
    }

    public void Draw(Matrix4x4 mat, ICamera cam)
    {
        foreach (var layer in _uiLayers.Values)
        {
            layer.Draw(mat, cam);
        }
    }
}
