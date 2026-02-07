using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            print("There is a duplicate of SFXManger");
        }
    }


    public AudioSource SoundFXObject;
    public AudioSource SoundFXObjectLooping;
    public AudioMixerGroup audioMixerGroup;

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransorm, float volume) {
        //Spawn audio in GameObject
        AudioSource audioSource = Instantiate(SoundFXObject, spawnTransorm.position, Quaternion.identity);

        //Assign the audioClip
        audioSource.clip = audioClip;

        audioSource.outputAudioMixerGroup = audioMixerGroup;
        //Assign Volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get Length of Sound FX CLip
        float clipLength = audioSource.clip.length;

        //Destroy the clip after its done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransorm, float volume) {
        //Assign random index
        int rand = Random.Range(0, audioClip.Length-1);

        //Spawn audio in GameObject
        AudioSource audioSource = Instantiate(SoundFXObject, spawnTransorm.position, Quaternion.identity);

        //Assign the audioClip
        audioSource.clip = audioClip[rand];

        audioSource.outputAudioMixerGroup = audioMixerGroup;
        //Assign Volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get Length of Sound FX CLip
        float clipLength = audioSource.clip.length;

        //Destroy the clip after its done playing
        Destroy(audioSource.gameObject,clipLength);
    }

    public void PlayLoopingSFX(AudioClip audioClip, Transform spawnTransorm, float volume) {
        
        //Spawn audio in GameObject
        AudioSource audioSource = Instantiate(SoundFXObjectLooping, spawnTransorm.position, Quaternion.identity);

        //Assign the audioClip
        audioSource.clip = audioClip;

        audioSource.outputAudioMixerGroup = audioMixerGroup;
        //Assign Volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get Length of Sound FX CLip
        float clipLength = audioSource.clip.length;

        //Destroy the clip after its done playing
        Destroy(audioSource.gameObject, clipLength);
    }





}
