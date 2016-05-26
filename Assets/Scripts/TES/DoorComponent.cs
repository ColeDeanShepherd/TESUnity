using UnityEngine;

namespace TESUnity
{
	public class DoorComponent : MonoBehaviour
	{
		public bool leadsToInteriorCell;
		public string doorName;
		public bool leadsToAnotherCell;
		public string doorExitName;
		public Vector3 doorExitPos;
		public Quaternion doorExitOrientation;
		public bool isOpen;
	}
}