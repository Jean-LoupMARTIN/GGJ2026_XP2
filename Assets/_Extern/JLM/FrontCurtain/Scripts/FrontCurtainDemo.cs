using System.Collections;
using UnityEngine;

public class FrontCurtainDemo : MonoBehaviour
{
    [SerializeField, Min(0.1f)] float openCloseDelay = 2;

    void Start()
    {
        StartCoroutine(OpenCloseCurtain());
    }

    IEnumerator OpenCloseCurtain()
    {
        while (true)
        {
            yield return new WaitForSeconds(openCloseDelay);

            if (!FrontCurtain.Instance)
                continue;

            if (FrontCurtain.Instance.CrtState == FrontCurtain.State.Opened)
                FrontCurtain.Instance.Close(() => print("Closed"));

            else if (FrontCurtain.Instance.CrtState == FrontCurtain.State.Closed)
                FrontCurtain.Instance.Open(() => print("Opened"));
        }
    }
}
