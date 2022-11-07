
namespace ZombieMessage
{
	public class WaveFinished : IZombieMessage
	{
		public void Initialize(object[] settings) { }

		public string GetText()
		{
			return $"Wave finished";
		}
	}
}