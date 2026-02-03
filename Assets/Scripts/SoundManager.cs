using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] Vector2 volumeCrowdRange = new Vector2(0.2f, 1f);
    [SerializeField] Vector2 volumeMusicRange = new Vector2(1f, 1f);
    [SerializeField] Vector2 shakeRange = new Vector2(3, 10);
    [SerializeField] Vector2 dtSoundRangeStart = new Vector2(3, 10);
    [SerializeField] Vector2 dtSoundRangeEnd = new Vector2(1, 5);
    [SerializeField] Vector2 dtBombRangeStart = new Vector2(3, 7);
    [SerializeField] Vector2 dtBombRangeEnd = new Vector2(1, 5);
    [SerializeField] Vector2 bombVolumeRange = new Vector2(0.5f, 1f);
    [SerializeField] Vector2 bombPitchRange = new Vector2(0.9f, 1.1f);
    [SerializeField] float shakeCoefCoef = 50;
    [SerializeField] float shakeNoiseCoef = 2;
    
    [SerializeField] AudioClip[] randomSounds;
    [SerializeField] BombSound[] randomBombs;
    [SerializeField] ShakeAdv shake;

    public AudioClip bootWindowsSound;
    [SerializeField] GameObject frontScreen;
    [SerializeField] AudioClip clickSound, releaseSound;
    [SerializeField, Range(0, 1)] float volumeClick = 1f;
    [SerializeField] float dtMinReleased = 0.1f;

    [SerializeField] AudioSource audioSourceCrowd;
    [SerializeField] AudioSource audioSourceCrowd2;
    [SerializeField] AudioSource audioSourceMusic;

    [SerializeField] float timeFadeCrowd = 3;
    [SerializeField] AudioClip[] randomSoundsOut;
    [SerializeField] BombSound[] randomBombsOut;
    bool isOut = false;


    VigilenceDirect vigilenceDirect;



    protected override void Awake()
    {
        base.Awake();
        vigilenceDirect = FindAnyObjectByType<VigilenceDirect>(FindObjectsInactive.Include);
    }

    public void StartPlay()
    {
        audioSourceCrowd.Play();
        audioSourceMusic.Play();
        MatchCrtLevel();
        StartCoroutine(PlayRandomSoundsLoop());
        StartCoroutine(PlayRandomBombsLoop());
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame &&
            !frontScreen.activeSelf)
        {
            Play(clickSound, volumeClick);
            StartCoroutine(PlayReleasedSound(dtMinReleased));
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Application.Quit();
    }


    void Play(AudioClip clip, float volume = 1, float pitch = 1) => AudioExtension.Play(clip, volume, pitch);

    public void MatchCrtLevel()
    {
        float t = Mathf.InverseLerp(0, vigilenceDirect.levels.Length-1, vigilenceDirect.crtLevelIdx);
        
        audioSourceCrowd.volume = volumeCrowdRange.Lerp(t);
        audioSourceMusic.volume = volumeMusicRange.Lerp(t);

        float shakeValue = shakeRange.Lerp(t);
        shake.CoefTarget = shakeValue;
        shake.NoiseTarget = shakeValue;
    }

    IEnumerator PlayReleasedSound(float dtMin)
    {
        float startTime = Time.time;

        yield return new WaitUntil(() => !Mouse.current.leftButton.isPressed);

        float dt = Time.time - startTime;

        if (dt < dtMin)
            yield return new WaitForSeconds(dtMin - dt);
 
        Play(releaseSound, volumeClick);
    }

    IEnumerator PlayRandomSoundsLoop()
    {
        AudioClip lastPlayed = null;
        
        while (true)
        {
            float lvlProgress = Mathf.InverseLerp(0, vigilenceDirect.levels.Length-1, vigilenceDirect.crtLevelIdx);
            float dt = Vector2.Lerp(dtSoundRangeStart, dtSoundRangeEnd, lvlProgress).RandomInRange();
            yield return new WaitForSeconds(dt);

            AudioClip clip = (isOut ? randomSoundsOut : randomSounds).Where((c) => c != lastPlayed).PickRandom();
            Play(clip, Mathf.Max(audioSourceCrowd.volume, audioSourceCrowd2.volume));
            lastPlayed = clip;
        }
    }

    IEnumerator PlayRandomBombsLoop()
    {
        BombSound lastBomb = null;
        int lvlStart = int.MaxValue;

        foreach (BombSound bomb in randomBombs)
            lvlStart = Mathf.Min(lvlStart, bomb.lvlMin);
            
        yield return new WaitUntil(() => vigilenceDirect.crtLevelIdx >= lvlStart);

        while (true)
        {
            float lvlProgress = Mathf.InverseLerp(lvlStart, vigilenceDirect.levels.Length-1, vigilenceDirect.crtLevelIdx);
            float dt = Vector2.Lerp(dtBombRangeStart, dtBombRangeEnd, lvlProgress).RandomInRange();
            yield return new WaitForSeconds(dt);

            BombSound bomb = (isOut ? randomBombsOut : randomBombs).Where((b) => b != lastBomb && vigilenceDirect.crtLevelIdx >= b.lvlMin).PickRandom();
            float volume = Mathf.Max(audioSourceCrowd.volume, audioSourceCrowd2.volume) * bombVolumeRange.RandomInRange();
            Play(bomb.clip, volume, bombPitchRange.RandomInRange());
            float shakeValue = bomb.shakeCoef * volume;
            shake.Coef += shakeValue * shakeCoefCoef;
            shake.Noise += shakeValue * shakeNoiseCoef;
            lastBomb = bomb;
        }
    }

    public void SetOut()
    {
        isOut = true;
        StartCoroutine(FadeCrowd(timeFadeCrowd));
    }

    IEnumerator FadeCrowd(float time)
    {
        audioSourceCrowd2.volume = 0;
        audioSourceCrowd2.Play();
        audioSourceCrowd2.time = audioSourceCrowd.time;

        float volumeCrowdStart = audioSourceCrowd.volume;
        float volumeMusicStart = audioSourceMusic.volume;
        float t = 0;

        while (t < time)
        {
            yield return new WaitForEndOfFrame();
            t+= Time.deltaTime;
            float progress = Mathf.Min(t / time, 1);
            audioSourceCrowd2.volume = progress * volumeCrowdStart;
            audioSourceCrowd.volume = (1 - progress) * volumeCrowdStart;
            audioSourceMusic.volume = (1 - progress) * volumeMusicStart;
        }

        audioSourceCrowd.Stop();
        audioSourceMusic.Stop();
    }
}

[Serializable]
public class BombSound
{
    public int lvlMin;
    public AudioClip clip;
    public float shakeCoef;
}