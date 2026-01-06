using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    public AudioClip loadingMusicClip;
    public AudioClip mainMusicClip;

    public AudioClip heartSoundClip;
    public AudioClip shovelSoundClip;
    public AudioClip wateringCanSoundClip;
    public AudioClip scissorsSoundClip;
    public AudioClip soapingSoundClip;
    public AudioClip placementSoundClip;

    private void Start()
    {
        //PlayMusic(musicClip);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }
}
