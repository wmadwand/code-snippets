using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARSystem;

[Serializable]
public struct MapObject
{
	public GameEntity gameEntity;
	public ARObject ARObject;
	public Vector3 position;
}