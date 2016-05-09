using UnityEngine;

public class OneShotAudioSourceComponent : MonoBehaviour
{
	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.Play();
	}
	private void Update()
	{
		if(!audioSource.isPlaying)
		{
			Destroy(gameObject);
			enabled = false;
		}
	}
}