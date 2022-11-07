using System;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    public class AudioChannel : IAudioEventMember
    {
        #region Properties
        private static List<AudioChannel> cache = new List<AudioChannel>();

        private AudioClip sound;
        public AudioSource channel { get; private set; }

        private float pausePosition = 0;
        private int repeatCount;
        private int repeats;

        private AudioData data;
        public AudioData Data
        {
            get { return data; }
        }

        private float volumeWant = 0;
        public float VolumeWant
        {
            get { return volumeWant; }
        }

        public bool IsPlaying
        {
            get { return sound; }
        }

        public bool IsPlayingReal
        {
            get { return (bool)channel; }
        }

        private float volume;
        public float Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                if (channel)
                {
                    channel.volume = volume;
                }
            }
        }

        //@TODO: deal with its publicity...
        public AudioEvent AudioEventCurrent { get; set; }
        #endregion

        #region Channel base methods
        public AudioChannel() { }

        public static AudioChannel Create()
        {
            AudioChannel _channel = null;

            if (cache.Count > 0)
            {
                _channel = cache[cache.Count - 1];
                cache.RemoveAt(cache.Count - 1);
            }
            else
            {
                _channel = new AudioChannel();
            }

            return _channel;
        }

        private void InitBase(AudioData _data)
        {
            data = _data;
            volumeWant = _data.volume + UnityEngine.Random.Range(0, 1) * _data.volumeRandomize;
            channel = AudioSystemDemo.Instance.gameObject.AddComponent<AudioSource>();
        }

        public void Init(AudioData _data)
        {
            InitBase(_data);
            data.DownloadSound();
        }

        public void Init(AudioData _data, Action<AudioClip> callback)
        {
            InitBase(_data);
            data.DownloadSound((sound) => callback(sound));
        }

        public void Destroy()
        {
            Stop();

            if (cache.Count < 30 && cache.IndexOf(this) < 0)
            {
                cache.Add(this);
            }
        }
        #endregion

        #region Sound controllers
        public void Play(AudioClip _sound, int _repeatCount, float _volume, float _startTime = 0)
        {
            Stop();

            sound = _sound;
            repeatCount = _repeatCount;
            repeats = 0;
            volume = _volume;
            pausePosition = _startTime;

            PlayChannel(pausePosition);
        }

        public void Stop()
        {
            if (channel)
            {
                StopChannel();
            }
            sound = null;
        }

        public void Pause()
        {
            if (channel)
            {
                pausePosition = channel.time;
                StopChannel();
            }
            else
            {
                pausePosition = 0;
            }
        }

        public void Resume()
        {
            if (sound && !channel)
            {
                PlayChannel(pausePosition);
            }
        }
        #endregion

        #region Channel controllers
        private void PlayChannel(float startTime)
        {
            try
            {
                if (!channel)
                {
                    channel = AudioSystemDemo.Instance.gameObject.AddComponent<AudioSource>();
                }

                channel.clip = sound;
                channel.volume = volume;
                channel.time = startTime;
                channel.loop = false;

                AudioEventCurrent = this.AddEventListener(OnChannelComplete);
                channel.Play();
            }
            catch
            {
                channel = null;
            }
        }

        private void OnChannelComplete(AudioChannel a)
        {
            repeats++;

            if (repeatCount < 0 || repeats < repeatCount)
            {
                StopChannel();
                PlayChannel(0);
            }
            else
            {
                Stop();

                Debug.LogError("AudioChannel - OnChannelComplete OK");
            }
        }

        private void StopChannel()
        {
            if (AudioEventCurrent != null)
            {
                AudioEventCurrent = AudioEventCurrent.RemoveEventListener();
            }

            channel.Stop();
            Component.Destroy(channel);
            channel = null;

            Debug.LogError("StopChannel OK");
        }
        #endregion
    }
}