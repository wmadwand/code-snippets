using System.Collections;
using System.Collections.Generic;

namespace AudioSystem
{
    public delegate void AudioEventCallback(AudioChannel _ch);

    public class AudioEvent
    {
        #region Properties
        private static List<AudioEvent> EvList = new List<AudioEvent>();

        private AudioChannel ch;
        private AudioEvent ev;
        private AudioEventCallback callback;

        private bool isEventListenerStarted;
        private bool isCallRemoveEventListener;
        #endregion

        public AudioEvent(AudioChannel _ch, AudioEventCallback _callback)
        {
            ch = _ch;
            if (ch == null)
            {
                return;
            }

            callback = _callback;
            ev = this;
            StartEventListener();
        }

        public static AudioEvent Create(AudioChannel _ch, AudioEventCallback _callback)
        {
            AudioEvent ae = new AudioEvent(_ch, _callback);
            return ae;
        }

        private void StartEventListener()
        {
            EvList.Add(ev);
            AudioSystemDemo.Instance.StartCoroutine(AudioEventListener());
        }

        private IEnumerator AudioEventListener()
        {
            isEventListenerStarted = true;

            while (true)
            {
                if (isCallRemoveEventListener)
                {
                    break;
                }

                //@TODO: What about all the other subscribers in case of the sound's interruption (Stop) by any 3rd part?
                // How their AudioEventListeners could be terminated so as to avoid forsaken useless circular coroutines?

                if (ch == null)
                {
                    StopEventListener();
                    break;
                }

                if (ch.channel != null && ch.channel.time >= ch.Data.sound.length && !ch.channel.isPlaying)
                {
                    callback(ch);
                    break;
                }

                yield return null;
            }
        }

        public static AudioEvent Destroy(AudioEvent _ae)
        {
            _ae.StopEventListener();
            return null;
        }

        private void StopEventListener()
        {
            if (isEventListenerStarted)
            {
                isCallRemoveEventListener = true;
                isEventListenerStarted = false;

                EvList.Remove(ev);
                ev = null;
            }
        }
    }
}