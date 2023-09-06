using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;
using FainEngine_v2.Rendering;
using FainEngine_v2.Utils;
using Shader = FainEngine_v2.Rendering.Materials.Shader;

namespace FainEngine_v2.Core;

class OldProgram
{
    private static unsafe void OnRender(double deltaTime)
    {
        //texture.Bind();
        //Shader.Use();
        //Shader.SetUniform("uTexture0", 0);

        //Use elapsed time to convert to radians to allow our cube to rotate over time
    }

    private static void OnClose()
    {
        //Model.Dispose();
        //Shader.Dispose();
        //texture.Dispose();
    }
}
