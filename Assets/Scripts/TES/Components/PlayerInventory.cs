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

        public void Add(InventoryItemComponent item)
        {
            Add(item.record);

            // For now.
            var weapon = item.record as WEAPRecord;
            if (weapon != null)
            {
                var renderer = item.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    var colliders = item.GetComponents<MeshCollider>();
                    foreach (var collider in colliders)
                        collider.enabled = false;

                    var rightHand = GetComponent<PlayerComponent>().rightHand;

                    if (rightHand.transform.childCount > 0)
                    {
                        rightHand.transform.GetChild(0).parent = _disabledObjects;
                    }

                    var target = renderer.transform.parent;
                    target.parent = rightHand.transform;
                    target.localPosition = Vector3.zero;
                    target.localRotation = Quaternion.identity;
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