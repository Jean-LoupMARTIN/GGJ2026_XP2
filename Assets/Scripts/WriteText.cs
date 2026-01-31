using System.Collections;
using TMPro;
using UnityEngine;

public class WriteText : MonoBehaviour
{
    [SerializeField] WriteMode mode = WriteMode.Char;
    enum WriteMode
    {
        Char,
        Line
    }

    [SerializeField] Vector2 dtRange = new Vector2(0f, 0.1f);
    TMP_Text tmpText;

    [SerializeField, TextArea] string text;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    public void StartWriting(string text)
    {
        StopAllCoroutines();
        StartCoroutine(Write(text));
    }

    public void StartWriting() => StartWriting(text);

    IEnumerator Write(string text)
    {
        tmpText.text = "";

        if (mode == WriteMode.Char)
        {
            int idx = 0;

            while (idx <= text.Length-1)
            {
                yield return new WaitForSeconds(RandomExtension.Range(dtRange));
                tmpText.text += text[idx];
                idx++;
            }
        }

        else {
            string[] lines = text.Split('\n');
            int idx = 0;

            while (idx < lines.Length)
            {
                yield return new WaitForSeconds(RandomExtension.Range(dtRange));
                
                if (idx > 0)
                    tmpText.text += '\n';
                
                tmpText.text += lines[idx];
                
                idx++;
            }
        }
    }
}
