﻿using System.Buffers;

namespace Noveler.Compiler
{
    internal class PooledList<T> : IDisposable
    {
        T[]? _buffer;
        int _count;

        public int Count => _count;
        public int Capacity => _buffer?.Length ?? 0;
        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _buffer![index];
            }
            set
            {
                if ((uint)index >= (uint)_count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                _buffer![index] = value;
            }
        }

        private PooledList()
        { }

        public void Add(T item)
        {
            if (_buffer == null)
                throw new Exception("Pooled list instance has already been returned.");


            // if capacity has been reached, rent a buffer twice the size
            if (_buffer.Length == _count)
            {
                var newBuffer = ArrayPool<T>.Shared.Rent(_buffer.Length * 2);
                _buffer.CopyTo(newBuffer.AsSpan());

                ArrayPool<T>.Shared.Return(_buffer);
                _buffer = newBuffer;
            }

            _buffer[_count] = item;
            _count++;
        }

        public void EnsureCapacity(int capacity)
        {
            if (_buffer == null)
            {
                _buffer = ArrayPool<T>.Shared.Rent(capacity);
                _count = 0;
                return;
            }

            if (_buffer.Length >= capacity)
                return;

            var newBuffer = ArrayPool<T>.Shared.Rent(Math.Max(_buffer.Length * 2, capacity));
            _buffer.CopyTo(newBuffer.AsSpan());

            ArrayPool<T>.Shared.Return(_buffer);
            _buffer = newBuffer;
        }

        public Span<T> AsSpan()
        {
            return new Span<T>(_buffer, 0, _count);
        }

        public Span<T> AsCapacitySpan()
        {
            return new Span<T>(_buffer);
        }

        public static PooledList<T> Rent(int minimumSize)
        {
            return new PooledList<T>
            {
                _buffer = ArrayPool<T>.Shared.Rent(minimumSize),
                _count = default
            };
        }

        public static void Return(PooledList<T> instance)
        {
            if (instance._buffer == null)
                throw new Exception("Pooled list instance has already been returned.");

            ArrayPool<T>.Shared.Return(instance._buffer);
            instance._buffer = null;
            instance._count = 0;
        }

        /// <summary>
        /// Disposes internal resources, the instance should no longer be used after calling this function.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ~PooledList()
        {
            if (_buffer != null)
            {
                Return(this);
            }
        }
    }
}