using System.Collections.Generic;
using TESUnity.Components.Records;
using TESUnity.ESM;
using UnityEngine;

namespace TESUnity.Components
{
    public class PlayerInventory : MonoBehaviour
    {
        private List<Record> _inventory = new List<Record>();
        private Transform _disabledObjects = null;

        void Start()
        {
            var disabledObjectGO = new GameObject("DisabledObjects");
            disabledObjectGO.SetActive(false);
            _disabledObjects = disabledObjectGO.GetComponent<Transform>();
        }

        public void Add(GenericObjectComponent item)
        {
            Add(item.record);

            // For now.
            var weapon = item.record as WEAPRecord;
            if (weapon != null)
            {
                //var p = obj.transform;
                //while (p.parent != null && p.parent.gameObject.name != "objects")
                    //p = p.parent;

                var renderer = item.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    var rightHand = GetComponent<PlayerComponent>().rightHand;
                    if (rightHand.childCount > 0)
                        rightHand.GetChild(0).parent = _disabledObjects;

                    ((WeaponComponent)item).Equip(rightHand.transform);
                    /*var target = renderer.transform.parent;
                    target.parent = rightHand.transform;
                    target.localPosition = Vector3.zero;
                    target.localRotation = Quaternion.identity;*/
                    return;
                }
            }

            item.transform.parent = _disabledObjects.transform;
        }

        public void Add(Record record)
        {
            _inventory.Add(record);
        }
    }
}