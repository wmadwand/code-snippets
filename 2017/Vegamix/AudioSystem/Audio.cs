using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace AudioSystem
{
    public class Audio : IAudioEventMember
    {
        #region Properties
        public static AudioMusic MUSIC = new AudioMusic();
        public static AudioMusic AMBIENT = new AudioMusic();

        private static List<AudioChannel> soundChannels = new List<AudioChannel>();
        private static List<string> idsDelay = new List<string>();

        private static float soundVolume = 1.0f;
        public static float SoundVolume
        {
            get
            {
                return soundVolume;
            }
            set
            {
                soundVolume = value;

                foreach (AudioChannel ch in soundChannels)
                {
                    ch.Volume = value * ch.VolumeWant;
                }

                AMBIENT.Volume = value;
            }
        }

        //@TODO: deal with its publicity...
        public AudioEvent AudioEventCurrent { get; set; }
        private static Dictionary<string, AudioEvent> audioEventDict = new Dictionary<string, AudioEvent>();
        #endregion

        #region Sound controllers
        public static AudioChannel PlaySound(string id, int repeatCount = 1)
        {
            if (soundVolume <= 0 || String.IsNullOrEmpty(id))
            {
                return null;
            }

            if (HasInDelay(id))
            {
                return null;
            }

            //if (Vars.isLocal && id == "birds")
            //{
            //    return null;
            //}

            AudioData data = AudioData.Find(id);
            if (data == null)
            {
                return null;
            }

            AudioChannel ch = AudioChannel.Create();
            ch.Init(data, (sound) =>
            {
                AudioEvent ae = ch.AddEventListener(OnSoundChannelComplete);

                audioEventDict[id] = ae;

                ch.Play(ch.Data.sound, repeatCount, soundVolume * ch.VolumeWant);
                soundChannels.Add(ch);
                AddToDelay(data.id, data.delayBetween);
            });

            return ch;
        }

        public static void StopSound(string id)
        {
            AudioChannel ch;

            for (int i = soundChannels.Count - 1; i >= 0; i--)
            {
                ch = soundChannels[i];

                if (ch.Data.id == id)
                {
                    if (audioEventDict.ContainsKey(id))
                    {
                        audioEventDict[id] = audioEventDict[id].RemoveEventListener();
                        audioEventDict.Remove(id);
                    }

                    ch.Destroy();
                    soundChannels.RemoveRange(i, 1);

                    break;
                }
            }
        }
        #endregion

        #region Service methods
        private static void OnSoundChannelComplete(AudioChannel _ch)
        {
            if (audioEventDict.ContainsKey(_ch.Data.id) && audioEventDict[_ch.Data.id] != null)
            {
                audioEventDict[_ch.Data.id] = audioEventDict[_ch.Data.id].RemoveEventListener();
                audioEventDict.Remove(_ch.Data.id);
            }

            _ch.Destroy(); //@TODO: figure out the details  
            soundChannels.RemoveAll(item => item == _ch);

            Debug.LogError("Audio - OnSoundChannelComplete OK");
        }

        public static bool IsSoundPlaying(string id)
        {
            return soundChannels.Find(ch => (ch.Data.id == id && ch.IsPlaying)) != null;
        }

        public static bool HasInDelay(string id)
        {
            return idsDelay.IndexOf(id) >= 0;
        }

        private static void AddToDelay(string id, int delay)
        {
            if (HasInDelay(id) || delay <= 0) return;
            idsDelay.Add(id);
            AudioSystemDemo.Instance.StartCoroutine(DoAfter(delay, RemoveFromDelay, id));
        }

        private static void RemoveFromDelay(string id)
        {
            idsDelay.RemoveAll(item => item == id);
        }

        static IEnumerator DoAfter(int delay, Action<string> callback, string id)
        {
            yield return new WaitForSeconds(delay * 0.01f);
            callback(id);
        }
        #endregion
    }
}