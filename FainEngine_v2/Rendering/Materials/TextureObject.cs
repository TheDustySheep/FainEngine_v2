using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.Materials
{
    public abstract class TextureObject : GLObject
    {
        protected uint _handle;
        public uint Handle => _handle;

        protected abstract TextureTarget Target { get; }

        public TextureObject() : base()
        {
            _handle = _GL.GenTexture();
        }

        protected override void Release()
        {
            GLDisposalService.Delete(_handle, GLObjectType.Texture);
        }
    }
}