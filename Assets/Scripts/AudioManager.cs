using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] AudioClip[] audioClips;

    void Start ()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void DashSound ()
    {
        audioSource.clip = audioClips[0];
        audioSource.Play();
    }
}
