using Game.Scripts.Console.Graphics.Math;
using UnityEngine;
using Ray = Game.Scripts.Console.Graphics.Math.Ray;

namespace Game.Scripts.Console.Graphics.Shapes
{
    public class Sphere : Shape
    {
        public Vector3 Center;
        public float Radius;

        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public override bool Intersect(Ray ray, out HitInfo hit)
        {
            hit = new HitInfo();
        
            Vector3 oc = ray.Origin - Center;
            float a = Vector3.Dot(ray.Direction, ray.Direction);
            float b = 2f * Vector3.Dot(oc, ray.Direction);
            float c = Vector3.Dot(oc, oc) - Radius * Radius;
            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                return false;
            }

            discriminant = Mathf.Sqrt(discriminant);
            float t1 = (-b - discriminant) / (2f * a);
            float t2 = (-b + discriminant) / (2f * a);

            // Выбираем наименьший положительный корень
            float t = (t1 > 0) ? t1 : t2;

            if (t < 0)
            {
                return false; // Оба корня отрицательные — сфера позади камеры
            }
            hit.IsHit = true;
            hit.HitPoint = ray.Origin + ray.Direction * t;
            hit.Normal = (hit.HitPoint - Center).normalized;
            hit.Material = Material;
            return true;
        }
    }
}