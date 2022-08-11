using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SoundAudioClip<soundT>
{
    public string name;
    public soundT sound;
    public AudioClip audioClip;
    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;
    public bool loop = false;

}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;
    private static GameObject soundTrackGameObject;
    private static AudioSource soundTrackAudioSource;
    [Range(0f, 1f)]
    [SerializeField] private float _masterVolumeMultiplier;
    [Range(0f, 1f)]
    [SerializeField] private float _soundTrackVolume;
    public float MVM
    {
        get
        {
            return _masterVolumeMultiplier;
        }
        set
        {
            _masterVolumeMultiplier = value;
            OnMasterVolumeChange();

        }
    }
    public float STV
    {
        get
        {
            return _soundTrackVolume;
        }
        set
        {
            _soundTrackVolume = value;
            OnSoundTrackVolumeChange();

        }
    }
    public enum Sound
    {
        BombSizzle,
        BombExplode,
        Pop,
        LaserBeam,
        Shield,
        ButtonClick,
    }
    public enum SoundTrack
    {
        ST01,
        ST02,
        ST03,
        ST03_1,
        None = -1,

    }
    //can't use struct and dictionary type is a bit limited
    public class soundTimer
    {
        public Sound soundType;
        public float lastTimePlayed;
        public float timeDelay;
        public soundTimer(Sound sound, float LastTimePlayed, float TimeDelay)
        {
            this.soundType = sound;
            this.lastTimePlayed = LastTimePlayed;
            this.timeDelay = TimeDelay;
        }
    }

    /*
    List of sound with delay add more into it if there are more sound need time delay 
    Current array size: 1 <- increase this number if there are more 
    */
    public static soundTimer[] soundTimerArray;

    public SoundAudioClip<AudioManager.Sound>[] soundAudiosClipArray;
    public SoundAudioClip<AudioManager.SoundTrack>[] soundTrackArray;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        // PlaySoundTrack(SoundTrack.NormalLevelST);
        switch (SceneManager.GetActiveScene().name)
        {
            default:
                break;
        }
        soundTimerArray = new soundTimer[0];
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            PlaySoundTrack(SoundTrack.ST02);
        }
        else Destroy(soundTrackGameObject);
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {

        // AudioManager.instance.PlaySoundTrack(AudioManager.SoundTrack.GameOverST); test 

        //play main theme or music here
        //Play("Theme");
    }

    //Spacial Sound 
    public void PlaySound(Sound sound, Vector3 position)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Spacial Sound ");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            SoundAudioClip<Sound> s = System.Array.Find(soundAudiosClipArray, Sound => Sound.sound == sound);
            soundGameObject.name += s.name;
            audioSource.loop = s.loop;
            audioSource.volume = s.volume * MVM;
            audioSource.pitch = s.pitch;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.maxDistance = 100f;
            audioSource.dopplerLevel = 0f;
            audioSource.clip = GetAudioClip(sound);
            audioSource.Play();
            if (!audioSource.loop)
                Destroy(soundGameObject, audioSource.clip.length);
        }
    }
    public GameObject PlaySound(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("One Shot Sound ");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            //GameObject soundGameObject = new GameObject("Sound");
            //AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            SoundAudioClip<Sound> s = System.Array.Find(soundAudiosClipArray, Sound => Sound.sound == sound);
            oneShotGameObject.name += s.name;
            oneShotAudioSource.loop = s.loop;
            oneShotAudioSource.volume = s.volume * MVM;
            oneShotAudioSource.pitch = s.pitch;
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
            return oneShotGameObject;
        }
        return null;

    }
    public void PlaySoundTrack(SoundTrack soundTrack)
    {
        if (soundTrackGameObject == null)
        {
            soundTrackGameObject = new GameObject();
            soundTrackAudioSource = soundTrackGameObject.AddComponent<AudioSource>();
            DontDestroyOnLoad(soundTrackGameObject);
        }
        SoundAudioClip<SoundTrack> s = System.Array.Find(soundTrackArray, SoundTrack => SoundTrack.sound == soundTrack);
        soundTrackAudioSource.name = s.name;
        soundTrackAudioSource.loop = s.loop;
        soundTrackAudioSource.volume = STV * MVM;
        soundTrackAudioSource.pitch = s.pitch;
        // soundTrackAudioSource.PlayOneShot(GetAudioClip(soundTrack));
        soundTrackAudioSource.clip = GetAudioClip(soundTrack);
        soundTrackAudioSource.Play();
    }

    //need to implement this later
    public void removeSound(Sound sound)
    {

    }

    public static AudioClip GetAudioClip(Sound sound)
    {

        foreach (SoundAudioClip<Sound> soundAudioClip in instance.soundAudiosClipArray)
        {
            if (soundAudioClip.sound == sound)
                return soundAudioClip.audioClip;
        }
        Debug.LogErrorFormat("Sound" + sound + "not found!");
        return null;
    }
    public static AudioClip GetAudioClip(SoundTrack sound)
    {
        foreach (SoundAudioClip<SoundTrack> soundTrack in instance.soundTrackArray)
        {
            if (soundTrack.sound == sound)
                return soundTrack.audioClip;
        }
        Debug.LogErrorFormat("Sound Track: " + sound + "not found!");
        return null;
    }


    private static bool CanPlaySound(Sound sound)
    {
        // for sound like foot step and such
        soundTimer soundT = System.Array.Find(soundTimerArray, soundtimer => soundtimer.soundType == sound);
        if (soundT != null)
        {
            if (soundT.lastTimePlayed + soundT.timeDelay < Time.time)
            {
                soundT.lastTimePlayed = Time.time;
                return true;
            }
            else return false;
        }

        return true;
    }

    public GameObject GetSoundTrackGameObject()
    {
        return soundTrackGameObject;
    }

    public IEnumerator FadeOutST(float fadeDuration = 0f, float targetVolumne = 0, SoundTrack NextST = SoundTrack.None)
    {
        float currentTime = 0;
        float start = soundTrackAudioSource.volume;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            soundTrackAudioSource.volume = Mathf.Lerp(start, targetVolumne, currentTime / fadeDuration);
            yield return null;
        }
        PlaySoundTrack(NextST);
        yield break;
    }

    public virtual void OnMasterVolumeChange()
    {
        if (soundTrackGameObject != null)
        {
            soundTrackAudioSource.volume = STV * MVM; // redundant 
        }
    }
    public virtual void OnSoundTrackVolumeChange()
    {
        if (soundTrackGameObject != null)
        {
            soundTrackAudioSource.volume = STV * MVM; // redundant
        }
    }
}
