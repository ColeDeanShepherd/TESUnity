using UnityEngine;

namespace TESUnity.Components
{
    public class CreatureComponent : GenericObjectComponent
    {
        void Start()
        {
            transform.rotation = Quaternion.Euler(-70, 0, 0); 
        }
    }
}
