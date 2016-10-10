using UnityEngine;
using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class GenericObjectComponent : MonoBehaviour
    {
        public class ObjectData
        {
            public Texture2D icon;
            public string interactionPrefix;
            public string name;
            public string weight;
            public string value;
        }

        public CELLRecord.RefObjDataGroup refObjDataGroup = null;
        public Record record;
        public ObjectData objData = new ObjectData();

        public virtual void Interact()
        {
        }

        public static GenericObjectComponent Create(GameObject gameObject, Record record, string tag)
        {
            gameObject.tag = tag;

            var transform = gameObject.GetComponent<Transform>();

            for (int i = 0, l = transform.childCount; i < l; i++)
                transform.GetChild(i).tag = tag;

            GenericObjectComponent component = null;

            // TODO: Create a subclass InteractiveObjectSubRecord which contains NAME, FNAM and MODL
            // Will help to remove all this code

            if (record is DOORRecord)
                component = gameObject.AddComponent<DoorComponent>();

            else if (record is LIGHRecord)
                component = gameObject.AddComponent<LightComponent>();

            else if (record is BOOKRecord)
                component = gameObject.AddComponent<BookComponent>();

            else if (record is CONTRecord)
                component = gameObject.AddComponent<ContainerComponent>();

            else if (record is MISCRecord)
                component = gameObject.AddComponent<MiscObjectComponent>();

            else if (record is WEAPRecord)
                component = gameObject.AddComponent<WeaponComponent>();

            else if (record is ARMORecord)
                component = gameObject.AddComponent<ArmorComponent>();

            else if (record is INGRRecord)
                component = gameObject.AddComponent<IngredientComponent>();

            else
            {
                // TODO: Create a component for each types.
                component = gameObject.AddComponent<GenericObjectComponent>();

                if (record is ACTIRecord)
                    component.objData.name = (record as ACTIRecord).FNAM.value;

                if (record is LOCKRecord)
                    component.objData.name = (record as LOCKRecord).FNAM.value;

                if (record is PROBRecord)
                    component.objData.name = (record as PROBRecord).FNAM.value;

                if (record is REPARecord)
                    component.objData.name = (record as REPARecord).FNAM.value;

                if (record is CLOTRecord)
                    component.objData.name = (record as CLOTRecord).FNAM.value;

                if (record is ALCHRecord)
                    component.objData.name = (record as ALCHRecord).FNAM.value;

                if (record is APPARecord)
                    component.objData.name = (record as APPARecord).FNAM.value;
            }

            component.record = record;

            return component;
        }
    }
}