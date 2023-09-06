using System.Numerics;
using System.Xml.Linq;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Materials;

public class Shader : IDisposable
{
    private uint _handle;
    private GL GL;

    public Shader(GL gl, string vertexSrc, string fragmentSrc)
    {
        GL = gl;

        uint vertex = LoadShader(ShaderType.VertexShader, vertexSrc);
        uint fragment = LoadShader(ShaderType.FragmentShader, fragmentSrc);
        _handle = GL.CreateProgram();
        GL.AttachShader(_handle, vertex);
        GL.AttachShader(_handle, fragment);
        GL.LinkProgram(_handle);
        GL.GetProgram(_handle, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            throw new Exception($"Program failed to link with error: {GL.GetProgramInfoLog(_handle)}");
        }
        GL.DetachShader(_handle, vertex);
        GL.DetachShader(_handle, fragment);
        GL.DeleteShader(vertex);
        GL.DeleteShader(fragment);
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    private int GetUniformLocation(string name)
    {
        int location = GL.GetUniformLocation(_handle, name);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }
        return location;
    }

    public void SetUniform(string name, int value)
    {
        int location = GetUniformLocation(name);
        GL.Uniform1(location, value);
    }

    public void SetUniform(string name, uint value)
    {
        int location = GetUniformLocation(name);
        GL.Uniform1(location, value);
    }

    public void SetUniform(string name, float value)
    {
        int location = GetUniformLocation(name);
        GL.Uniform1(location, value);
    }

    public void SetUniform(string name, Vector2 value)
    {
        int location = GetUniformLocation(name);
        GL.Uniform2(location, value);
    }

    public void SetUniform(string name, Vector3 value)
    {
        int location = GetUniformLocation(name);
        GL.Uniform3(location, value);
    }

    public void SetUniform(string name, Vector4 value)
    {
        int location = GetUniformLocation(name);
        GL.Uniform4(location, value);
    }

    public unsafe void SetUniform(string name, Matrix4x4 value)
    {
        int location = GetUniformLocation(name);
        GL.UniformMatrix4(location, 1, false, (float*)&value);
    }

    public void Dispose()
    {
        GL.DeleteProgram(_handle);
    }

    private uint LoadShader(ShaderType type, string src)
    {
        uint handle = GL.CreateShader(type);
        GL.ShaderSource(handle, src);
        GL.CompileShader(handle);
        string infoLog = GL.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }

        return handle;
    }
}
