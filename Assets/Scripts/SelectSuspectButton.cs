using UnityEngine;
using UnityEngine.UI;

public class SelectSuspectButton : MonoBehaviour
{
    public Image icon;
    public GameObject activeOnSelected;

    [HideInInspector] public int idx = -1;

    void Awake()
    {
        //GetComponent<Button>().onClick.AddListener(() => VigilenceDirect.Instance.SelectSuspect(idx));
    }

    public void SetSelected(bool selected)
    {
        activeOnSelected.SetActive(selected);
    }
}
