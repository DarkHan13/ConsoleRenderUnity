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

        private char[] _screen;
        
        public Graphics(float charAspect, int width, int height)
        {
            this.charAspect = charAspect;
            Width = width;
            Height = height;
            screenAspect = (float)Width / height;
            GradientSize = gradient.Length;

            _screen = new char[width * height];
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
        
        public Vector3 LightPos = new Vector3(0, 5, 0);
        public Camera Camera = new Camera();

        public void Update()
        {
            List<Shape> shapes = new List<Shape>() 
            {
                // new AABB(new Vector3(0, 0, 0), 1), 
                new Sphere(new Vector3(0, 0, 5), 1f),
            };
            for (int x = -5; x <= 5; x++)
            {
                for (int z = -5; z <= 5; z++)
                {
                    shapes.Add(new AABB(new Vector3(x, 1, z), 0.8f));
                }
            }
            

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
                    float minDist = Single.PositiveInfinity;
                    foreach (var shape in shapes)
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
                        float brightness = MathF.Max(0,
                            Vector3.Dot(bestHit.Normal, (bestHit.HitPoint - LightPos).normalized));
                        pixel = gradient[(int)(brightness * (GradientSize - 1))];
                    }

                    _screen[i + j * Width] = pixel;
                }
            }        
        }
    }
}
