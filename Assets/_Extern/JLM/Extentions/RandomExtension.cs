using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RandomExtension
{
    public static T PickRandom<T>(this T[] array) => array.Length == 0 ? default : array[(int)(Random.value * array.Length)];
    public static T PickRandom<T>(this List<T> list) => list.Count == 0 ? default : list[(int)(Random.value * list.Count)];
    public static T PickRandom<T>(this IEnumerable<T> ienum) => ienum.ToArray().PickRandom();


    public static float Range(Vector2 range) => Random.Range(range.x, range.y);
    

    public static Vector2 RandomVector2(float min, float max) => new Vector2(Random.Range(min, max), Random.Range(min, max));
    public static Vector3 RandomVector3(float min, float max) => new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
    
    public static Vector2 RandomPointInCircle()
    {
        while (true)
        {
            Vector2 pnt = RandomVector2(-1, 1);
            
            if (pnt.sqrMagnitude <= 1)
                return pnt;
        }
    }

    public static Vector3 RandomPointInSphere()
    {
        while (true)
        {
            Vector3 pnt = RandomVector3(-1, 1);
            
            if (pnt.sqrMagnitude <= 1)
                return pnt;
        }
    }
}


