using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

#region types declerations
public enum SoundEffectType { Lazer, Click, MeteorExplosion, BossWarning, Rocket }

[System.Serializable]
public class SoundEffectEntry
{
    public SoundEffectType type;
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
            Destroy(this);
    }
    #endregion

    [SerializeField] private List<SoundEffectEntry> SoundEffects = new List<SoundEffectEntry>();

    public AudioSource main_music;
    public AudioSource dead_music;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setMusicVolume();
    }

    public void PlaySound(SoundEffectType type)
    {
        AudioSource audio = SoundEffects.Find(x => x.type == type).audio;
        if (audio == null) return;

        audio.PlayOneShot(audio.clip, GetVolume());
    }

    public float GetVolume()
    {
        return Settings.Instance.activeSound ? (Settings.Instance.sound_general_value / 100) * (Settings.Instance.sound_effect_value / 100) : 0;
    }

    public IEnumerator TransitionMusic(AudioSource from, AudioSource to)
    {
        float time = 0;
        float fromVolume = from.volume;
        float toVolume = to.volume;

        to.volume = 0f;
        to.Play();

        while (time < 1.5f)
        {

            float t = time / 1.5f;
            from.volume = Mathf.Lerp(fromVolume, 0, t);
            to.volume = Mathf.Lerp(0, toVolume, t);
            time += Time.deltaTime;
            yield return null;
        }
        from.Stop();
        from.volume = fromVolume;
        to.volume = toVolume;
    }

    public void lauchTransitionMusic(AudioSource from, AudioSource to)
    {
        StartCoroutine(TransitionMusic(from, to));
    }

    public void setMusicVolume()
    {
        float vol = (Settings.Instance.sound_general_value / 100) * (Settings.Instance.sound_music_value / 100);
        if (!Settings.Instance.activeSound) vol = 0;

        main_music.volume = vol;
        dead_music.volume = vol;
    }
}


