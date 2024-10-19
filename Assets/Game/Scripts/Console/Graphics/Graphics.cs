using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Game.Scripts.Console.Graphics.Math;
using Game.Scripts.Console.Graphics.Shapes;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using Ray = Game.Scripts.Console.Graphics.Math.Ray;

namespace Game.Scripts.Console.Graphics
{
    public class Graphics
    {
        private static string gradient = " .:!/(1rIZ98$@4HW";
        // private static string gradient = " .'`^,:;Il!i><~+_-?][}{1)(|/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@";

        public float charAspect;
        public float screenAspect;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int GradientSize { get; private set; }


        public Vector3 LightPos = new Vector3(3, 5, 0);
        public Camera Camera = new Camera();
        public int ReflectLimit = 1;
        public List<Shape> Shapes = new();

        private struct PixelInfo
        {
            public char S;
            public Color Color;

            public PixelInfo(char s, Color color)
            {
                S = s;
                Color = color;
            }
        }
        


        private PixelInfo[] _screen;
        private PixelInfoGpu[] _screenBufferData;
        private GraphicShader _shaderManager;

        public Graphics(float charAspect, int width, int height)
        {
            this.charAspect = charAspect;
            Width = width;
            Height = height;
            screenAspect = (float)Width / height;
            GradientSize = gradient.Length;

            _screen = new PixelInfo[width * height];
            _screenBufferData = new PixelInfoGpu[width * height];
            for (int i = 0; i < width * height; i++)
            {
                _screen[i] = new PixelInfo();
            }

            Shapes.Add(new Sphere(new Vector3(0, 4, 5), 2f));
            Shapes[0].Material = new ShapeMaterial(1f, Color.white, Color.black, '/');

            var aabbMat = new ShapeMaterial(0f, Color.black, Color.green, '.');
            for (int x = -5; x <= 5; x++)
            {
                for (int z = -5; z <= 5; z++)
                {
                    var aabb = new AABB(new Vector3(x, -1, z), 0.8f);
                    aabb.Material = aabbMat;
                    Shapes.Add(aabb);
                    
                }
            }

            _shaderManager = new GraphicShader("", width, height, charAspect);
            var newData = _shaderManager.GetRender();
            
            for (int i = 0; i < width * height; i++)
            {
                _screen[i].Color = Color.white;
                try
                {
                    _screen[i].S = gradient[(int)((GradientSize - 1) * (Mathf.Clamp01(newData[i].Brightness)))];
                }
                catch (Exception e)
                {
                    Debug.Log($"{i} {(int)((GradientSize - 1) * (Mathf.Clamp01(newData[i].Brightness)))}");
                    // throw e;
                }
            }
        }



        
        public string GetScreen()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var pixel = _screen[x + y * Width];
                    if (pixel.S == ' ') sb.Append(_screen[x + y * Width].S);
                    else
                    {
                        var colorHex = ColorUtility.ToHtmlStringRGB(pixel.Color);
                        sb.Append($"<color=#{colorHex}>{pixel.S}</color>");
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private bool InBoundary(int x, int y) => !(x < 0 || x >= Width || y < 0 || y >= Height);
        
        public void Update()
        {
            return;
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Vector2 uv = new Vector2((float)i / Width * 2f - 1, (float)j / Height * 2 - 1f);
                    uv.x *= screenAspect * charAspect;
                    uv.y *= -1;
                    Vector3 rayDir = new Vector3(uv.x, uv.y, 1);
                    rayDir = Camera.RayDir(rayDir);
                    Ray ray = new Ray(Camera.position, rayDir);
                    char pixel = ' ';
                    int pixelIndex = i + j * Width;
                    HitInfo bestHit = new HitInfo();

                    Color rayColor = Color.white;
                    Color incomingLight = Color.black;
                    for (int k = 0; k <= ReflectLimit; k++)
                    {
                        bestHit.IsHit = false;

                        float minDist = Single.PositiveInfinity;
                        foreach (var shape in Shapes)
                        {
                            if (shape.Intersect(ray, out var hit))
                            {
                                var dist = (ray.Origin - hit.HitPoint).sqrMagnitude;
                                if (dist < minDist)
                                {
                                    minDist = dist;
                                    bestHit = hit;
                                }
                            }
                        }

                        if (bestHit.IsHit)
                        {
                            // brightness += MathF.Max(0,
                            //     -Vector3.Dot(bestHit.Normal, (bestHit.HitPoint - LightPos).normalized)) * (1f / (k + 1));
                            if (k == 0) pixel = bestHit.Material.Texture;
                            incomingLight += bestHit.Material.EmissionStrength * bestHit.Material.EmissionColor * rayColor /* * (1f / (k + 1))*/;
                            rayColor *= bestHit.Material.MatColor;

                            // ray.Direction = Vector3.Reflect(ray.Direction, bestHit.Normal);
                            ray.Direction = MyRandom.RandomHemisphereDirection(bestHit.Normal);
                            ray.Origin = bestHit.HitPoint + ray.Direction * 0.01f;
                        }
                        else
                        {
                            if (k == 0) pixel = '#';
                            Color skyColor = new Color(0.3f, 0.6f, 1f);
                            // Color sunColor = new Color(0.95f, 0.9f, 1f);
                            // sunColor *= Mathf.Max(0, Vector3.Dot(ray.Direction, -(ray.Origin - LightPos).normalized));
                            // sunColor *= sunColor;
                            // sunColor.r = Mathf.Pow(sunColor.r, 16);
                            // sunColor.g = Mathf.Pow(sunColor.g, 16);
                            // sunColor.b = Mathf.Pow(sunColor.b, 16);
                            
                            // incomingLight += (skyColor /*+ sunColor*/) * (1f / (k + 1));
                            break;
                        }
                    }

                    // pixel = gradient[(int)(brightness * (GradientSize - 1))];
                    _screen[pixelIndex].S = pixel;
                    _screen[pixelIndex].Color = incomingLight;
                    // _screen[pixelIndex].color = Color.Lerp(incomingLight, _screen[pixelIndex].color, 0.5f);
                }
            }

            var fpsString = (1f / Time.deltaTime).ToString("00");
            for (var i = 0; i < fpsString.Length; i++)
            {
                _screen[i].S = fpsString[i];
                _screen[i].Color = Color.white;
            }

            var reflectionLimitMessage = $"Reflects: {ReflectLimit}";
            var length = reflectionLimitMessage.Length;
            var startIndex = Width - length;
            for (int i = startIndex; i < Width; i++)
            {
                _screen[i].S = reflectionLimitMessage[i - startIndex];
                _screen[i].Color = Color.white;
            }
        }

        public void OnDestroy()
        {
            _shaderManager.Dispose();
        }

        public void RayDestroy()
        {
            var ray = new Ray(Camera.position, Camera.Forward);
            int index = -1;
            float minDist = Single.PositiveInfinity;
            for (var i = 0; i < Shapes.Count; i++)
            {
                var shape = Shapes[i];
                if (shape.Intersect(ray, out var hit))
                {
                    var dist = (ray.Origin - hit.HitPoint).sqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        index = i;
                    }
                }
            }

            if (index != -1) Shapes.RemoveAt(index);
        }
    }
}