using UnityEngine;

namespace Game.Scripts.Console.Graphics.Math
{
    public struct Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction.normalized; 
        }
    }
    
    public struct HitInfo
    {
        public bool IsHit;
        public Vector3 HitPoint;
        public Vector3 Normal;
        public ShapeMaterial Material;
    }

    public struct ShapeMaterial
    {
        public float EmissionStrength;
        public Color EmissionColor;
        public Color MatColor;
        public char Texture;

        public ShapeMaterial(float emissionStrength, Color emissionColor, Color matColor, char texture)
        {
            EmissionStrength = emissionStrength;
            EmissionColor = emissionColor;
            MatColor = matColor;
            Texture = texture;
        }
    }
}