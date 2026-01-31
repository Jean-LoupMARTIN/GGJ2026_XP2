using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AudioExtension
{
    public static void Play(AudioClip clip)
    {
        if (!clip)
            return;
            
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
    }
}


