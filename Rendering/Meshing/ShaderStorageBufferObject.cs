using Silk.NET.OpenGL;
using System.Runtime.CompilerServices;

namespace FainEngine_v2.Rendering.Meshing
{
    public class ShaderStorageBufferObject<T> : IDisposable where T : unmanaged
    {
        public uint Handle => _handle;
        private readonly uint _handle;

        private readonly GL _gl;

        private readonly uint _bufferSize;
        private readonly uint _elementCount;

        public unsafe ShaderStorageBufferObject(uint elementCount, BufferStorageMask storageFlags = BufferStorageMask.DynamicStorageBit)
        {
            _elementCount = elementCount;
            _bufferSize = (uint)(Unsafe.SizeOf<T>() * elementCount);

            _gl = GameGraphics.GL;
            _handle = _gl.CreateBuffer();

            _gl.NamedBufferStorage(_handle, _bufferSize, (void*)0, storageFlags);
        }

        public void SetData(ReadOnlySpan<T> data)
        {
            if (data.Length != _elementCount)
                return;

            _gl.NamedBufferSubData(_handle, 0, data);
        }

        public void FillByte(byte value)
        {
            _gl.ClearNamedBufferData(
                _handle,
                SizedInternalFormat.R8,
                PixelFormat.Red,
                PixelType.UnsignedByte,
                ref value
            );
        }

        public void Bind(uint bindingPoint)
        {
            _gl.BindBufferBase(BufferTargetARB.ShaderStorageBuffer, bindingPoint, _handle);
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(_handle);
            GC.SuppressFinalize(this);
        }

        public unsafe void Clear()
        {
            _gl.ClearNamedBufferSubData(
                _handle,
                SizedInternalFormat.R8ui,
                0,
                _bufferSize,
                PixelFormat.RedInteger,
                PixelType.UnsignedByte,
                (void*)0);
        }

        ~ShaderStorageBufferObject() => Dispose();
    }
}
