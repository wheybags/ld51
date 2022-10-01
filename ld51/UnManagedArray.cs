using System;
using System.Runtime.InteropServices;

namespace ld51
{
    public unsafe class UnManagedArray<T> : IDisposable, ICloneable where T : unmanaged
    {
        public T* data { get; private set; }
        public long size { get; private set; }

        public UnManagedArray(long size)
        {
            this.size = size;
            data = (T*)Marshal.AllocHGlobal((int)size);
            Util.ReleaseAssert(data != null);
            GC.AddMemoryPressure(size);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                GC.SuppressFinalize(this);

            if (data != null)
            {
                Marshal.FreeHGlobal((IntPtr) data);
                GC.RemoveMemoryPressure(size);
                data = null;
                size = 0;
            }
        }

        public void Dispose() { Dispose(true); }

        ~UnManagedArray() { Dispose(false); }

        public T* get(int i) { return &data[i]; }

        public object Clone()
        {
            UnManagedArray<T> copy = new UnManagedArray<T>(size);
            Util.memcpy((IntPtr) copy.data, (IntPtr) data, size);
            return copy;
        }
    }
}