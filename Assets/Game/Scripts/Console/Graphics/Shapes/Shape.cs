using Game.Scripts.Console.Graphics.Math;

namespace Game.Scripts.Console.Graphics.Shapes
{
    public interface Shape
    {
        public bool Intersect(Ray ray, out HitInfo hit);
    }
}