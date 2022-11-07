
namespace ZombieMessage
{
	public class TeamPlayerDead : IZombieMessage
	{
		private string _name;

		public void Initialize(object[] settings)
		{
			_name = (string)settings[1];
		}

		public string GetText()
		{
			return $"Enemy {_name} is destroyed!";
		}
	}
}