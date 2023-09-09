using FainEngine_v2.Rendering;
using FainEngine_v2.Rendering.Materials;
using static FainEngine_v2.Rendering.Materials.Texture2D;
using GL = Silk.NET.OpenGL.GL;

namespace FainEngine_v2.Resources;
public static class ResourceLoader
{
    static GL? _gl;

    internal static void SetGL(GL gl)
    {
        _gl = gl;
    }

    public static Texture2D LoadTexture2D(
        string filePath,
        WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode = FilterModes.Nearest,
        MipMapModes mipMapMode = MipMapModes.None)
    {
        if (_gl is null)
            throw new Exception("OpenGL is not set");

        return new Texture2D(_gl, filePath, wrapMode, filterMode, mipMapMode);
    }

    public static Shader LoadShader(string folder)
    {
        if (_gl is null)
            throw new Exception("OpenGL is not set");

        string[] files = Directory.GetFiles(folder);

        string? vertFile = files.FirstOrDefault(i => i.EndsWith(".vert"));
        string? fragFile = files.FirstOrDefault(i => i.EndsWith(".frag"));

        if (vertFile is null)
            throw new FileNotFoundException($"Vertex shader not found in folder: {folder}");

        if (fragFile is null)
            throw new FileNotFoundException($"Fragment shader not found in folder: {folder}");

        string vertSRC = File.ReadAllText(vertFile);
        string fragSRC = File.ReadAllText(fragFile);

        return new Shader(_gl, vertSRC, fragSRC);
    }

    public static Model LoadModel(string filePath)
    {
        if (_gl is null)
            throw new Exception("OpenGL is not set");

        return new Model(_gl, filePath);
    }
}
