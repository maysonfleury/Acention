using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    AudioSource[] songs;
    public Sound[] sounds;

    bool gameStart;

    void Start()
    {
        Play("area_0");

        songs = gameObject.GetComponents<AudioSource>();
        songs[0].volume = 1f;
    }

    public void GameStart()
    {
        if (!gameStart)
        {
            gameStart = true;
            Play("area_1");
            Play("area_2");
            Play("area_3");
            Play("area_4");            

            songs = gameObject.GetComponents<AudioSource>();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.group;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    public void ChangeSong(int index)
    {
        if (songs[index].volume > 0.5f)
        {
            return;
        }
        else
        {
            foreach (AudioSource song in songs)
            {
                if (song.volume > 0)
                    StartCoroutine(FadeVolume(song, false));
            }
            StartCoroutine(FadeVolume(songs[index], true));
        }     
    }

    public void GameOver()
    {
        foreach (AudioSource song in songs)
        {
            song.Stop();
        }
        songs[5].volume = 1f;
        Play("siren");
    }

    IEnumerator FadeVolume(AudioSource song, bool increasing)
    {
        if (increasing)
        {
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 2) 
            {
                song.volume = Mathf.Lerp(0.0f, 1.0f, t);
                yield return null;
            }
        }
        else
        {
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 2)
            {
                song.volume = Mathf.Lerp(1.0f, 0.0f, t);
                yield return null;
            }
        }
        yield return null;
    }

    public void Play(string name)
    {

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Stop();
    }
}
