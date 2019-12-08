using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{

	public AudioSource source;
	public AudioClip stepSounds;
	public float footstepDelay;
	
	public void StartFootstep()
	{
		StartCoroutine(LoopFootstepSound());
	}

	IEnumerator LoopFootstepSound()
	{
		while (true)
		{
			if (!source.isPlaying)
			{
				source.clip = stepSounds;
				source.Play();
			}
			yield return new WaitForSeconds(footstepDelay);
		}
	}

	public void StopFootstep()
	{
		StopAllCoroutines();
		source.Stop();
	}

}
