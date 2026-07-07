using FainEngine_v2.Core;
using Silk.NET.OpenGL;
using System.Runtime.CompilerServices;

namespace FainEngine_v2.Rendering.Meshing
{
    public class ShaderStorageBufferObject<T> : IDisposable where T : unmanaged
    {
        public uint Handle => _handle;
        private readonly uint _handle;

        private readonly GL _GL;

        private readonly uint _bufferSize;
        private readonly uint _elementCount;

        public unsafe ShaderStorageBufferObject(uint elementCount, BufferStorageMask storageFlags = BufferStorageMask.DynamicStorageBit)
        {
            _elementCount = elementCount;
            _bufferSize = (uint)(Unsafe.SizeOf<T>() * elementCount);

            _GL = GameGraphics.GL;
            _handle = _GL.CreateBuffer();

            _GL.NamedBufferStorage(_handle, _bufferSize, (void*)0, storageFlags);
        }

        public void SetData(ReadOnlySpan<T> data)
        {
            if (data.Length != _elementCount)
                return;

            _GL.NamedBufferSubData(_handle, 0, data);
        }

        public void FillByte(byte value)
        {
            _GL.ClearNamedBufferData(
                _handle,
                SizedInternalFormat.R8,
                PixelFormat.Red,
                PixelType.UnsignedByte,
                ref value
            );
        }

        public void Bind(uint bindingPoint)
        {
            _GL.BindBufferBase(BufferTargetARB.ShaderStorageBuffer, bindingPoint, _handle);
        }

        public unsafe void Clear()
        {
            _GL.ClearNamedBufferSubData(
                _handle,
                SizedInternalFormat.R8ui,
                0,
                _bufferSize,
                PixelFormat.RedInteger,
                PixelType.UnsignedByte,
                (void*)0);
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            GLDisposalService.Enqueue(() => _GL.DeleteBuffer(_handle));

            GC.SuppressFinalize(this);
        }

        ~ShaderStorageBufferObject()
        {
            if (_disposed)
                return;

            _disposed = true;

            GLDisposalService.Enqueue(() => _GL.DeleteBuffer(_handle));
        }
    }
}
