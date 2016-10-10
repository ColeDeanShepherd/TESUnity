using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class RepaireComponent : GenericObjectComponent
    {
        void Start()
        {
            var REPA = (REPARecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = REPA.FNAM.value;
            objData.weight = REPA.RIDT.weight.ToString();
            objData.value = REPA.RIDT.value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}