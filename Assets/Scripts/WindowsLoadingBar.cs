using System.Collections;
using UnityEngine;

public class WindowsLoadingBar : Bar
{
    [SerializeField] float timeToLoad = 2f;
    [SerializeField] float timeToEnd = 2f;
    [SerializeField] Vector2 dtStepRange = new Vector2(0.1f, 0.2f);

    [SerializeField] GameObject desableOnEnd;
    [SerializeField] GameObject activeOnEnd;
    [SerializeField] float delayActive = 0.5f;

    void Start()
    {
        StartCoroutine(FakeLoad());
    }

    IEnumerator FakeLoad()
    {
        SetProgress(0);

        yield return new WaitForEndOfFrame();

        SoundManager.Instance.StartPlay();
        float startTime = Time.time;

        while (Progress < 1)
        {
            float loadTime = Time.time - startTime;
            float restTime = timeToLoad - loadTime;
            yield return new WaitForSeconds(Mathf.Min(dtStepRange.RandomInRange(), restTime));
            loadTime = Time.time - startTime;
            SetProgress(loadTime / timeToLoad);
        }

        AudioExtension.Play(SoundManager.Instance.bootWindowsSound);

        yield return new WaitForSeconds(timeToEnd); 

        desableOnEnd?.SetActive(false);
        Invoke("ActiveOnEnd", delayActive);
    }

    void ActiveOnEnd() => activeOnEnd.SetActive(true);
}
