using System;
using System.Collections.Generic;
using System.Text;
using Game.Scripts.Console.Graphics.Math;
using Game.Scripts.Console.Graphics.Shapes;
using UnityEngine;
using Ray = Game.Scripts.Console.Graphics.Math.Ray;

namespace Game.Scripts.Console.Graphics
{
    public class Graphics
    {
        private static string gradient = " .:!/r(I1Z4H9W8$@";
        // private static string gradient = " .'`^,:;Il!i><~+_-?][}{1)(|/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@";
        
        public float charAspect;
        public float screenAspect;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int GradientSize { get; private set; }

                
        public Vector3 LightPos = new Vector3(0, 5, 0);
        public Camera Camera = new Camera();
        public int ReflectLimit = 1;
        public List<Shape> Shapes = new ();

        private char[] _screen;
        
        public Graphics(float charAspect, int width, int height)
        {
            this.charAspect = charAspect;
            Width = width;
            Height = height;
            screenAspect = (float)Width / height;
            GradientSize = gradient.Length;

            _screen = new char[width * height];
            
            Shapes.Add(new Sphere(new Vector3(0, 0, 5), 1f));
            for (int x = -5; x <= 5; x++)
            {
                for (int z = -5; z <= 5; z++)
                {
                    Shapes.Add(new AABB(new Vector3(x, 1, z), 0.8f));
                }
            }

        }

        public void SetPixel(int x, int y, char pixel)
        {
            if (!InBoundary(x, y)) return;
            _screen[x + y * Width] = pixel;
        }

        public string GetScreen()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    sb.Append(_screen[x + y * Width]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private bool InBoundary(int x, int y) => !(x < 0 || x >= Width || y < 0 || y >= Height);

        public void Random()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _screen[x + y * Width] = gradient[UnityEngine.Random.Range(0, GradientSize)];
                }
            }
        }


        public void Update()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Vector2 uv = new Vector2((float)i / Width * 2f - 1, (float)j / Height * 2 - 1f);
                    uv.x *= screenAspect * charAspect;
                    Vector3 rayDir = new Vector3(uv.x, uv.y, 1);
                    rayDir = Camera.RayDir(rayDir);
                    Ray ray = new Ray(Camera.position, rayDir);
                    char pixel = ' ';
                    HitInfo bestHit = new HitInfo();
                    float brightness = 0;
                    for (int k = 0; k < ReflectLimit; k++)
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
                            brightness += MathF.Max(0,
                                Vector3.Dot(bestHit.Normal, (bestHit.HitPoint - LightPos).normalized)) * (k == 0 ? 1 : 0.5f);
                            ray.Direction = Vector3.Reflect(ray.Direction, bestHit.Normal);
                            ray.Origin = bestHit.HitPoint + ray.Direction * 0.01f;
                        } else break;
                    }

                    brightness = Mathf.Clamp01((brightness));
                    pixel = gradient[(int)(brightness * (GradientSize - 1))];
                    _screen[i + j * Width] = pixel;
                }
            }

            var fpsString = (1f / Time.deltaTime).ToString("00");
            for (var i = 0; i < fpsString.Length; i++)
            {
                _screen[i] = fpsString[i];
            }

            var reflectionLimitMessage = $"Reflects: {ReflectLimit - 1}";
            var length = reflectionLimitMessage.Length;
            var startIndex = Width - length;
            for (int i = startIndex; i < Width; i++)
            {
                _screen[i] = reflectionLimitMessage[i - startIndex];
            }
        }
    }
}
