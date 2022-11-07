
namespace ZombieMessage
{
	public class BaseUnderAttack : IZombieMessage
	{
		private bool _flag;

		public void Initialize(object[] settings)
		{
			_flag = (bool)settings[1];
		}

		public string GetText()
		{
			return $"Player is dead";
		}
	}

	public class Generic : IZombieMessage
	{
		private string _text;

		public void Initialize(object[] settings)
		{
			_text = (string)settings[1];
		}

		public string GetText()
		{
			return _text;
		}
	}
}