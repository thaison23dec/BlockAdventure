using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioClip[] soundFxList;
    public AudioClip[] voiceList;

    public bool soundOpen = true;


    private AudioSource audioSource;
        

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        audioSource = GetComponent<AudioSource>();
        soundOpen = true;
    }

    public void VolumeToggle()
    {
        if (soundOpen)
        {
            soundOpen = false;
            audioSource.mute = true;
        } else
        {
            soundOpen = true;
            audioSource.mute = false;
        }
    }


    public void PlayPlaceSoundFx()
    {
        audioSource.PlayOneShot(soundFxList[0]);
    }

    public void PlayLineCompletedSoundFx()
    {
        audioSource.PlayOneShot(soundFxList[1]); ;
    }

    public void PlayVoiceLine(int index)
    {
        if (index < voiceList.Length)
        {
           audioSource.PlayOneShot(voiceList[index]);
        }
        else if (index >= voiceList.Length)
        {
            audioSource.PlayOneShot(voiceList[voiceList.Length - 1]);
        }
        else
        {
            return;
        }
    }
}
