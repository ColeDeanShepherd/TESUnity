using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class WeaponComponent : GenericObjectComponent
    {
        void Start()
        {
            var WEAP = (WEAPRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = WEAP.FNAM.value;
            objData.weight = WEAP.WPDT.weight.ToString();
            objData.value = WEAP.WPDT.value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}
