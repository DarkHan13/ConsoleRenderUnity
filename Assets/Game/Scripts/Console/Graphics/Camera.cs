using UnityEngine;

namespace Game.Scripts.Console.Graphics
{
    public class Camera
    {
        public Vector3 position = Vector3.zero;
        public float yaw = 0f; // left/right rotation
        public float pitch = 0f; // up/down rotation


        public Vector3 Forward => RayDir(Vector3.forward).normalized;
        public Vector3 Right => RayDir(Forward, 90, 0);
        

        // public void UpdateRotation()
        // {
        //     _forward = new Vector3
        //     (
        //         Mathf.Cos(pitch) * Mathf.Sin(yaw),
        //         Mathf.Sin(pitch),
        //         Mathf.Cos(pitch) * Mathf.Cos(yaw)
        //     ).normalized;
        // }

        public Vector3 RayDir(Vector3 v)
        {
            v = Quaternion.AngleAxis(pitch, Vector3.right) * v;
            v = Quaternion.AngleAxis(yaw, Vector3.up) * v;
            return v;
        }

        public Vector3 RayDir(Vector3 v, float yaw, float pitch)
        {
            v = Quaternion.AngleAxis(pitch, Vector3.right) * v;
            v = Quaternion.AngleAxis(yaw, Vector3.up) * v;
            return v;
        }
    }
}