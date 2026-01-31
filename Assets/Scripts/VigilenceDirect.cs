using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class VigilenceDirect : Singleton<VigilenceDirect>
{
    [SerializeField] Level[] levels;

    VDLoading loading;
    
    [HideInInspector] public Level crtLevel = null;
    [HideInInspector] public int crtLevelIdx = -1;


    protected override void Awake()
    {
        base.Awake();
        loading = GetComponentInChildren<VDLoading>();
    }

    void OnEnable()
    {
        if (crtLevelIdx == -1) SetLevel(0);
        else                   SetLevel(crtLevel);
    }

    public void SetLevel(Level level)
    {
        crtLevel = level;
        crtLevelIdx = levels.IndexOf(level);

        loading.StartLoading();
        VDSuspectsSelection.Instance.SelectIdx(0);
        
        // // wanted notice
        // wantedImage.sprite = crtLevel.wantedNotice.sprite;
        // wantedDescription.StartWriting(crtLevel.wantedNotice.description);

        // // suspects
        // for (int i = 0; i < selectSuspectButtons.Length; i++)
        //     selectSuspectButtons[i].icon.sprite = level.suspects[i].icon;

        // SelectSuspect(level.suspects[0]);
    }
    public void SetLevel(int levelIdx) => SetLevel(levels[levelIdx]);


    // [SerializeField] WriteText loadingWriteText;
    // [SerializeField] Image wantedImage;
    // [SerializeField] WriteText wantedDescription;
    // [SerializeField] SelectSuspectButton[] selectSuspectButtons;
    // [SerializeField] Image suspectPicture;
    
    // int selectedSuspectIdx = -1;
    // Suspect selectedSuspect = null;

    // protected override void Awake()
    // {
    //     base.Awake();

    //     for (int i = 0; i < selectSuspectButtons.Length; i++)
    //         selectSuspectButtons[i].idx = i;

    // }

    // void OnEnable()
    // {
    //     SetLevel(levels[0]);
    // }

    
    // public void NextLevel()
    // {
    //     crtLevelIdx++;
    //     crtLevelIdx = Mathf.Min(crtLevelIdx, levels.Length-1);
    //     SetLevel(crtLevelIdx);
    // }

    // public void SelectSuspect(Suspect suspect)
    // {
    //     selectedSuspect = suspect;
    //     selectedSuspectIdx = crtLevel.suspects.IndexOf(suspect);

    //     for (int i = 0; i < selectSuspectButtons.Length; i++)
    //         selectSuspectButtons[i].SetSelected(i == selectedSuspectIdx);

    //     suspectPicture.sprite = suspect.picture;
    // }
    // public void SelectSuspect(int suspectIdx) => SelectSuspect(crtLevel.suspects[suspectIdx]);

    // public void SwipSuspect(bool left)
    // {
    //     selectedSuspectIdx = left ? selectedSuspectIdx - 1 + crtLevel.suspects.Length : 
    //                                 selectedSuspectIdx + 1;

    //     selectedSuspectIdx %= crtLevel.suspects.Length;

    //     SelectSuspect(selectedSuspectIdx);
    // }

    // public void ConfirmSelectedSuspect()
    // {
    //     if (selectedSuspectIdx == crtLevel.answer)
    //     {
    //         print("Yes !!!");
    //         NextLevel();
    //     }

    //     else {
    //         print("No...");
    //     }
    // }
}

[Serializable]
public class Level
{
    public Suspect[] suspects;
    public wantedNotice wantedNotice;
    public int answer = 0;
}


[Serializable]
public class Suspect
{
    public Sprite icon;
    public Sprite picture;
    //[TextArea] public string description = "";
}

[Serializable]
public class wantedNotice
{
    public Sprite sprite;
    [TextArea] public string description;
}