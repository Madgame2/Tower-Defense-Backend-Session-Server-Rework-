using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Infrastructure.Serialization
{
    public class PooledBufferWriter : IBufferWriter<byte>, IDisposable
    {
        private byte[] _buffer;
        private int _written;

        public PooledBufferWriter(int initialCapacity = 256)
        {
            _buffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
        }

        public int WrittenCount => _written;

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint);
            return _buffer.AsMemory(_written);
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint);
            return _buffer.AsSpan(_written);
        }

        public void Advance(int count)
        {
            _written += count;
        }

        private void CheckAndResizeBuffer(int sizeHint)
        {
            if (sizeHint == 0) sizeHint = 1; 
            if (_buffer!.Length - _written < sizeHint)
            {
                int newSize = Math.Max(_buffer.Length * 2, _written + sizeHint);
                var newBuffer = ArrayPool<byte>.Shared.Rent(newSize);

                _buffer.AsSpan(0, _written).CopyTo(newBuffer);
                ArrayPool<byte>.Shared.Return(_buffer);
                _buffer = newBuffer;
            }
        }

        public byte[] DetachBuffer()
        {
            var result = _buffer ?? throw new ObjectDisposedException(nameof(PooledBufferWriter));
            _buffer = null; 
            return result;
        }

        public void Dispose()
        {
            if (_buffer != null)
            {
                ArrayPool<byte>.Shared.Return(_buffer);
                _buffer = null;
            }
        }
    }
}
