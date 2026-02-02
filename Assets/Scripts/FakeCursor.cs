using UnityEngine;

public class FakeCursor : Singleton<FakeCursor>
{
    [SerializeField] Vector2 maxPosition = new Vector2(700, 525);

    void Start()
    {
        Cursor.visible = false;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            Cursor.visible = false;
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause)
            Cursor.visible = false;
    }

    void LateUpdate()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.x = Mathf.Clamp(pos.x, -maxPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(pos.y, -maxPosition.y, maxPosition.y);
        pos.z = 0;
        transform.position = pos;
    }
}
