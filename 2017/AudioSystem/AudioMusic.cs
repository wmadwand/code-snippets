using System.Collections.Generic;

namespace AudioSystem
{
    public class AudioMusic
    {
        private Dictionary<string, AudioChannel> channels = new Dictionary<string, AudioChannel>(); // id --> AudioChannel

        private string currentId;
        private bool stopped;

        private float volume = 1.0f;
        public float Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                UpdateChannel(false);
            }
        }

        public AudioMusic() { }

        public void Play(string id, bool fromStart)
        {
            if (currentId == id)
            {
                if (stopped)
                {
                    stopped = false;
                    UpdateChannel(fromStart);
                }

                return;
            }

            stopped = false;
            AudioChannel ch;

            if (!string.IsNullOrEmpty(currentId) && channels.ContainsKey(currentId))
            {
                ch = channels[currentId];
                ch.Pause();
            }

            currentId = id;

            if (channels.ContainsKey(id))
            {
                ch = channels[id];
            }
            else
            {
                ch = AudioChannel.Create();
                ch.Init(AudioData.Find(id));
                channels[id] = ch;
            }

            UpdateChannel(fromStart);
        }

        public void Stop()
        {
            stopped = true;
            UpdateChannel(false);
        }

        private void UpdateChannel(bool fromStart)
        {
            if (!string.IsNullOrEmpty(currentId) && channels.ContainsKey(currentId))
            {
                AudioChannel ch = channels[currentId];
                float vol = volume * ch.Data.volume;

                if (vol == 0 || stopped)
                {
                    ch.Pause();
                }
                else if (ch.IsPlaying && !fromStart)
                {
                    ch.Resume();
                }
                else
                {
                    ch.Play(ch.Data.sound, -1, vol, 0);
                }

                ch.Volume = vol;
            }
        }
    }
}