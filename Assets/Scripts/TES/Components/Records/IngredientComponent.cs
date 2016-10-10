using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class IngredientComponent : GenericObjectComponent
    {
        void Start()
        {
            var INGR = (INGRRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(INGR.ITEX.value, "icons"); 
            objData.name = INGR.FNAM.value;
            objData.weight = INGR.IRDT.weight.ToString();
            objData.value = INGR.IRDT.value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}
