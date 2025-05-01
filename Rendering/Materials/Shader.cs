using FainEngine_v2.Core;
using Silk.NET.OpenGL;
using System.Numerics;

namespace FainEngine_v2.Rendering.Materials;

public class Shader : IDisposable
{
    private readonly uint _handle;
    private readonly GL _gl;

    public Shader(string vertexSrc, string fragmentSrc)
    {
        _gl = GameGraphics.GL;

        uint vertex = LoadShader(ShaderType.VertexShader, vertexSrc);
        uint fragment = LoadShader(ShaderType.FragmentShader, fragmentSrc);
        _handle = _gl.CreateProgram();
        _gl.AttachShader(_handle, vertex);
        _gl.AttachShader(_handle, fragment);
        _gl.LinkProgram(_handle);
        _gl.GetProgram(_handle, ProgramPropertyARB.LinkStatus, out var status);
        if (status == 0)
        {
            throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(_handle)}");
        }
        _gl.DetachShader(_handle, vertex);
        _gl.DetachShader(_handle, fragment);
        _gl.DeleteShader(vertex);
        _gl.DeleteShader(fragment);
    }

    public void Use()
    {
        _gl.UseProgram(_handle);
    }

    private int GetUniformLocation(string name)
    {
        int location = _gl.GetUniformLocation(_handle, name);
        //if (location == -1)
        //{
        //    throw new Exception($"{name} uniform not found on shader.");
        //}
        return location;
    }

    public void SetUniform(string name, int value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform1(location, value);
    }

    public void SetUniform(string name, uint value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform1(location, value);
    }

    public void SetUniform(string name, float value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform1(location, value);
    }

    public void SetUniform(string name, Vector2 value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform2(location, value);
    }

    public void SetUniform(string name, Vector3 value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform3(location, value);
    }

    public void SetUniform(string name, Vector4 value)
    {
        int location = GetUniformLocation(name);
        _gl.Uniform4(location, value);
    }

    public unsafe void SetUniform(string name, Matrix4x4 value)
    {
        int location = GetUniformLocation(name);
        _gl.UniformMatrix4(location, 1, false, (float*)&value);
    }

    public void Dispose()
    {
        _gl.DeleteProgram(_handle);
    }

    private uint LoadShader(ShaderType type, string src)
    {
        uint handle = _gl.CreateShader(type);
        _gl.ShaderSource(handle, src);
        _gl.CompileShader(handle);
        string infoLog = _gl.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }

        return handle;
    }
}
