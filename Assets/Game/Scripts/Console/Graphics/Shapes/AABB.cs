using System;
using Game.Scripts.Console.Graphics.Math;
using UnityEngine;
using Ray = Game.Scripts.Console.Graphics.Math.Ray;

namespace Game.Scripts.Console.Graphics.Shapes
{
    public class AABB : Shape
    { 
        public Vector3 Center { get; private set; }
        public Vector3 Min, Max;  // Минимальная и максимальная точки куба

        public AABB(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
            Center = new Vector3((Max.x + Min.x) / 2f, (Max.y + Min.y) / 2f, (Max.z + Min.z) / 2f);
        }

        public AABB(Vector3 center, float boxSide)
        {
            Center = center;
            var half = boxSide / 2;
            Min = new Vector3(Center.x - half, Center.y - half, Center.z - half);
            Max = new Vector3(Center.x + half, Center.y + half, Center.z + half);
        }

        public void SetCenter(Vector3 center)
        {
            var oldCenter = Center;
            Center = center;
            var offset = Center - oldCenter;
            Min += offset;
            Max += offset;
        }

        public void Move(Vector3 moveVector) => SetCenter(Center + moveVector);
        
        public bool Intersect(Ray ray, out HitInfo hitInfo)
        {
            float tMin = (Min.x - ray.Origin.x) / ray.Direction.x;
            float tMax = (Max.x - ray.Origin.x) / ray.Direction.x;
            if (tMin > tMax) (tMin, tMax) = (tMax, tMin);

            float tyMin = (Min.y - ray.Origin.y) / ray.Direction.y;
            float tyMax = (Max.y - ray.Origin.y) / ray.Direction.y;
            if (tyMin > tyMax) (tyMin, tyMax) = (tyMax, tyMin);

            hitInfo = new HitInfo();
            if ((tMin > tyMax) || (tyMin > tMax))
            {
                hitInfo.IsHit = false;
                return false;
            }

            tMin = MathF.Max(tMin, tyMin);
            tMax = MathF.Min(tMax, tyMax);

            float tzMin = (Min.z - ray.Origin.z) / ray.Direction.z;
            float tzMax = (Max.z - ray.Origin.z) / ray.Direction.z;
            if (tzMin > tzMax) (tzMin, tzMax) = (tzMax, tzMin);

            if ((tMin > tzMax) || (tzMin > tMax))
            {
                hitInfo.IsHit = false;
                return false;
            }

            tMin = MathF.Max(tMin, tzMin);
            tMax = MathF.Min(tMax, tzMax);

            if (tMin < 0) return false;
            // Вычисляем точку пересечения
            hitInfo.HitPoint = ray.Origin + ray.Direction * tMin;

            // Определяем нормаль в точке пересечения
            hitInfo.Normal = GetNormalAtPoint(hitInfo.HitPoint);
            hitInfo.IsHit = true;
            hitInfo.Color = new (0, 1f, 0f);
            return true;
        }
        
        private Vector3 GetNormalAtPoint(Vector3 point)
        {
            // Определение нормали в зависимости от пересечённой грани
            if (MathF.Abs(point.x - Min.x) < 0.001f) return new Vector3(-1, 0, 0); // Левая грань
            if (MathF.Abs(point.x - Max.x) < 0.001f) return new Vector3(1, 0, 0);  // Правая грань
            if (MathF.Abs(point.y - Min.y) < 0.001f) return new Vector3(0, -1, 0); // Нижняя грань
            if (MathF.Abs(point.y - Max.y) < 0.001f) return new Vector3(0, 1, 0);  // Верхняя грань
            if (MathF.Abs(point.z - Min.z) < 0.001f) return new Vector3(0, 0, -1); // Задняя грань
            if (MathF.Abs(point.z - Max.z) < 0.001f) return new Vector3(0, 0, 1);  // Передняя грань

            return new Vector3(0, 0, 0); // На случай ошибки
        }
    }
}