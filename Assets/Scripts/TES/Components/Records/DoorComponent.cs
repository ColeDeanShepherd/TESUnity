using UnityEngine;
using System.Collections;
using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class DoorComponent : GenericObjectComponent
    {
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

        void Start()
        {
            doorData = new DoorData();
            doorData.closedRotation = transform.rotation;
            doorData.openRotation = doorData.closedRotation * Quaternion.Euler(Vector3.up * 90f);
            doorData.moving = false;

            var DOOR = record as DOORRecord;
            if (DOOR.FNAM != null)
                doorData.doorName = DOOR.FNAM.value;

            doorData.leadsToAnotherCell = (refObjDataGroup.DNAM != null) || (refObjDataGroup.DODT != null);
            doorData.leadsToInteriorCell = (refObjDataGroup.DNAM != null);

            if (doorData.leadsToInteriorCell)
                doorData.doorExitName = refObjDataGroup.DNAM.value;

            if (doorData.leadsToAnotherCell && !doorData.leadsToInteriorCell)
            {
                var doorExitCell = MorrowindEngine.instance.dataReader.FindExteriorCellRecord(MorrowindEngine.instance.GetExteriorCellIndices(doorData.doorExitPos));
                doorData.doorExitName = (doorExitCell != null) ? doorExitCell.RGNN.value : doorData.doorName;
            }

            if (refObjDataGroup.DODT != null)
            {
                doorData.doorExitPos = Convert.NifPointToUnityPoint(refObjDataGroup.DODT.position);
                doorData.doorExitOrientation = Convert.NifEulerAnglesToUnityQuaternion(refObjDataGroup.DODT.eulerAngles);
            }

            objData.name = doorData.leadsToAnotherCell ? doorData.doorExitName : "Use " + doorData.doorName;
        }

        public override void Interact()
        {
            if (doorData != null)
            {
                if (doorData.isOpen)
                    Close();
                else
                    Open();
            }
        }

        private void Open()
        {
            if (!doorData.moving)
                StartCoroutine(c_Open());
        }

        private void Close()
        {
            if (!doorData.moving)
                StartCoroutine(c_Close());
        }

        private IEnumerator c_Open()
        {
            doorData.moving = true;

            while (Quaternion.Angle(transform.rotation, doorData.openRotation) > 1f)
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

            while (Quaternion.Angle(transform.rotation, doorData.closedRotation) > 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, doorData.closedRotation, Time.deltaTime * 5f);
                yield return new WaitForEndOfFrame();
            }

            doorData.isOpen = false;
            doorData.moving = false;
        }
    }
}