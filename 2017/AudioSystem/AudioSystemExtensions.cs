namespace AudioSystem
{
    public static class AudioSystemExtensions
    {
        public static AudioEvent AddEventListener(this AudioChannel _ch, AudioEventCallback _callback)
        {
            return AudioEvent.Create(_ch, _callback);
        }

        public static AudioEvent RemoveEventListener(this AudioEvent _ae)
        {
            return AudioEvent.Destroy(_ae);
        }
    }
}