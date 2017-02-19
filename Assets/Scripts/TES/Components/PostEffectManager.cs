using UnityEngine;
using UnityEngine.PostProcessing;

namespace TESUnity.Components
{
    [RequireComponent(typeof(Camera))]
    public class PostEffectManager : MonoBehaviour
    {
        void Awake()
        {
            var tes = TESUnity.instance;

            // Toggle effects depending on user selection
            SetEffectEnabled
            (
                tes.antiAliasing,
                tes.ambientOcclusion,
                tes.depthOfField,
                tes.motionblur,
                tes.eyeAdaption,
                tes.bloom,
                tes.vignette
            );
        }

        public void SetEffectEnabled(bool AA, bool AO, bool DOF, bool MBlur, bool EyeAdaption, bool Bloom, bool Vignette) 
        {
            PostProcessingBehaviour  postprocessBehaviour = GetComponent<PostProcessingBehaviour>(); 

            if (postprocessBehaviour != null)
            {
                postprocessBehaviour.profile.antialiasing.enabled = AA;
                postprocessBehaviour.profile.antialiasing.enabled = AO;
                postprocessBehaviour.profile.antialiasing.enabled = DOF;
                postprocessBehaviour.profile.antialiasing.enabled = MBlur;
                postprocessBehaviour.profile.antialiasing.enabled = EyeAdaption;
                postprocessBehaviour.profile.antialiasing.enabled = Bloom;
                postprocessBehaviour.profile.antialiasing.enabled = Vignette;
            }
                
        }
    }
}
