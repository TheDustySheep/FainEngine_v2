using FainEngine_v2.Core;
using FainEngine_v2.Rendering;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Resources;
using FainEngine_v2.Utils;
using System.Numerics;

namespace FainEngine_v2.Entities;
public class TestModel : IEntity
{
    Material material;
    Model model;

    public TestModel()
    {
        var shader = ResourceLoader.LoadShader("Resources/DefaultShader");
        material = new Material(shader);
        model = ResourceLoader.LoadModel("cube.model");
    }

    public void Update()
    {
        var difference = (float)(GameTime.TotalTime * 100);

        var modelMat = Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(difference)) * Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(difference));

        GameGraphics.DrawMesh(model.Meshes[0], material, modelMat);
    }
}
