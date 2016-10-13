using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class ProbComponent : GenericObjectComponent
    {
        void Start()
        {
            var PROB = (PROBRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = PROB.FNAM.value;
            objData.weight = PROB.PBDT.weight.ToString();
            objData.value = PROB.PBDT.value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}