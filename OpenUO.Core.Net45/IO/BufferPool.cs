#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// BufferPool.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.Collections.Generic;

#endregion

namespace OpenUO.Core.IO
{
    public class BufferPool
    {
        private readonly int _bufferSize;
        private readonly Queue<byte[]> _freeBuffers;
        private readonly int _initialCapacity;
        private readonly string _name;
        private int _misses;

        public BufferPool(string name, int initialCapacity, int bufferSize)
        {
            _name = name;

            _initialCapacity = initialCapacity;
            _bufferSize = bufferSize;

            _freeBuffers = new Queue<byte[]>(initialCapacity);

            for (var i = 0; i < initialCapacity; ++i)
            {
                _freeBuffers.Enqueue(new byte[bufferSize]);
            }

            lock (Pools)
                Pools.Add(this);
        }

        public static List<BufferPool> Pools
        {
            get;
            set;
        } = new List<BufferPool>();

        public void GetInfo(out string name, out int freeCount, out int initialCapacity, out int currentCapacity, out int bufferSize, out int misses)
        {
            lock (this)
            {
                name = _name;
                freeCount = _freeBuffers.Count;
                initialCapacity = _initialCapacity;
                currentCapacity = _initialCapacity*(1 + _misses);
                bufferSize = _bufferSize;
                misses = _misses;
            }
        }

        public byte[] AcquireBuffer()
        {
            lock (this)
            {
                if (_freeBuffers.Count > 0)
                {
                    return _freeBuffers.Dequeue();
                }

                ++_misses;

                for (var i = 0; i < _initialCapacity; ++i)
                {
                    _freeBuffers.Enqueue(new byte[_bufferSize]);
                }

                return _freeBuffers.Dequeue();
            }
        }

        public void ReleaseBuffer(byte[] buffer)
        {
            if (buffer == null)
            {
                return;
            }

            lock (this)
                _freeBuffers.Enqueue(buffer);
        }

        public void Free()
        {
            lock (Pools)
                Pools.Remove(this);
        }
    }
}