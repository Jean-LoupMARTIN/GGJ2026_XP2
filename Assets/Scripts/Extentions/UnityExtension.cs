using System.Collections.Generic;
using UnityEngine;


public static class UnityExtension
{
    public static T CacheIfNull<T>(this Component component, ref T cachedComponent) where T : Component
    {
        if (cachedComponent == null)
            cachedComponent = component.GetComponent<T>();

        return cachedComponent;
    }

    public static T CacheIfNull<T>(this GameObject gameObject, ref T cachedComponent) where T : Component
    {
        if (cachedComponent == null)
            cachedComponent = gameObject.GetComponent<T>();

        return cachedComponent;
    }

    public static bool TryGetComponentInParent<T>(this Component component, out T componentOut, bool includeInactive = true)
    {
        componentOut = component.GetComponentInParent<T>(includeInactive);
        return componentOut != null;
    }

    public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T componentOut, bool includeInactive = true)
    {
        componentOut = gameObject.GetComponentInParent<T>(includeInactive);
        return componentOut != null;
    }

    public static Transform[] GetChildren(this Transform parent)
    {
        if (!parent)
            return new Transform[0];

        List<Transform> children = new();

        foreach (Transform child in parent)
            children.Add(child);

        return children.ToArray();
    }

    public static Transform DeepFind(this Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            var result = child.DeepFind(childName);
            if (result != null)
                return result;
        }
        return null;
    }

    public static T Instantiate<T>(this Transform transform, T prefab) where T : Object
    {
        return Object.Instantiate(prefab, transform.position, transform.rotation, transform);
    }

    public static void SetX(this Transform transform, float x)
    {
        Vector3 p = transform.position;
        p.x = x;
        transform.position = p;
    }

    public static void SetY(this Transform transform, float y)
    {
        Vector3 p = transform.position;
        p.y = y;
        transform.position = p;
    }

    public static void SetZ(this Transform transform, float z)
    {
        Vector3 p = transform.position;
        p.z = z;
        transform.position = p;
    }

    public static void AddX(this Transform transform, float x)
    {
        Vector3 p = transform.position;
        p.x += x;
        transform.position = p;
    }

    public static void AddY(this Transform transform, float y)
    {
        Vector3 p = transform.position;
        p.y += y;
        transform.position = p;
    }

    public static void AddZ(this Transform transform, float z)
    {
        Vector3 p = transform.position;
        p.z += z;
        transform.position = p;
    }
}


