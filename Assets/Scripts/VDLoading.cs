using UnityEngine;

public class VDLoading : MonoBehaviour
{
    [SerializeField] WriteText writeText;
    [SerializeField, TextArea] string loadingText;

    public void StartLoading()
    {
        writeText.StartWriting(loadingText);
    }
}
