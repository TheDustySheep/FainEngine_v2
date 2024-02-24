using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Materials
{
    public abstract class TextureObject : IDisposable
    {
        protected GL _gl;
        protected uint _handle;
        protected abstract TextureTarget Target { get; }

        public TextureObject(GL gl)
        {
            _gl = gl;
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