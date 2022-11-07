
namespace ZombieMessage
{
	public class WaveNumberBegan : IZombieMessage
	{
		private int _number;

		public void Initialize(object[] settings)
		{
			_number = (int)settings[1];
		}

		public string GetText()
		{
			return $"Wave # {_number}";
		}
	}
}