using System;
using System.Diagnostics;
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
            _screenBuffer.SetData(_resultScreenBufferData);
            _shader.SetBuffer(_kernelIndex, "screen", _screenBuffer);
            
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