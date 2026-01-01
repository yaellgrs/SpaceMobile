using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class Song : MonoBehaviour
{
    public static Song Instance;

    public AudioSource main_music;
    public AudioSource dead_music;
    public GameObject lazer_sound;
    public GameObject meteor_sound;
    public GameObject rocket_sound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setMusicVolume();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playSound(GameObject src)
    {
        GameObject prefab = Instantiate(src);
        AudioSource audio = prefab.GetComponent<AudioSource>();
        audio.volume = (Settings.Instance.sound_general_value / 100 ) * (Settings.Instance.sound_effect_value / 100);
        if (audio)
        {
            audio.Play();
        }

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
        main_music.volume = vol;
        dead_music.volume = vol;
    }
}
