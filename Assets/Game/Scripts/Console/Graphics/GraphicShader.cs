using System;
using System.Diagnostics;
using Game.Console.Helpers;
using Game.Console.Objects;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.Scripts.Console.Graphics
{
    public class GraphicShader : IDisposable
    {
        private ComputeShader _shader;
        private int _kernelIndex;
        private int _threadGroupX,  _threadGroupY;
        private bool _disposed = false;
        
        // Data for Buffers
        private PixelInfoGpu[] _screenBufferData;
        private SphereObject[] _spheres;
        
        // Buffers
        private ComputeBuffer _screenBuffer;
        ComputeBuffer _sphereBuffer = null;

        
        public GraphicShader(string path, int width, int height, float charAspect)
        {
            if (path == "") path = "RayTracing";
            var wh = width * height;
            _screenBufferData = new PixelInfoGpu[wh];
            for (var i = 0; i < _screenBufferData.Length; i++)
            {
                _screenBufferData[i] = new PixelInfoGpu();
            }
            
            _shader = Resources.Load<ComputeShader>(path);
            _kernelIndex = _shader.FindKernel("GenerateRay");

            
            ComputeShaderHelper.CreateStructureBuffer(ref _screenBuffer, _screenBufferData);
            _shader.SetFloats("screenParams", width, height, charAspect);

            _threadGroupX = Mathf.CeilToInt((width * height) / 32f);
            _threadGroupY = Mathf.CeilToInt(1f);
            
            RayTracingMaterial material = new RayTracingMaterial();
            material.SetDefaultValues();
            
            SphereObject sphereObject = new SphereObject()
            {
                material = material,
                position = new Vector3(-7, 8, 0),
                radius = 3f,
            };
            var sphereObject2 = sphereObject;
            sphereObject2.material.emissionStrength = 0;
            // sphereObject2.material.smoothness = 1;
            sphereObject2.position = new Vector3(-3, 0, 5);
            sphereObject2.radius = 1f;
            var sphereObject3 = sphereObject2;
            sphereObject3.position = new Vector3(0, 0, 5);
            sphereObject3.radius = 2f;
            var sphereObject4 = sphereObject2;
            sphereObject4.position = new Vector3(0, -23, 5);
            sphereObject4.radius = 20f;
            _spheres = new [] { sphereObject, sphereObject2, sphereObject3, sphereObject4 };
            
        }

        public PixelInfoGpu[] GetRender()
        {
            
            // ------- Buffers -------
            // Screen Buffer
            _screenBuffer.SetData(_screenBufferData);
            _shader.SetBuffer(_kernelIndex, "screen", _screenBuffer);
            
            // Object Buffer 
            // _spheres[0].position.x = Mathf.Cos(Time.time) * 5f;
            _spheres[0].radius += Time.deltaTime / 3f;
            // _spheres[1].position = new Vector3(Mathf.Cos(Time.time) * 3f, Mathf.Sin(Time.time) * 3f, 4);
            ComputeShaderHelper.CreateStructureBuffer(ref _sphereBuffer, _spheres);
            _shader.SetBuffer(_kernelIndex, "spheres", _sphereBuffer);
            _shader.SetInt("num_spheres", _spheres.Length);
            
            // ------- Dynamic Params -------
            _shader.SetInt("frame", Time.renderedFrameCount);
            
            _shader.Dispatch(_kernelIndex, _threadGroupX, _threadGroupY, 1);
            _screenBuffer.GetData(_screenBufferData);
            return _screenBufferData;
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
                    ComputeShaderHelper.Release(_screenBuffer, _sphereBuffer);
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