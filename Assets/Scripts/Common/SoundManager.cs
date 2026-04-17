using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [SerializeField] Sound[] bgmSoundDates;
    [SerializeField] Sound[] sfxSoundDates;

    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBgm("TitleBgm");
    }

    public void  PlayBgm(string name)
    {
        Sound s = Array.Find(bgmSoundDates, x => x.name == name);

        if (s == null)
        {
            Debug.LogError("音楽ファイルが見つかりませんでした。");
        } else
        {
            bgmAudioSource.clip = s.clip;
            bgmAudioSource.Play();
        }
    }

    public void StopBgm(string name)
    {
        bgmAudioSource.Stop();
    }

    public void PlaySfx(string name)
    {
        Sound s = Array.Find(sfxSoundDates, x => x.name == name);

        if (s == null)
        {
            Debug.LogError("音楽ファイルが見つかりませんでした。");
        } else
        {
            sfxAudioSource.PlayOneShot(s.clip);
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
