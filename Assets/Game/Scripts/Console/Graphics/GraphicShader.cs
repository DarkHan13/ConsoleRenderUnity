using System;
using System.Diagnostics;
using Game.Console.Objects;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.Scripts.Console.Graphics
{
    public class GraphicShader : IDisposable
    {
        private ComputeShader _shader;
        private int _kernelIndex;
        private ComputeBuffer _screenBuffer;
        private PixelInfoGpu[] _resultScreenBufferData;
        private int _threadGroupX,  _threadGroupY;
        private bool _disposed = false;
        
        public GraphicShader(string path, int width, int height, float charAspect)
        {
            if (path == "") path = "RayTracing";
            var wh = width * height;
            _resultScreenBufferData = new PixelInfoGpu[wh];
            for (var i = 0; i < _resultScreenBufferData.Length; i++)
            {
                _resultScreenBufferData[i] = new PixelInfoGpu();
            }
            
            _shader = Resources.Load<ComputeShader>(path);
            _kernelIndex = _shader.FindKernel("GenerateRay");
            _screenBuffer = new ComputeBuffer(wh, PixelInfoGpu.GetSize());
            _screenBuffer.SetData(new PixelInfoGpu[wh]);
            _shader.SetFloats("screenParams", width, height, charAspect);

            _threadGroupX = Mathf.CeilToInt((width * height) / 32f);
            _threadGroupY = Mathf.CeilToInt(1f);

            
        }

        public PixelInfoGpu[] GetRender()
        {
            // Todo: Create ShaderHelper
            
            // Screen Buffer
            _screenBuffer.SetData(_resultScreenBufferData);
            _shader.SetBuffer(_kernelIndex, "screen", _screenBuffer);
            
            // Object Buffer todo: Release all buffers
            RayTracingMaterial material = new RayTracingMaterial();
            material.SetDefaultValues();
            SphereObject sphereObject = new SphereObject()
            {
                material = material,
                position = new Vector3(0, 0, 5),
                radius = 1f,
            };
            ComputeBuffer sphereBuffer =
                new ComputeBuffer(1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(SphereObject)));
            sphereBuffer.SetData(new[] {sphereObject});
            _shader.SetBuffer(_kernelIndex, "spheres", sphereBuffer);
            _shader.SetInt("num_spheres", 1);
            
            // dynamic params
            _shader.SetInt("frame", Time.renderedFrameCount);
            
            _shader.Dispatch(_kernelIndex, _threadGroupX, _threadGroupY, 1);
            _screenBuffer.GetData(_resultScreenBufferData);
            Debug.Log(_resultScreenBufferData.Length);
            return _resultScreenBufferData;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    // Dispose managed resources (buffers)
                    _screenBuffer?.Release();
                }
                _disposed = true;
            }
        }

        ~GraphicShader() {
            Dispose(false);
        }
    }
    
    public struct PixelInfoGpu
    {
        public float Brightness;
        public Color Color;

        public PixelInfoGpu(float brightness, Color color)
        {
            Brightness = brightness;
            Color = color;
        }

        public static int GetSize()
        {
            return sizeof(float) + sizeof(float) * 4;
        }
    }
}