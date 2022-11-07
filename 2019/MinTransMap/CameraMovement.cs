using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MinTrans
{
	public class CameraMovement : MonoBehaviour
	{
		public static event Action<bool> OnMove;
		public bool IsMoving { get; private set; }

		//---------------------------------------------------------		

		private Dictionary<string, Vector3[]> _cameraPositions;
		private Transform _startTransform;
		private Transform _targetTransform;

		//---------------------------------------------------------

		private void Awake()
		{
			MapZoom.OnStateChanged += MapZoom_OnStateChanged;

			_cameraPositions = new Dictionary<string, Vector3[]>();
			_targetTransform = new GameObject().transform;
		}

		private void Start()
		{
			LoadMapPoints();

			if (_cameraPositions != null)
			{
				transform.position = _cameraPositions[AppManager.RUSSIA_REGION][0];
				transform.rotation = Quaternion.Euler(_cameraPositions[AppManager.RUSSIA_REGION][1]);
			}

			StartCoroutine(CheckForTargetTransform());
		}

		private void OnDestroy()
		{
			MapZoom.OnStateChanged -= MapZoom_OnStateChanged;
		}

		private void MapZoom_OnStateChanged(MapZoomStateEnum obj)
		{
			LoadMapPoints();

			ChangePosition(AppManager.Instance.CurrentRegion);
		}

		private void ChangePosition(string pos)
		{
			if (IsMoving)
			{
				return;
			}

			_startTransform = transform;

			_targetTransform.position = _cameraPositions[pos][0];
			_targetTransform.rotation = Quaternion.Euler(_cameraPositions[pos][1]);

			transform.DOMove(_targetTransform.position, 1).SetEase(Ease.InOutCubic);
			transform.DORotate(_targetTransform.rotation.eulerAngles, 1).SetEase(Ease.InOutCubic);

			StartCoroutine(CheckForTargetTransform());
		}

		private IEnumerator CheckForTargetTransform()
		{
			if (IsMoving)
			{
				yield break;
			}

			IsMoving = true;
			OnMove?.Invoke(IsMoving);

			while (transform.position != _targetTransform.position)
			{
				yield return null;
			}

			IsMoving = false;
			OnMove?.Invoke(IsMoving);
		}

		private void LoadMapPoints()
		{
			if (_cameraPositions != null && _cameraPositions.Count > 0)
			{
				return;
			}

			District[] districts = AppManager.Instance.TextMananager.GetDistricts().Values.ToArray();

			for (int i = 0; i < districts.Length; i++)
			{
				Vector3[] posRotDistrict = new Vector3[2];

				posRotDistrict[0] = TextManager.GetVector3FromString(districts[i].position);
				posRotDistrict[1] = TextManager.GetVector3FromString(districts[i].rotation);

				_cameraPositions.Add(districts[i].key, posRotDistrict);

				for (int j = 0; j < districts[i].regions.Length; j++)
				{
					Vector3[] posRotRegion = new Vector3[2];

					if (districts[i].regions[j].key == "Krasnodar_Krai")
					{
						Vector3 shiftPos = new Vector3(-7.861f, -0.79f, -0.99f);

						posRotRegion[0] = shiftPos;
						posRotRegion[1] = TextManager.GetVector3FromString(districts[i].regions[j].rotation);
					}
					else if (districts[i].regions[j].key == "Primorsky_Krai")
					{
						Vector3 shiftPos = new Vector3(5.36f, -4.15f, -1.1f);

						posRotRegion[0] = shiftPos;
						posRotRegion[1] = TextManager.GetVector3FromString(districts[i].regions[j].rotation);
					}
					else
					{
						posRotRegion[0] = TextManager.GetVector3FromString(districts[i].regions[j].position);
						posRotRegion[1] = TextManager.GetVector3FromString(districts[i].regions[j].rotation);
					}

					_cameraPositions.Add(districts[i].regions[j].key, posRotRegion);
				}
			}
		}
	}
}