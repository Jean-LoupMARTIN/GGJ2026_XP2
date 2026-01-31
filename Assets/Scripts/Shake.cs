using UnityEngine;

public class Shake : MonoBehaviour
{
    [SerializeField] Vector3 vector = new Vector3(1, 1, 0);
    [SerializeField] float coef = 1;
    [SerializeField] float noiseScale = 1;
    [SerializeField] Transform aze;
    Vector3 offset;

    void Awake()
    {
        offset = RandomExtension.RandomVector3(0, 10000);
    }

    void Update()
    {
        aze.localPosition = new Vector3(
            (Mathf.PerlinNoise1D(offset.x + Time.time * noiseScale) - 0.5f) * vector.x * coef,
            (Mathf.PerlinNoise1D(offset.y + Time.time * noiseScale) - 0.5f) * vector.y * coef,
            (Mathf.PerlinNoise1D(offset.z + Time.time * noiseScale) - 0.5f) * vector.z * coef);
    }
}
