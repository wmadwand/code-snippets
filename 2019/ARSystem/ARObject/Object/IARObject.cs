using UnityEngine;

namespace ARSystem
{
	public interface IARObject
	{
		bool IsInitiated { get; }
		bool IsActive { get; }

		void Init(GameObject parent, Vector3 parentPosition);
		void SetActive(bool flag);
	}
}