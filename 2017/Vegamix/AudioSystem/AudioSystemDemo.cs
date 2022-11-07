using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;
using UnityEngine.UI;

public class AudioSystemDemo : AutoSingletonManager<AudioSystemDemo>
{
    [Header("Controller Properties")]
    public Slider MusicSlider;
    public Slider SoundSlider;

    private bool gotAllSounds;

    public string UrlPath
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return "http://localhost/vegamix/bin/" + "json/resources/audio_pc.json";
            }
            else if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return "http://localhost/vegamix/bin/" + "json/resources/audio_pc.json" /*"json/resources/audio_webgl.json"*/;
            }

            return "";
        }
    }

    private void GetAudioJson()
    {
        WWWDownloader.DownloadAsync(UrlPath, (www) =>
        {
            string jsonStr = WWWFileProcessor.GetString(www);
            AudioData.ReadJson(jsonStr);

            AudioData.Find("match3Theme").DownloadSound((sound) => { });
            AudioData.Find("mapTheme").DownloadSound((sound) => { });
            //AudioData.Find("amb_map").DownloadSound((sound) => { });
            AudioData.Find("click").DownloadSound((sound) => { });
            AudioData.Find("birds").DownloadSound((sound) => { gotAllSounds = true; });
        });
    }

    IEnumerator Start()
    {
        MusicSlider.value = Audio.MUSIC.Volume;
        SoundSlider.value = Audio.AMBIENT.Volume;

        GetAudioJson();

        yield return new WaitUntil(() => gotAllSounds);
    }

    private void LateUpdate()
    {
        Audio.MUSIC.Volume = MusicSlider.value;
        Audio.SoundVolume = SoundSlider.value;
    }

    public void PlayMusic(string _music)
    {
        ButtonClick();

        if (!gotAllSounds)
        {
            return;
        }

        _music = "mapTheme";
        Audio.MUSIC.Play(_music, false);
    }

    public void PlayAmbient()
    {
        ButtonClick();

        if (!gotAllSounds)
        {
            return;
        }

        if (!Audio.IsSoundPlaying("amb_map"))
        {
            Audio.PlaySound("amb_map");
        }
        else
        {
            Audio.StopSound("amb_map");
        }

        //Audio.AMBIENT.Play("amb_map", false);
    }

    public void ButtonClick()
    {
        Audio.PlaySound("click");
    }

    public void PlayStopSoundById(string soundId)
    {
        ButtonClick();

        if (!Audio.IsSoundPlaying(soundId))
        {
            Audio.PlaySound(soundId);
        }
        else
        {
            Audio.StopSound(soundId);
        }
    }
}