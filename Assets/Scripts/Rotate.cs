using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] Vector3 rotationSpeed = new Vector3(0, 0, 10);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
