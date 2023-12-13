using System;
using TNRD.Autohook;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField, AutoHook(searchArea: AutoHookSearchArea.Children, ReadOnlyWhenFound = true)]
    private AudioSource source;

    public SFX[] sfxList;

    [Range(0, 1)]
    [SerializeField] private float volume;

    private void Update()
    {
        source.volume = volume;
    }

    public void PlaySound(string soundName)
    {
        bool foundSound = false;

        foreach (SFX sfx in sfxList)
        {
            if (sfx.name == soundName) {
                source.clip = sfx.clip;

                foundSound = true;

                source.Play();
            }
        }

        if (!foundSound)
        {
            Debug.LogError($"Invalid SFX name \"{soundName}\"");
        }
    }
}

[Serializable]
public struct SFX
{
    public AudioClip clip;
    public string name;
}

