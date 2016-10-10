using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class MiscObjectComponent : GenericObjectComponent
    {
        void Start()
        {
            var MISC = (MISCRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(MISC.ITEX.value, "icons"); 
            objData.name = MISC.FNAM.value;
            objData.weight = MISC.MCDT.weight.ToString();
            objData.value = MISC.MCDT.value.ToString();
            objData.interactionPrefix = "Take ";
        }

        public override void Interact()
        {

        }
    }
}
