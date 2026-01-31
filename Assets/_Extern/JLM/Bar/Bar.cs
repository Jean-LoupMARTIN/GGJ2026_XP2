using UnityEngine;

public class Bar : MonoBehaviour
{
    [SerializeField, Range(0, 1)] float progress = 1;
    [SerializeField] Transform fill;

    public float Progress => progress;

    void OnValidate()
    {
        FillMatchProgress();
    }

    void FillMatchProgress() 
    {
        if (!fill)
            return;

        Vector3 scale = fill.localScale;
        scale.x = progress;
        fill.localScale = scale;        
    }

    public void SetProgress(float progress)
    {
        this.progress = Mathf.Clamp01(progress);
        FillMatchProgress();
    }
}
