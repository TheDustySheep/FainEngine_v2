using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Materials
{
    public abstract class TextureObject : IDisposable
    {
        protected GL _gl;
        protected uint _handle;
        public uint Handle => _handle;

        protected abstract TextureTarget Target { get; }

        public TextureObject()
        {
            _gl = GameGraphics.GL;
            _handle = _gl.GenTexture();
        }

        public void Dispose()
        {
            _gl.DeleteTexture(_handle);
        }
    }
}