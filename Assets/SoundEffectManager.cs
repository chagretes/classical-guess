using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance { get; private set; }

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip wellDone;
    [SerializeField]
    private AudioClip thatsRight;
    [SerializeField]
    private AudioClip excellent;
    [SerializeField]
    private List<AudioClip> congratulations;


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        congratulations.Add(wellDone);
        congratulations.Add(thatsRight);
        congratulations.Add(excellent);
    }

    public float PlayAudio(AudioClip audioClip, float Vol = 1)
    {
        audioSource.clip = audioClip;
        audioSource.volume = Vol;
        audioSource.Play();
        return audioSource.clip.length;
    }

    public float PlayAudioNamed(string audioName)
    {
        try
        {
            AudioClip audioClip = Resources.Load<AudioClip>("Audios/"+audioName);
            return PlayAudio(audioClip);
        }
        catch
        {
            Debug.Log($"Não existe arquivo com o nome {audioName}");
            return 0;
        }
    }

    public float PlayPositiveFeedback()
    {
        int value = UnityEngine.Random.Range(0, congratulations.Count);
        return PlayAudio(congratulations[value]);
    }
}
