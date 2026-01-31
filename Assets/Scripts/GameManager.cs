using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] SelectSuspectButton[] selectSuspectButtons;
    [SerializeField] Image suspectPicture;
    [SerializeField] Image wantedImage;
    [SerializeField] WriteText wantedDescription;
    [SerializeField] Level[] levels;
    int crtLevelIdx = -1;
    Level crtLevel = null;
    int selectedSuspectIdx = -1;
    Suspect selectedSuspect = null;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < selectSuspectButtons.Length; i++)
            selectSuspectButtons[i].idx = i;

        SetLevel(levels[0]);
    }

    public void SetLevel(Level level)
    {
        if (crtLevel == level)
            return;

        crtLevel = level;
        crtLevelIdx = levels.IndexOf(level);

        // wanted notice
        wantedImage.sprite = crtLevel.wantedNotice.sprite;
        wantedDescription.StartWriting(crtLevel.wantedNotice.description);

        // suspects
        for (int i = 0; i < selectSuspectButtons.Length; i++)
            selectSuspectButtons[i].icon.sprite = level.suspects[i].icon;

        SelectSuspect(level.suspects[0]);
    }
    public void SetLevel(int levelIdx) => SetLevel(levels[levelIdx]);

    public void NextLevel()
    {
        crtLevelIdx++;
        crtLevelIdx = Mathf.Min(crtLevelIdx, levels.Length-1);
        SetLevel(crtLevelIdx);
    }

    public void SelectSuspect(Suspect suspect)
    {
        selectedSuspect = suspect;
        selectedSuspectIdx = crtLevel.suspects.IndexOf(suspect);

        for (int i = 0; i < selectSuspectButtons.Length; i++)
            selectSuspectButtons[i].SetSelected(i == selectedSuspectIdx);

        suspectPicture.sprite = suspect.picture;
    }
    public void SelectSuspect(int suspectIdx) => SelectSuspect(crtLevel.suspects[suspectIdx]);

    public void SwipSuspect(bool left)
    {
        selectedSuspectIdx = left ? selectedSuspectIdx - 1 + crtLevel.suspects.Length : 
                                    selectedSuspectIdx + 1;

        selectedSuspectIdx %= crtLevel.suspects.Length;

        SelectSuspect(selectedSuspectIdx);
    }

    public void ConfirmSelectedSuspect()
    {
        if (selectedSuspectIdx == crtLevel.answer)
        {
            print("Yes !!!");
            NextLevel();
        }

        else {
            print("No...");
        }
    }
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