using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VSRapportResult : MonoBehaviour
{
    [SerializeField] TMP_Text tmpText;
    [SerializeField] Button closeButton;
    [SerializeField, TextArea] string textStart, textEnd;
    [SerializeField, TextArea] string[] textValids;
    [SerializeField, TextArea] string[] textWrongs;
    [SerializeField] GameObject finalPopup;


    bool valid = false;
    int textWrongsIdx = 0;

    void Awake()
    {
       closeButton.onClick.AddListener(OnClick);
    }

    public void SetResult(bool valid)
    {
        this.valid = valid;

        if (valid)
            tmpText.text = textStart + textValids[VigilenceDirect.Instance.crtLevelIdx] + textEnd;

        else {
            tmpText.text = textStart + textWrongs[textWrongsIdx] + textEnd;
            textWrongsIdx++;
            textWrongsIdx %= textWrongs.Length;
        }
    }

    void OnClick()
    {
        gameObject.SetActive(false);

        if (valid)
        {
            if (VigilenceDirect.Instance.crtLevelIdx == VigilenceDirect.Instance.levels.Length - 1)
            {
                finalPopup.SetActive(true);
                SoundManager.Instance.StartFadeInOut();
            }

            else VigilenceDirect.Instance.NextLevel();
        }
    }
}
