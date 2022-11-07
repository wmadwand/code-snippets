
namespace ZombieMessage
{
	public class NextWaveCountdown : IZombieMessage
	{
		private float _time;

		public void Initialize(object[] settings)
		{
			_time = (float)settings[1];
		}

		public string GetText()
		{
			return $"Next mission in: {_time}";
		}
	}
}