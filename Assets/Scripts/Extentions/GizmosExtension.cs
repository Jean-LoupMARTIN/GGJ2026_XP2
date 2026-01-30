using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GizmosExtension
{
    public static void DrawCircle(Vector3 center, Vector3 normal, float radius, int resolution = 30)
    {
        if (resolution <= 1)
            return;

        Quaternion rot = Quaternion.LookRotation(normal);
        float drot = 360f / resolution;

        for (int i = 0; i < resolution; i++)
        {
            Vector3 a = center + rot * Vector3.up * radius;
            rot *= Quaternion.Euler(0, 0, drot);
            Vector3 b = center + rot * Vector3.up * radius;
            Gizmos.DrawLine(a, b);
        }
    }
}
