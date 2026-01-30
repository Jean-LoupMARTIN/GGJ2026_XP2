using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void Load(string sceneName)
    {
        FrontCurtain.Instance.Close(() => SceneManager.LoadScene(sceneName));
    }
}
