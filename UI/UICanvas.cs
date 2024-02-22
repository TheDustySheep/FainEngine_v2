using FainEngine_v2.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Resources;
using FainEngine_v2.UI.FontSystem;
using FainEngine_v2.UI.UIElements;
using NuklearDotNet;
using System.Numerics;

namespace FainEngine_v2.UI;
public class UICanvas : IEntity
{
    readonly UIMesh uiMesh;
    readonly Material material;
    Matrix4x4 model = Matrix4x4.Identity;

    public UICanvas()
    {
        FontLoader fontLoader = new FontLoader();
        var font = fontLoader.LoadFont();

        uiMesh = new UIMesh();

        material = new UIMaterial(ResourceLoader.LoadShader("Resources/UI"), font.Texture);

        RenderTexture rt = new RenderTexture(GameGraphics.GL, 128, 128);
    }

    public void Update()
    {

    }
}
