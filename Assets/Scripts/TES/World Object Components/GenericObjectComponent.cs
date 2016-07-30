using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TESUnity
{
	public class GenericObjectComponent : MonoBehaviour
	{
		public ESM.CELLRecord.RefObjDataGroup refObjDataGroup = null;
		public class DoorData
		{
			public string doorName;
			public string doorExitName;
			public bool leadsToAnotherCell;
			public bool leadsToInteriorCell;
			public Vector3 doorExitPos;
			public Quaternion doorExitOrientation;

			public bool isOpen;
			public Quaternion closedRotation;
			public Quaternion openRotation;
			public bool moving = false;
		}
		public DoorData doorData = null;

		[System.Serializable]
		public class LightData
		{
			public Light lightComponent;
			public enum LightFlags { Dynamic = 0x0001, CanCarry = 0x0002, Negative = 0x0004, Flicker = 0x0008, Fire = 0x0010, OffDefault = 0x0020, FlickerSlow = 0x0040, Pulse = 0x0080, PulseSlow = 0x0100 }
			public int flags;
		}
		public LightData lightData = null;

		public class ObjectData
		{
			public string name;
		}
		public ObjectData objData = new ObjectData();

		public ESM.Record record;

		public void init(ESM.Record record, string tag)
		{
			gameObject.tag = tag;
			foreach(Transform c in transform) c.tag = tag;
			this.record = record;
			if(record is ESM.DOORRecord) SetupDoorData(record);
			if(record is ESM.LIGHRecord) SetupLightData(record);
			if(record is ESM.ACTIRecord) objData.name = (record as ESM.ACTIRecord).FNAM.value;
			if(record is ESM.CONTRecord) objData.name = (record as ESM.CONTRecord).FNAM.value;
			if(record is ESM.LOCKRecord) objData.name = (record as ESM.LOCKRecord).FNAM.value;
			if(record is ESM.PROBRecord) objData.name = (record as ESM.PROBRecord).FNAM.value;
			if(record is ESM.REPARecord) objData.name = (record as ESM.REPARecord).FNAM.value;
			if(record is ESM.WEAPRecord) objData.name = (record as ESM.WEAPRecord).FNAM.value;
			if(record is ESM.CLOTRecord) objData.name = (record as ESM.CLOTRecord).FNAM.value;
			if(record is ESM.ARMORecord) objData.name = (record as ESM.ARMORecord).FNAM.value;
			if(record is ESM.INGRRecord) objData.name = (record as ESM.INGRRecord).FNAM.value;
			if(record is ESM.ALCHRecord) objData.name = (record as ESM.ALCHRecord).FNAM.value;
			if(record is ESM.APPARecord) objData.name = (record as ESM.APPARecord).FNAM.value;
			if(record is ESM.BOOKRecord) objData.name = (record as ESM.BOOKRecord).FNAM.value;
			if(record is ESM.MISCRecord) objData.name = (record as ESM.MISCRecord).FNAM.value;
		}

		public void SetupDoorData(ESM.Record record)
		{
			doorData = new DoorData();
			doorData.closedRotation = transform.rotation;
			doorData.openRotation = doorData.closedRotation * Quaternion.Euler(Vector3.up * 90f);
			doorData.moving = false;

			ESM.DOORRecord DOOR = record as ESM.DOORRecord;
			if(DOOR.FNAM != null) doorData.doorName = DOOR.FNAM.value;

			doorData.leadsToAnotherCell = (refObjDataGroup.DNAM != null) || (refObjDataGroup.DODT != null);
			doorData.leadsToInteriorCell = (refObjDataGroup.DNAM != null);
			if(doorData.leadsToInteriorCell) doorData.doorExitName = refObjDataGroup.DNAM.value;
			if(doorData.leadsToAnotherCell && !doorData.leadsToInteriorCell)
			{
				var doorExitCell = MorrowindEngine.instance.dataReader.FindExteriorCellRecord(MorrowindEngine.instance.GetExteriorCellIndices(doorData.doorExitPos));
				doorData.doorExitName = (doorExitCell != null) ? doorExitCell.RGNN.value : doorData.doorName;
			}

			if(refObjDataGroup.DODT != null)
			{
				doorData.doorExitPos = Convert.NifPointToUnityPoint(refObjDataGroup.DODT.position);
				doorData.doorExitOrientation = Convert.NifEulerAnglesToUnityQuaternion(refObjDataGroup.DODT.eulerAngles);
			}

			objData.name = doorData.leadsToAnotherCell ? doorData.doorExitName : "Use " + doorData.doorName;
		}

		public void SetupLightData(ESM.Record record)
		{
			lightData = new LightData();
			ESM.LIGHRecord LIGH = record as ESM.LIGHRecord;
			lightData.lightComponent = gameObject.GetComponentInChildren<Light>(true);

			if(LIGH.FNAM != null)
			{
				objData.name = LIGH.FNAM.value;
			}

			if(LIGH.LHDT != null)
			{
				lightData.flags = LIGH.LHDT.flags;
				if(Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.CanCarry))
				{
					gameObject.AddComponent<BoxCollider>().size *= 0.5f; //very weak-- adding a box collider to light objects so we can interact with them
																		 //adding kinematic rigidbodies to static colliders prevents the physics collision tree from being rebuilt, which impacts performance
					if(TESUnity.instance.UseKinematicRigidbodies)
					{
						gameObject.AddComponent<Rigidbody>().isKinematic = true;
					}
				}
				StartCoroutine(ConfigureLightComponent());
			}
		}

		public IEnumerator ConfigureLightComponent()
		{
			var time = 0f;
			//wait until we have found the light component. this will typically be the frame /after/ object creation as the light component is added after this component is created
			while(lightData.lightComponent == null && time < 5f)
			{
				lightData.lightComponent = gameObject.GetComponentInChildren<Light>(true);
				yield return new WaitForEndOfFrame();
				time += Time.deltaTime;
			}
			if(lightData.lightComponent != null) //if we have found the light component by the end of the loop
			{
				// Only disable the light based on flags if the light component hasn't already been disabled due to settings.
				if(lightData.lightComponent.enabled)
				{
					lightData.lightComponent.enabled = !Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.OffDefault);
				}

				var flicker = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.Flicker);
				var flickerSlow = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.FlickerSlow);
				var pulse = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.Pulse);
				var pulseSlow = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.PulseSlow);
				var fire = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.Fire);
				var animated = flicker || flickerSlow || pulse || pulseSlow || fire;

				if(animated && TESUnity.instance.EnableAnimatedLights)
				{
					var lightAnim = lightData.lightComponent.gameObject.AddComponent<LightAnim>();
					if(flicker) lightAnim.mode = LightAnimMode.Flicker;
					if(flickerSlow) lightAnim.mode = LightAnimMode.FlickerSlow;
					if(pulse) lightAnim.mode = LightAnimMode.Pulse;
					if(pulseSlow) lightAnim.mode = LightAnimMode.PulseSlow;
					if(fire) lightAnim.mode = LightAnimMode.Fire;
				}
			}
			else
			{
				Debug.Log("Light Record Object Created Without Light Component. Search Timed Out.");
			}
		}

		public void Interact()
		{
			if(doorData != null)
			{
				if(doorData.isOpen) Close(); else Open();
				return;
			}

			if(record is ESM.BOOKRecord)
			{
				ESM.BOOKRecord BOOK = record as ESM.BOOKRecord;
				if(BOOK.TEXT != null)
				{
					Debug.Log(BOOK.TEXT.value);
				}
				return;
			}
		}



		#region door functions
		private void Open()
		{
			if(!doorData.moving) StartCoroutine(c_Open());
		}

		private void Close()
		{
			if(!doorData.moving) StartCoroutine(c_Close());
		}

		private IEnumerator c_Open()
		{
			doorData.moving = true;
			while(Quaternion.Angle(transform.rotation, doorData.openRotation) > 1f)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, doorData.openRotation, Time.deltaTime * 5f);
				yield return new WaitForEndOfFrame();
			}
			doorData.isOpen = true;
			doorData.moving = false;
		}

		private IEnumerator c_Close()
		{
			doorData.moving = true;
			while(Quaternion.Angle(transform.rotation, doorData.closedRotation) > 1f)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, doorData.closedRotation, Time.deltaTime * 5f);
				yield return new WaitForEndOfFrame();
			}
			doorData.isOpen = false;
			doorData.moving = false;
		}
		#endregion
	}
}