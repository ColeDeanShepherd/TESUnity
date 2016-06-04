using System.Collections;
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

		private Quaternion closedRotation;
		private Quaternion openRotation;
		private bool moving = false;

		void Start ()
		{
			closedRotation = transform.rotation;
			openRotation = closedRotation * Quaternion.Euler( Vector3.up * 90f );
			moving = false;
		}

		public void UseDoor ()
		{
			if ( isOpen ) Close(); else Open();
		}

		private void Open ()
		{
			if ( !moving ) StartCoroutine( c_Open() );
		}

		private void Close ()
		{
			if ( !moving ) StartCoroutine( c_Close() );
		}

		private IEnumerator c_Open ()
		{
			moving = true;
			while ( Quaternion.Angle( transform.rotation , openRotation ) > 1f )
			{
				transform.rotation = Quaternion.Slerp( transform.rotation , openRotation , Time.deltaTime * 5f );
				yield return new WaitForEndOfFrame();
			}
			isOpen = true;
			moving = false;
		}

		private IEnumerator c_Close ()
		{
			moving = true;
			while ( Quaternion.Angle( transform.rotation , closedRotation ) > 1f )
			{
				transform.rotation = Quaternion.Slerp( transform.rotation , closedRotation , Time.deltaTime * 5f );
				yield return new WaitForEndOfFrame();
			}
			isOpen = false;
			moving = false;
		}
	}
}