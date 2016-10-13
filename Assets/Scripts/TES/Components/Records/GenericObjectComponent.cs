using UnityEngine;
using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public interface IUsableComponent
    {
        void Use();
    }

    public interface IPickableComponent
    {
        void Pick();
    }

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

        protected Transform m_transform = null;
   
        public CELLRecord.RefObjDataGroup refObjDataGroup = null;
        public Record record;
        public ObjectData objData = new ObjectData();
        public bool usable = false;
        public bool pickable = true;

        protected virtual void Awake()
        {
            m_transform = GetComponent<Transform>();
        }

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

            else if (record is ACTIRecord)
                component = gameObject.AddComponent<ActivatorComponent>();

            else if (record is LOCKRecord)
                component = gameObject.AddComponent<LockComponent>();

            else if (record is PROBRecord)
                component = gameObject.AddComponent<ProbComponent>();

            else if (record is REPARecord)
                component = gameObject.AddComponent<RepaireComponent>();

            else if (record is CLOTRecord)
                component = gameObject.AddComponent<ClothComponent>();

            else if (record is ALCHRecord)
                component = gameObject.AddComponent<AlchemyComponent>();

            else if (record is APPARecord)
                component = gameObject.AddComponent<AlchemyApparatusComponent>();

            else
                component = gameObject.AddComponent<GenericObjectComponent>();

            component.record = record;

            return component;
        }
    }
}