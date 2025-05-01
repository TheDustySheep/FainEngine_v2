using FainEngine_v2.Core;
using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Materials
{
    public abstract class TextureObject : IDisposable
    {
        protected GL _gl;
        protected uint _handle;
        protected abstract TextureTarget Target { get; }

        public TextureObject()
        {
            _gl = GameGraphics.GL;
            _handle = _gl.GenTexture();
        }

        public void Bind()
        {
            _gl.BindTexture(Target, _handle);
        }

        public void Dispose()
        {
            _gl.DeleteTexture(_handle);
        }
    }
}