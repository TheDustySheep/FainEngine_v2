using FainEngine_v2.Core;
using Silk.NET.OpenGL;
using System.Numerics;

namespace FainEngine_v2.Rendering.Materials;

public class Shader : IDisposable
{
    private readonly uint _handle;
    private readonly GL _GL;

    public Shader(string vertexSrc, string fragmentSrc)
    {
        _GL = GameGraphics.GL;

        uint vertex = LoadShader(ShaderType.VertexShader, vertexSrc);
        uint fragment = LoadShader(ShaderType.FragmentShader, fragmentSrc);
        _handle = _GL.CreateProgram();
        _GL.AttachShader(_handle, vertex);
        _GL.AttachShader(_handle, fragment);
        _GL.LinkProgram(_handle);
        _GL.GetProgram(_handle, ProgramPropertyARB.LinkStatus, out var status);
        if (status == 0)
        {
            throw new Exception($"Program failed to link with error: {_GL.GetProgramInfoLog(_handle)}");
        }
        _GL.DetachShader(_handle, vertex);
        _GL.DetachShader(_handle, fragment);
        _GL.DeleteShader(vertex);
        _GL.DeleteShader(fragment);
    }

    public void Use()
    {
        _GL.UseProgram(_handle);
    }

    private int GetUniformLocation(string name) => _GL.GetUniformLocation(_handle, name);

    public void SetUniform(string name, int value)
    {
        int location = GetUniformLocation(name);
        _GL.ProgramUniform1(_handle, location, value);
    }

    public void SetUniform(string name, uint value)
    {
        int location = GetUniformLocation(name);
        _GL.ProgramUniform1(_handle, location, value);
    }

    public void SetUniform(string name, float value)
    {
        int location = GetUniformLocation(name);
        _GL.ProgramUniform1(_handle, location, value);
    }

    public void SetUniform(string name, Vector2 value)
    {
        int location = GetUniformLocation(name);
        _GL.ProgramUniform2(_handle, location, value);
    }

    public void SetUniform(string name, Vector3 value)
    {
        int location = GetUniformLocation(name);
        _GL.ProgramUniform3(_handle, location, value);
    }

    public void SetUniform(string name, Vector4 value)
    {
        int location = GetUniformLocation(name);
        _GL.ProgramUniform4(_handle, location, value);
    }

    public unsafe void SetUniform(string name, Matrix4x4 value)
    {
        int location = GetUniformLocation(name);
        _GL.ProgramUniformMatrix4(_handle, location, 1, false, (float*)&value);
    }

    private uint LoadShader(ShaderType type, string src)
    {
        uint handle = _GL.CreateShader(type);
        _GL.ShaderSource(handle, src);
        _GL.CompileShader(handle);
        string infoLog = _GL.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
        }

        return handle;
    }

    private bool _disposed;
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        GLDisposalService.Enqueue(() => _GL.DeleteProgram(_handle));

        GC.SuppressFinalize(this);
    }

    ~Shader()
    {
        if (_disposed)
            return;

        _disposed = true;

        GLDisposalService.Enqueue(() => _GL.DeleteProgram(_handle));
    }
}
