using System.Collections.Generic;
using UnityEngine;

namespace Game.Console.Helpers
{
    public static class ComputeShaderHelper
    {
        public static void Release(params ComputeBuffer[] computeBuffers)
        {
            foreach (var computeBuffer in computeBuffers)
                computeBuffer?.Release();
            
        }

        #region Creation of Buffers
        
        public static void CreateStructureBuffer<T>(ref ComputeBuffer buffer, int count)
        {
            count = Mathf.Max(1, count);
            int stride = GetStride<T>();
            bool createNewBuffer =
                buffer == null || !buffer.IsValid() || buffer.count != count || buffer.stride != stride;
            if (createNewBuffer)
            {
                Release(buffer);
                buffer = new ComputeBuffer(count, stride, ComputeBufferType.Structured);
            }
        }
        
        public static void CreateStructureBuffer<T>(ref ComputeBuffer buffer, T[] data)
        {
            int count = Mathf.Max(1, data.Length);
            CreateStructureBuffer<T>(ref buffer, count);
            buffer.SetData(data);
        }

        public static void CreateStructureBuffer<T>(ref ComputeBuffer buffer, List<T> data) where T: struct
        {
            int count = Mathf.Max(1, data.Count);
            CreateStructureBuffer<T>(ref buffer, count);
            buffer.SetData(data);
        }

        
        public static ComputeBuffer CreateStructuredBuffer<T>(T[] data)
        {
            var buffer = new ComputeBuffer(data.Length, GetStride<T>());
            buffer.SetData(data);
            return buffer;
        }

        public static ComputeBuffer CreateStructuredBuffer<T>(int count)
        {
            return new ComputeBuffer(count, GetStride<T>());
        }

        public static int GetStride<T>() => System.Runtime.InteropServices.Marshal.SizeOf<T>();

        #endregion
    }
}