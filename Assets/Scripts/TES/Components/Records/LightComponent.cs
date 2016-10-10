using UnityEngine;
using System.Collections;

namespace TESUnity.Components.Records
{
    public class LightComponent : GenericObjectComponent
    {
        [System.Serializable]
        public class LightData
        {
            public Light lightComponent;
            public enum LightFlags
            {
                Dynamic = 0x0001,
                CanCarry = 0x0002,
                Negative = 0x0004,
                Flicker = 0x0008,
                Fire = 0x0010,
                OffDefault = 0x0020,
                FlickerSlow = 0x0040,
                Pulse = 0x0080,
                PulseSlow = 0x0100
            }

            public int flags;
        }

        public LightData lightData = null;

        void Start()
        {
            lightData = new LightData();
            ESM.LIGHRecord LIGH = record as ESM.LIGHRecord;
            lightData.lightComponent = gameObject.GetComponentInChildren<Light>(true);

            if (LIGH.FNAM != null)
            {
                objData.name = LIGH.FNAM.value;
            }

            if (LIGH.LHDT != null)
            {
                lightData.flags = LIGH.LHDT.flags;
                if (Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.CanCarry))
                {
                    gameObject.AddComponent<BoxCollider>().size *= 0.5f; //very weak-- adding a box collider to light objects so we can interact with them
                                                                         //adding kinematic rigidbodies to static colliders prevents the physics collision tree from being rebuilt, which impacts performance
                    if (TESUnity.instance.useKinematicRigidbodies)
                    {
                        gameObject.AddComponent<Rigidbody>().isKinematic = true;
                    }
                }
                StartCoroutine(ConfigureLightComponent());
            }
        }

        public IEnumerator ConfigureLightComponent()
        {
            var time = 0f;
            //wait until we have found the light component. this will typically be the frame /after/ object creation as the light component is added after this component is created
            while (lightData.lightComponent == null && time < 5f)
            {
                lightData.lightComponent = gameObject.GetComponentInChildren<Light>(true);
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
            }

            if (lightData.lightComponent != null) //if we have found the light component by the end of the loop
            {
                // Only disable the light based on flags if the light component hasn't already been disabled due to settings.
                if (lightData.lightComponent.enabled)
                {
                    lightData.lightComponent.enabled = !Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.OffDefault);
                }

                var flicker = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.Flicker);
                var flickerSlow = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.FlickerSlow);
                var pulse = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.Pulse);
                var pulseSlow = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.PulseSlow);
                var fire = Utils.ContainsBitFlags((uint)lightData.flags, (uint)LightData.LightFlags.Fire);
                var animated = flicker || flickerSlow || pulse || pulseSlow || fire;

                if (animated && TESUnity.instance.animateLights)
                {
                    var lightAnim = lightData.lightComponent.gameObject.AddComponent<LightAnim>();
                    if (flicker)
                        lightAnim.mode = LightAnimMode.Flicker;

                    if (flickerSlow)
                        lightAnim.mode = LightAnimMode.FlickerSlow;

                    if (pulse)
                        lightAnim.mode = LightAnimMode.Pulse;

                    if (pulseSlow)
                        lightAnim.mode = LightAnimMode.PulseSlow;

                    if (fire)
                        lightAnim.mode = LightAnimMode.Fire;
                }
            }
            else
                Debug.Log("Light Record Object Created Without Light Component. Search Timed Out.");
        }
    }
}