using Game.Scripts.Console.Graphics.Math;
using Ray = Game.Scripts.Console.Graphics.Math.Ray;

namespace Game.Scripts.Console.Graphics.Shapes
{
    public abstract class Shape
    {
        public ShapeMaterial Material;
        public abstract bool Intersect(Ray ray, out HitInfo hit);
    }
}