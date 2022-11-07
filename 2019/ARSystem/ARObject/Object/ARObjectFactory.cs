namespace ARSystem
{
	public static class ARObjectFactory
	{
		public static ARObject Construct(GameEntityType type)
		{
			ARObject _ARObject;

			switch (type)
			{
				case GameEntityType.Player: { _ARObject = new ARPlayer(); break; }

				case GameEntityType.ArenaAR: { _ARObject = new ARArena(); break; }
				case GameEntityType.Arena3D: { _ARObject = new ARArena3D(); break; }

				case GameEntityType.Monster: { _ARObject = new ARMonster(); break; }
				case GameEntityType.Chest: { _ARObject = new ARChest(); break; }
				case GameEntityType.Mimic: { _ARObject = new ARMimic(); break; }
				default: { _ARObject = new ARGenericObject(); break; }
			}

			return _ARObject;
		}
	}
}