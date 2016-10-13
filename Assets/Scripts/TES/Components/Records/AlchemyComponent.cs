using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class AlchemyComponent : GenericObjectComponent
    {
        void Start()
        {
            var ALCH = (ALCHRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = ALCH.FNAM.value;
            objData.weight = ALCH.ALDT.weight.ToString();
            objData.value = ALCH.ALDT.value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}
