﻿using UnityEngine;

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
        public Color Color;
    }
}