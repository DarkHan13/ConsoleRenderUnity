using UnityEngine;

namespace Game.Scripts.Console.Graphics.Math
{
    public class MyRandom
    {
        public static float RandomValueNormalDistribution()
        {
            float theta = 2 * 3.1415926f * Random.value;
            float rho = Mathf.Sqrt(-2 * Mathf.Log(Random.value));
            return rho * Mathf.Cos(theta);
        }

        public static Vector3 RandomDirection()
        {
            float x = RandomValueNormalDistribution();
            float y = RandomValueNormalDistribution();
            float z = RandomValueNormalDistribution();
            return new Vector3(x, y, z).normalized;
        }

        public static Vector3 RandomHemisphereDirection(Vector3 normal)
        {
            Vector3 dir = RandomDirection();
            return dir * Mathf.Sign(Vector3.Dot(normal, dir));
        }
    }
}