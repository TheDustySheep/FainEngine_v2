using FainEngine_v2.Rendering;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.Meshing;
using System.Text;
using System.Text.RegularExpressions;
using static FainEngine_v2.Rendering.Materials.Texture;
using GL = Silk.NET.OpenGL.GL;

namespace FainEngine_v2.Resources;
public static class ResourceLoader
{
    static GL? _gl;

    internal static void SetGL(GL gl)
    {
        _gl = gl;
    }

    #region Textures
    public static Texture2D LoadTexture2D(
        string filePath,
        WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode = FilterModes.Nearest,
        MipMapModes mipMapMode = MipMapModes.None)
    {
        if (_gl is null)
            throw new Exception("OpenGL is not set");

        return new Texture2D(filePath, wrapMode, filterMode, mipMapMode);
    }

    public static Texture2DArray LoadTextureAtlas(
        string filePath,
        uint atlasSize,
        WrappingModes wrapMode = WrappingModes.Clamp_To_Edge,
        FilterModes filterMode = FilterModes.Nearest,
        MipMapModes mipMapMode = MipMapModes.None)
    {
        if (_gl is null)
            throw new Exception("OpenGL is not set");

        return new Texture2DArray(filePath, atlasSize, wrapMode, filterMode, mipMapMode);
    }
    #endregion

    #region Shaders
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

        string vertSRC = LoadShaderWithIncludes(vertFile);
        string fragSRC = LoadShaderWithIncludes(fragFile);

        return new Shader(vertSRC, fragSRC);
    }

    private static readonly Regex includeRegex = new Regex(@"^\s*#include\s+""(.+)""", RegexOptions.Compiled);

    public static string LoadShaderWithIncludes(string shaderPath)
    {
        var loadedFiles = new HashSet<string>();
        return LoadShaderRecursive(shaderPath, loadedFiles);
    }

    private static string LoadShaderRecursive(string shaderPath, HashSet<string> loadedFiles)
    {
        // Normalize path and prevent double-includes
        shaderPath = Path.GetFullPath(shaderPath);
        if (loadedFiles.Contains(shaderPath))
            return "";  // skip already included files

        loadedFiles.Add(shaderPath);

        if (!File.Exists(shaderPath))
            throw new FileNotFoundException($"Shader file not found: {shaderPath}");

        var shaderDir = Path.GetDirectoryName(shaderPath);
        var exeRoot = AppContext.BaseDirectory;
        var sb = new StringBuilder();

        foreach (var line in File.ReadLines(shaderPath))
        {
            // Skip comments
            if (line.StartsWith("//"))
                continue;

            var match = includeRegex.Match(line);
            if (match.Success)
            {
                var includePath = match.Groups[1].Value;

                string fullIncludePath;
                if (Path.IsPathRooted(includePath) ||
                    (!includePath.StartsWith("./") && !includePath.StartsWith("../") && !includePath.StartsWith(".\\")))
                {
                    // Treat as relative to exe root
                    fullIncludePath = Path.Combine(exeRoot, includePath.TrimStart('/', '\\'));
                }
                else
                {
                    // Treat as relative to current shader directory
                    fullIncludePath = Path.Combine(shaderDir, includePath);
                }

                // Recursively load included file
                var includedCode = LoadShaderRecursive(fullIncludePath, loadedFiles);
                sb.AppendLine($"// Begin include: {includePath}");
                sb.AppendLine(includedCode);
                sb.AppendLine($"// End include: {includePath}");
            }
            else
            {
                sb.AppendLine(line);
            }
        }

        return sb.ToString();
    }

    #endregion

    public static Model LoadModel(string filePath)
    {
        if (_gl is null)
            throw new Exception("OpenGL is not set");

        return new Model(_gl, filePath);
    }
}
