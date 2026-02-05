
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Loop")]
    [SerializeField] AudioSource musicAS;
    [SerializeField] AudioSource crowdInAS;
    [SerializeField] AudioSource crowdOutAS;
    [SerializeField] Vector2 musicVolume = new Vector2(1f, 0.8f);
    [SerializeField] Vector2 crowdVolume = new Vector2(0.2f, 1f);
    [SerializeField] float fadeTime = 1;
    [SerializeField] ShakeAdv shake;
    [SerializeField] Vector2 shakeRange = new Vector2(3, 6);
    [SerializeField] float fadeInOutTime = 3;


    [Header("Events")]
    [SerializeField] float waitToStartEvents = 10;
    [SerializeField] Vector2 dtEvents = new Vector2(5, -10);
    [SerializeField] AudioClip[] eventsIn;
    [SerializeField] AudioClip[] eventsOut;

    [Header("Bombs")]
    [SerializeField] BombSound[] bombsIn;
    //[SerializeField] BombSound[] bombsOut;
    [SerializeField] Vector2 dtBombRangeStart = new Vector2(0, 7);
    [SerializeField] Vector2 dtBombRangeEnd = new Vector2(0, 3);
    [SerializeField] Vector2 bombCoefRange = new Vector2(0.7f, 1f);
    [SerializeField] Vector2 bombPitchRange = new Vector2(0.8f, 1.2f);
    [SerializeField] float bombShakeCoef = 50;
    [SerializeField] float bombShakeNoiseCoef = 2;

    [Header("Init Windows Screen")]
    [SerializeField] GameObject initWindowsScreen;
    [SerializeField] AudioClip windowsLaunch;
    [SerializeField, Range(0, 1)] float windowsLaunchVolume = 1;


    [Header("Click")]
    [SerializeField] AudioClip clickPressed;
    [SerializeField] AudioClip clickReleased;
    [SerializeField, Range(0, 1)] float clickVolume = 1f;
    [SerializeField] float clickReleasedDtMin = 0.1f;


    VigilenceDirect vigilenceDirect;
    bool isOut = false;


    protected override void Awake()
    {
        base.Awake();
        vigilenceDirect = FindAnyObjectByType<VigilenceDirect>(FindObjectsInactive.Include);
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame &&
            !initWindowsScreen.activeSelf)
        {
            AudioExtension.Play(clickPressed, clickVolume);
            StartCoroutine(PlayClickReleased(clickReleasedDtMin));
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Application.Quit();
    }

    IEnumerator PlayClickReleased(float dtMin)
    {
        float startTime = Time.time;

        yield return new WaitUntil(() => !Mouse.current.leftButton.isPressed);

        float dt = Time.time - startTime;

        if (dt < dtMin)
            yield return new WaitForSeconds(dtMin - dt);
 
        AudioExtension.Play(clickReleased, clickVolume);
    }

    public void StartPlay()
    {        
        musicAS.volume = musicVolume.x;
        crowdInAS.volume = crowdVolume.x;
                
        shake.CoefTarget = shakeRange.x;
        shake.NoiseTarget = shakeRange.x;

        musicAS.Play();
        crowdInAS.Play();
        StartCoroutine(EventsLoop());
        StartCoroutine(BombsLoop());
    }

    public void PlayWindowsLaunch() => AudioExtension.Play(windowsLaunch, windowsLaunchVolume);

    public void MatchCrtLevel()
    {
        float t = Mathf.InverseLerp(0, vigilenceDirect.levels.Length-1, vigilenceDirect.crtLevelIdx);
        
        StartCoroutine(Fade(musicAS, musicVolume.Lerp(t), fadeTime));
        StartCoroutine(Fade(crowdInAS, crowdVolume.Lerp(t), fadeTime));

        float shakeValue = shakeRange.Lerp(t);
        shake.CoefTarget = shakeValue;
        shake.NoiseTarget = shakeValue;
    }

    IEnumerator Fade(AudioSource audioSource, float endVolume, float time)
    {
        float t = 0;
        float startVolume = audioSource.volume;

        while (t < time)
        {
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
            float progress = t / time;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, progress);
        }

        audioSource.volume = endVolume;
    }

    IEnumerator EventsLoop()
    {
        AudioClip lastEvent = null;

        yield return new WaitForSeconds(waitToStartEvents);

        while (true)
        {
            AudioClip ev = (isOut ? eventsOut : eventsIn).Where((c) => c != lastEvent).PickRandom();
            AudioExtension.Play(ev, Mathf.Max(crowdInAS.volume, crowdOutAS.volume));
            float progress = Mathf.InverseLerp(0, vigilenceDirect.levels.Length-1, vigilenceDirect.crtLevelIdx);
            yield return new WaitForSeconds(ev.length + dtEvents.Lerp(progress));
            lastEvent = ev;
        }
    }
    
    IEnumerator BombsLoop()
    {
        BombSound lastBomb = null;
        int lvlStart = int.MaxValue;

        foreach (BombSound bomb in bombsIn)
            lvlStart = Mathf.Min(lvlStart, bomb.lvlMin);
    
        yield return new WaitForSeconds(waitToStartEvents);
        yield return new WaitUntil(() => vigilenceDirect.crtLevelIdx >= lvlStart);

        while (true)
        {
            float progress = Mathf.InverseLerp(lvlStart, vigilenceDirect.levels.Length-1, vigilenceDirect.crtLevelIdx);
            float dt = Vector2.Lerp(dtBombRangeStart, dtBombRangeEnd, progress).RandomInRange();
            yield return new WaitForSeconds(dt);

            //BombSound bomb = (isOut ? bombsOut : bombsIn).Where((b) => b != lastBomb && vigilenceDirect.crtLevelIdx >= b.lvlMin).PickRandom();
            BombSound bomb = bombsIn.Where((b) => b != lastBomb && vigilenceDirect.crtLevelIdx >= b.lvlMin).PickRandom();
            float coef = bombCoefRange.RandomInRange();
            float volume = Mathf.Max(crowdInAS.volume, crowdOutAS.volume) * coef;
            AudioExtension.Play(bomb.clip, volume, bombPitchRange.RandomInRange());

            shake.Coef += volume * bombShakeCoef * bomb.shakeCoef;
            shake.Noise += volume * bombShakeNoiseCoef * bomb.shakeCoef;
            
            lastBomb = bomb;
        }
    }

    public void StartFadeInOut()
    {
        isOut = true;
        StartCoroutine(FadeInOut(fadeInOutTime));
    }

    IEnumerator FadeInOut(float time)
    {
        crowdOutAS.volume = 0;
        crowdOutAS.Play();
        crowdOutAS.time = crowdInAS.time;

        float musicVolumeStart = musicAS.volume;
        float crowdVolumeStart = crowdInAS.volume;
        float t = 0;

        while (t < time)
        {
            yield return new WaitForEndOfFrame();
            t+= Time.deltaTime;
            float progress = Mathf.Min(t / time, 1);
            crowdOutAS.volume = progress * crowdVolumeStart;
            crowdInAS.volume = (1 - progress) * crowdVolumeStart;
            musicAS.volume = (1 - progress) * musicVolumeStart;
        }

        musicAS.Stop();
        crowdInAS.Stop();
    }
    /*
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
    }*/
}

[Serializable]
public class BombSound
{
    public int lvlMin;
    public AudioClip clip;
    public float shakeCoef;
}