using System.Collections.Generic;

namespace ZombieMessage
{
	public enum ZombieMessageType
	{
		WaveNumberBegan,
		WaveFinished,
		NextWaveCountdown,
		TeamPlayerDead,
		BaseUnderAttack,
		Generic
	}

	public interface IZombieMessage
	{
		void Initialize(object[] settings);
		string GetText();
	}

	public class ZombieMessageFactory
	{
		Dictionary<ZombieMessageType, IZombieMessage> _messages = new Dictionary<ZombieMessageType, IZombieMessage>();

		public virtual IZombieMessage Create(object settings)
		{
			object[] currentSettings = (object[])settings;
			ZombieMessageType type = (ZombieMessageType)currentSettings[0];

			if (_messages.ContainsKey(type))
			{
				_messages[type].Initialize(currentSettings);
				return _messages[type];
			}

			switch (type)
			{
				case ZombieMessageType.WaveNumberBegan: _messages[type] = new WaveNumberBegan(); break;
				case ZombieMessageType.WaveFinished: _messages[type] = new WaveFinished(); break;
				case ZombieMessageType.NextWaveCountdown: _messages[type] = new NextWaveCountdown(); break;
				case ZombieMessageType.TeamPlayerDead: _messages[type] = new TeamPlayerDead(); break;
				case ZombieMessageType.BaseUnderAttack: _messages[type] = new BaseUnderAttack(); break;
				case ZombieMessageType.Generic: _messages[type] = new Generic(); break;
				default: break;
			}

			_messages[type].Initialize(currentSettings);

			return _messages[type];
		}
	}
}