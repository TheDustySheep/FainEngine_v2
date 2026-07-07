using Silk.NET.OpenGL;

namespace FainEngine_v2.Rendering.RenderObjects
{
    internal class RenderBufferObject : IDisposable
    {
        protected uint _handle;
        protected GL _gl;

        public RenderBufferObject(
            GL gL,
            int width,
            int height,
            InternalFormat format = InternalFormat.Depth24Stencil8)
        {
            _gl = gL;
            _handle = _gl.CreateRenderbuffer();

            Bind();

            _gl.RenderbufferStorage(
                RenderbufferTarget.Renderbuffer,
                format,
                (uint)width,
                (uint)height);
        }

        public void FrameBufferRenderBuffer(FramebufferAttachment attachment = FramebufferAttachment.DepthStencilAttachment)
        {
            _gl.FramebufferRenderbuffer(
                FramebufferTarget.Framebuffer,
                attachment,
                RenderbufferTarget.Renderbuffer,
                _handle);
        }

        public void Bind()
        {
            _gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _handle);
        }

        public void Unbind()
        {
            _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            _gl.DeleteRenderbuffer(_handle);
        }
    }
}
