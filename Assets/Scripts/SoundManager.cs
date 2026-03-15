using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

#region types declerations
public enum SoundEffectType { Lazer, Click, MeteorExplosion, BossWarning, Rocket, MeteorSpawn, Forge }
public enum MusicType { Main, Dead, Boss};

[System.Serializable]
public class SoundEffectEntry
{
    public SoundEffectType type;
    public AudioSource audio;
}

[System.Serializable]
public class MusicEntry
{
    public MusicType type;
    public AudioSource audio;
}
#endregion


public class SoundManager : MonoBehaviour
{

    #region INSTANCE
    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [SerializeField] private List<SoundEffectEntry> SoundEffects = new List<SoundEffectEntry>();
    [SerializeField] private List<MusicEntry> Musics = new List<MusicEntry>();

    public MusicEntry currentSound;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitSound();
        setMusicVolume();

    }

    private void InitSound()
    {
        MusicEntry targetMusic = Musics.Find(x => x.type == MusicType.Main);
        targetMusic.audio.volume = GetVolume();
        targetMusic.audio.Play();
        currentSound = targetMusic;
    }

    public void PlaySound(SoundEffectType type)
    {
        AudioSource audio = SoundEffects.Find(x => x.type == type).audio;
        if (audio == null) return;

        audio.PlayOneShot(audio.clip, GetVolume());
    }

    public IEnumerator PlaySoundWithTime(SoundEffectType type, float time)
    {
        AudioSource audio = SoundEffects.Find(x => x.type == type).audio;
        if (audio == null) yield break;

        audio.PlayOneShot(audio.clip, GetVolume());

        yield return new WaitForSeconds(time);
        audio.Stop();
    }

    public float GetVolume()
    {
        return Settings.Instance.activeSound ? (Settings.Instance.sound_general_value / 100) * (Settings.Instance.sound_effect_value / 100) : 0;
    }

    public IEnumerator TransitionMusic(MusicType type)
    {
        float time = 0;
        MusicEntry targetMusic = Musics.Find(x => x.type == type);
        if (targetMusic == null) yield break;


        targetMusic.audio.volume = 0f;
        targetMusic.audio.Play();

        while (time < 1.5f)
        {

            float t = time / 1.5f;
            currentSound.audio.volume = Mathf.Lerp(GetVolume(), 0, t);
            targetMusic.audio.volume = Mathf.Lerp(0, GetVolume(), t);
            time += Time.deltaTime;
            yield return null;
        }
        currentSound.audio.Stop();
        currentSound.audio.volume = GetVolume();
        targetMusic.audio.volume = GetVolume();

        currentSound = targetMusic;
    }

    public void lauchTransitionMusic(MusicType type)
    {
        StartCoroutine(TransitionMusic(type));
    }

    public void setMusicVolume()
    {
        float vol = (Settings.Instance.sound_general_value / 100) * (Settings.Instance.sound_music_value / 100);
        if (!Settings.Instance.activeSound) vol = 0;

        currentSound.audio.volume = vol;
    }
}


