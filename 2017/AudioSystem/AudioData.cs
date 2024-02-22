using System;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    public class AudioData
    {
        #region Properties
        public string id;
        public string path;
        public AudioClip sound = null;
        public float volume;
        public float volumeRandomize;
        public int delayBetween;

        private static Dictionary<string, AudioData> DICT = new Dictionary<string, AudioData>();
        #endregion

        public AudioData() { }

        public static void ReadJson(string jsonStr)
        {
            //@TODO: deal with it, since it's much faster -> //AudioData[] audioDataArray = JSONExtensions.GetJsonArray<AudioData>(jsonStr);

            JSONObject jsonObj = new JSONObject(jsonStr);

            foreach (var item in jsonObj.list)
            {
                AudioData audioData = new AudioData();

                audioData.id = item[0].ToStrFromJson();
                audioData.path = item[1].ToStrFromJson();
                audioData.volume = item[2].ToFloatFromJson();
                audioData.volumeRandomize = item[3].ToFloatFromJson();
                audioData.delayBetween = item[4].ToIntFromJson();

                DICT[audioData.id] = audioData;

                Debug.LogFormat("ID = {0}, path = {1}", audioData.id, audioData.path);
            }

            //foreach (AudioData _audioData in audioDataArray)
            //{
            //    AudioData audioData = new AudioData();

            //    audioData = _audioData;
            //    DICT[_audioData.id] = audioData;

            //    Debug.LogFormat("ID = {0}, path = {1}", audioData.id, audioData.path);
            //}
        }

        public static AudioData Find(string id)
        {
            return DICT.ContainsKey(id) ? DICT[id] : null;
        }

        public void DownloadSound()
        {
            if (sound)
            {
                return;
            }

            WWWDownloader.DownloadAsync("http://localhost/vegamix/" + path, (www) =>
              {
                  sound = WWWFileProcessor.GetAudioClip(www);
              });
        }

        public void DownloadSound(Action<AudioClip> callback)
        {
            if (sound)
            {
                callback(sound);
                return;
            }

            WWWDownloader.DownloadAsync("http://localhost/vegamix/" + path, (www) =>
            {
                sound = WWWFileProcessor.GetAudioClip(www);
                callback(sound);
            });
        }
    }
}