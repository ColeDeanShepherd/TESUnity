using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class ClothComponent : GenericObjectComponent
    {
        void Start()
        {
            var CLOT = (CLOTRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = CLOT.FNAM.value;
            objData.weight = CLOT.CTDT.weight.ToString();
            objData.value = CLOT.CTDT.value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}