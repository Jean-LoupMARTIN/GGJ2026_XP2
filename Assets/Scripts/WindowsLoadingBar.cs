using System.Collections;
using UnityEngine;

public class WindowsLoadingBar : Bar
{
    [SerializeField] Vector2 stepProgressRange = new Vector2(0.1f, 0.2f);
    [SerializeField] Vector2 stepDtRange = new Vector2(0.1f, 0.2f);
    [SerializeField] AudioClip endSound;
    [SerializeField] float waitStart = 0.5f;
    [SerializeField] float waitEnd = 1;
    [SerializeField] GameObject desableOnEnd;


    void Start()
    {
        StartCoroutine(FakeLoad());
    }

    IEnumerator FakeLoad()
    {
        SetProgress(0);

        yield return new WaitForSeconds(waitStart); 

        while (Progress < 1)
        {
            yield return new WaitForSeconds(RandomExtension.Range(stepDtRange));
            SetProgress(Progress + RandomExtension.Range(stepProgressRange));
        }

        AudioExtension.Play(endSound);

        yield return new WaitForSeconds(waitEnd); 

        desableOnEnd?.SetActive(false);
    }
}
