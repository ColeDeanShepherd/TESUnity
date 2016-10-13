using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class ArmorComponent : GenericObjectComponent
    {
        void Start()
        {
            var ARMO = (ARMORecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = ARMO.FNAM.value;
            objData.weight = ARMO.AODT.weight.ToString();
            objData.value = ARMO.AODT.value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}