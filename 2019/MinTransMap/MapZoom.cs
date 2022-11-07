using System;
using System.Collections.Generic;

namespace MinTrans
{
	public enum MapZoomStateEnum
	{
		ZoomIn00,
		ZoomIn01,
		ZoomIn02
	}

	public class MapZoom
	{
		public static event Action<MapZoomStateEnum> OnStateChanged;

		public IMapZoomState state;

		public MapZoomStateEnum stateEnum;

		public MapZoom(IMapZoomState state, MapZoomStateEnum stateEnum)
		{
			this.state = state;
			this.stateEnum = stateEnum;
		}

		public void ZoomIn()
		{
			state.ZoomIn(this);
			OnStateChanged?.Invoke(stateEnum);
		}

		public void ZoomOut()
		{
			state.ZoomOut(this);
			OnStateChanged?.Invoke(stateEnum);
		}
	}

	public interface IMapZoomState
	{
		void ZoomIn(MapZoom map);
		void ZoomOut(MapZoom map);
	}

	public class ZoomIn00MapZoomState : IMapZoomState
	{
		public void ZoomIn(MapZoom map)
		{
			map.state = MapZoomStateFactory.GetState(MapZoomStateEnum.ZoomIn01);
			map.stateEnum = MapZoomStateEnum.ZoomIn01;
		}

		public void ZoomOut(MapZoom map) { }
	}

	public class ZoomIn01MapZoomState : IMapZoomState
	{
		public void ZoomIn(MapZoom map)
		{
			map.state = MapZoomStateFactory.GetState(MapZoomStateEnum.ZoomIn02);
			map.stateEnum = MapZoomStateEnum.ZoomIn02;
		}

		public void ZoomOut(MapZoom map)
		{
			map.state = MapZoomStateFactory.GetState(MapZoomStateEnum.ZoomIn00);
			map.stateEnum = MapZoomStateEnum.ZoomIn00;
		}
	}

	public class ZoomIn02MapZoomState : IMapZoomState
	{
		public void ZoomIn(MapZoom map) { }

		public void ZoomOut(MapZoom map)
		{
			map.state = MapZoomStateFactory.GetState(MapZoomStateEnum.ZoomIn01);
			map.stateEnum = MapZoomStateEnum.ZoomIn01;
		}
	}

	public static class MapZoomStateFactory
	{
		private static Dictionary<MapZoomStateEnum, IMapZoomState> _states = new Dictionary<MapZoomStateEnum, IMapZoomState>();

		public static IMapZoomState GetState(MapZoomStateEnum name)
		{
			if (_states.ContainsKey(name))
			{
				return _states[name];
			}

			switch (name)
			{
				case MapZoomStateEnum.ZoomIn00: _states[name] = new ZoomIn00MapZoomState(); break;
				case MapZoomStateEnum.ZoomIn01: _states[name] = new ZoomIn01MapZoomState(); break;
				case MapZoomStateEnum.ZoomIn02: _states[name] = new ZoomIn02MapZoomState(); break;
			}

			return _states[name];
		}
	}
}