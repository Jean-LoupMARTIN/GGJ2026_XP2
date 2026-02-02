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
    [HideInInspector] public TMP_Text tmpText;
    public AudioClip clip;
    public AudioSource source;
    

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    public void StartWriting(string text)
    {
        StopAllCoroutines();
        StartCoroutine(Write(text));
    }


    IEnumerator Write(string text)
    {
        tmpText.text = "";
        int i = 0;
        float t = 0;

        if (source)
            source.Play();
        
        if (mode == WriteMode.Char)
        {
            while (i < text.Length)
            {   
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
                
                if (t > 0 && clip)
                    AudioExtension.Play(clip);

                while (t > 0 && i < text.Length)
                {
                    t -= dtRange.RandomInRange();
                    tmpText.text += text[i];
                    i++;
                }
            }
        }

        else {
            string[] lines = text.Split('\n');
            
            while (i < lines.Length)
            {
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
                
                if (t > 0 && clip)
                    AudioExtension.Play(clip);

                while (t > 0 && i < lines.Length)
                {
                    t -= dtRange.RandomInRange();
                    if (i > 0) tmpText.text += '\n';
                    tmpText.text += lines[i];
                    i++;
                }
            }
        }

        if (source)
            source.Stop();
    }
}
