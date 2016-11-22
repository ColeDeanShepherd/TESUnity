using UnityEngine;
using UnityStandardAssets.CinematicEffects;

namespace TESUnity.Components
{
    [RequireComponent(typeof(Camera))]
    public class PostEffectManager : MonoBehaviour
    {
        void Awake()
        {
            var tes = TESUnity.instance;

            SetEffectEnabled<AmbientOcclusion>(tes.ambientOcclusion);
            SetEffectEnabled<AntiAliasing>(tes.antiAliasing);
            SetEffectEnabled<Bloom>(tes.bloom);
        }

        public void SetEffectEnabled<T>(bool isEnabled) where T : MonoBehaviour
        {
            var effect = GetComponent<T>();
            if (effect != null)
                effect.enabled = isEnabled;
        }
    }
}
