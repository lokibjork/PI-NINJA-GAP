using UnityEngine;

[CreateAssetMenu(fileName = "Audio/Sound Manager", menuName = "Sound Manager")]
public class SoundManagerSO : ScriptableObject
{
    private static SoundManagerSO instance;

    private static SoundManagerSO Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<SoundManagerSO>("Sound Manager");
            }

            return instance;
        }
    }
    
    public AudioSource SoundObject;
    
    private static float _volumeChangeMultiplier;
    private static float _pitchChangeMultiplier;

    public static void PlaySoundFXClip(AudioClip clip, Vector3 soundPos, float volume)
    {
        float randVolume = Random.Range(volume - _volumeChangeMultiplier, volume + _volumeChangeMultiplier);
        float randPitch = Random.Range(volume - _pitchChangeMultiplier, volume + _pitchChangeMultiplier);

        AudioSource a = Instantiate(Instance.SoundObject, soundPos, Quaternion.identity);
        
        a.clip = clip;
        a.volume = randVolume;
        a.pitch = randPitch;
        a.Play();
    }
    
    public static void PlaySoundFXClips(AudioClip[] clips, Vector3 soundPos, float volume)
    {
        int randClips = Random.Range(0, clips.Length);
        float randVolume = Random.Range(volume - _volumeChangeMultiplier, volume + _volumeChangeMultiplier);
        float randPitch = Random.Range(volume - _pitchChangeMultiplier, volume + _pitchChangeMultiplier);

        AudioSource a = Instantiate(Instance.SoundObject, soundPos, Quaternion.identity);
        
        a.clip = clips[randClips];
        a.volume = randVolume;
        a.pitch = randPitch;
        a.Play();
    }
}
