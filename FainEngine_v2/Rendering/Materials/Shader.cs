using FainEngine_v2.Core;
using Silk.NET.OpenGL;
using System.Numerics;

namespace FainEngine_v2.Rendering.Materials;

public class Shader : GLObject
{
    private readonly uint _handle;

    public Shader(string vertexSrc, string fragmentSrc)
    {
        uint vertex = LoadShader(ShaderType.VertexShader, vertexSrc);
        uint fragment = LoadShader(ShaderType.FragmentShader, fragmentSrc);
        _handle = _GL.CreateProgram();

        _GL.AttachShader(_handle, vertex);
        ThrowOnError($"Attaching Vertex Shader: {vertexSrc}");

        _GL.AttachShader(_handle, fragment);
        ThrowOnError($"Attaching Fragment Shader: {fragmentSrc}");

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
        ThrowOnError("Binding Shader");
    }

    private int GetUniformLocation(string name)
    {
        int loc = _GL.GetUniformLocation(_handle, name);
        //if (loc == -1)
            //Console.WriteLine($"Unable to locate _shader uniform: {name}");

        ThrowOnError($"Uniform Location: {name}");
        return loc;
    }

    private void SetUniformValue<T>(string name, T value, Action<uint, int, T> set)
    {
        int loc = GetUniformLocation(name);
        set.Invoke(_handle, loc, value);
        ThrowOnError($"Setting {typeof(T)}");
    }

    public void SetUniform(string name,  int value)    => SetUniformValue(name, value, _GL.ProgramUniform1);
    public void SetUniform(string name, uint value)    => SetUniformValue(name, value, _GL.ProgramUniform1);
    public void SetUniform(string name, float value)   => SetUniformValue(name, value, _GL.ProgramUniform1);
    public void SetUniform(string name, Vector2 value) => SetUniformValue(name, value, _GL.ProgramUniform2);
    public void SetUniform(string name, Vector3 value) => SetUniformValue(name, value, _GL.ProgramUniform3);
    public void SetUniform(string name, Vector4 value) => SetUniformValue(name, value, _GL.ProgramUniform4);

    public unsafe void SetUniform(string name, Matrix4x4 value)
    {
        int location = GetUniformLocation(name);
        _GL.ProgramUniformMatrix4(_handle, location, 1, false, (float*)&value);
        ThrowOnError("Setting Matrix 4x4");
    }

    private uint LoadShader(ShaderType type, string src)
    {
        uint handle = _GL.CreateShader(type);
        _GL.ShaderSource(handle, src);
        _GL.CompileShader(handle);
        string infoLog = _GL.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling _shader of type {type}, failed with error {infoLog}");
        }

        return handle;
    }

    protected override void Release()
    {
        GLDisposalService.Delete(_handle, GLObjectType.Program);
    }
}
