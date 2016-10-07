using UnityEngine;

namespace TESUnity
{
    public class CreatureComponent : MonoBehaviour
    {
        void Start()
        {
            transform.rotation = Quaternion.Euler(-70, 0, 0); 
        }
    }
}
