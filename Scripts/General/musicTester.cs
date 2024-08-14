using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicTester : MonoBehaviour
{
    [SerializeField] private AudioClip song;
    [SerializeField] private double seconds = 116.57d;
    [SerializeField] private float volume = 0.15f;
    private AudioSource previousMusic;
    private AudioSource currentMusic;

    private void Awake()
    {
        spawnMusic();
    }

    private void Update()
    {
        Debug.Log(currentMusic.time);
        if(currentMusic.time >= seconds)
        {
            previousMusic = currentMusic;
            spawnMusic();
        }

        if (previousMusic != null && !previousMusic.isPlaying)
        {
            Destroy(previousMusic.gameObject);
            previousMusic = null;
        }
    }

    private void spawnMusic()
    {
        
        AudioSource newAudiosource = new GameObject().AddComponent<AudioSource>();
        newAudiosource.clip = song;
        newAudiosource.volume = volume;
        newAudiosource.Play();
        currentMusic = newAudiosource;
    }
}
