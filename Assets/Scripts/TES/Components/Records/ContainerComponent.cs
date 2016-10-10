using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class ContainerComponent : GenericObjectComponent
    {
        void Start()
        {
            objData.name = ((CONTRecord)record).FNAM.value;
            objData.interactionPrefix = "Open ";
        }
    }
}