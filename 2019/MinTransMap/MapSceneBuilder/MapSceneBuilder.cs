namespace MinTrans.MapSceneBuilderModule
{
	public abstract class MapSceneBuilder
	{
		public Map Map { get; private set; }

		public void BuildMap()
		{
			Map = new Map("");
		}

		public virtual void BuildBackground() { }
		public virtual void BuildModel() { }
		public virtual void BuildPath() { }
	}

	public class PCMapSceneBuilder : MapSceneBuilder
	{
		public const string BACKGROUND_PATH = "MapParts/Background";
		public const string MODEL_PATH = "MapParts/RussiaModelWiki";
		public string[] PATHES_PATH = { "MapParts/MapPathes/OGMap01", "MapParts/MapPathes/Corridors", "MapParts/MapPathes/Railroads02", "MapParts/MapPathes/Roads02" };

		public override void BuildBackground()
		{
			Map.Background = new Background(BACKGROUND_PATH);
		}

		public override void BuildModel()
		{
			Map.Model = new Model(MODEL_PATH);
		}

		public override void BuildPath()
		{
			foreach (string path in PATHES_PATH)
			{
				Map.MapPaths.Add(new Path(path));
			}
		}
	}

	public class MapSceneDirector
	{
		public Map CreateMap(MapSceneBuilder builder)
		{
			builder.BuildMap();
			builder.BuildBackground();
			builder.BuildModel();
			builder.BuildPath();

			return builder.Map;
		}
	}
}