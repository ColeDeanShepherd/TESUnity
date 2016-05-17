using UnityEngine;

namespace TESUnity
{
	public class DoorComponent : MonoBehaviour
	{
		public string doorName;
		public bool leadsToAnotherCell;
		public string doorExitName;
		public Vector3 doorExitPos;
		public Quaternion doorExitOrientation;
	}
}