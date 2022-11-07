﻿using UnityEngine;

public class MoveWithCompass : MonoBehaviour
{
	private double _lastCompassUpdateTime = 0;
	private Quaternion _correction = Quaternion.identity;
	private Quaternion _targetCorrection = Quaternion.identity;
	private Quaternion _compassOrientation = Quaternion.identity;

	void Start()
	{
		Input.gyro.enabled = true;
		Input.compass.enabled = true;
	}

	void Update()
	{
		// The gyro is very effective for high frequency movements, but drifts its
		// orientation over longer periods, so we want to use the compass to correct it.
		// The iPad's compass has low time resolution, however, so we let the gyro be
		// mostly in charge here.

		// First we take the gyro's orientation and make a change of basis so it better
		// represents the orientation we'd like it to have
		Quaternion gyroOrientation = Quaternion.Euler(90, 0, 0) * Input.gyro.attitude * Quaternion.Euler(0, 0, 90);

		// See if the compass has new data
		if (Input.compass.timestamp > _lastCompassUpdateTime)
		{
			_lastCompassUpdateTime = Input.compass.timestamp;

			// Work out an orientation based primarily on the compass
			Vector3 gravity = Input.gyro.gravity.normalized;
			Vector3 flatNorth = Input.compass.rawVector -
				Vector3.Dot(gravity, Input.compass.rawVector) * gravity;
			_compassOrientation = Quaternion.Euler(180, 0, 0) * Quaternion.Inverse(Quaternion.LookRotation(flatNorth, -gravity)) * Quaternion.Euler(0, 0, 90);

			// Calculate the target correction factor
			_targetCorrection = _compassOrientation * Quaternion.Inverse(gyroOrientation);
		}

		// Jump straight to the target correction if it's a long way; otherwise, slerp towards it very slowly
		if (Quaternion.Angle(_correction, _targetCorrection) > 45)
			_correction = _targetCorrection;
		else
			_correction = Quaternion.Slerp(_correction, _targetCorrection, 0.02f);

		// Easy bit :)
		transform.rotation = _correction * gyroOrientation;
	}
}