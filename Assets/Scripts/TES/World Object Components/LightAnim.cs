using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TESUnity
{
	public enum LightAnimMode { None, Flicker, FlickerSlow, Pulse, PulseSlow, Fire }//None is included as courtesy
	public class LightAnim : MonoBehaviour
	{
		public LightAnimMode mode = LightAnimMode.None;
		new Light light;
		float baseIntensity = 1f;

		void Start ()
		{
			//Debug.Log("Animated Light Created: " + mode);
			light = GetComponent<Light>();
			baseIntensity = light.intensity;
		}

		void Update ()
		{
			float value = 1f;
			switch (mode)
			{
				case LightAnimMode.None:
					break;
				case LightAnimMode.Flicker:
					value = Mathf.Round(Mathf.Clamp01(Random.value + 0.1f));
					break;
				case LightAnimMode.FlickerSlow:
					value = Mathf.Round(Mathf.Clamp01(Random.value + 0.47f));
					break;
				case LightAnimMode.Pulse:
					value = Mathf.Sin(Time.time) * 0.5f + 0.5f;
					break;
				case LightAnimMode.PulseSlow:
					value = Mathf.Sin(Time.time * 0.5f) * 0.5f + 0.5f;
					break;
				case LightAnimMode.Fire:
					value = Mathf.PerlinNoise(Time.time * 20f , 7f);
					break;
			}
			light.intensity = baseIntensity * value;
		}
	}
}