using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance { get; private set; }

    [SerializeField]
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
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
}
