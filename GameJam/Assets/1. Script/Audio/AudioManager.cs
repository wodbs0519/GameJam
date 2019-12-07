using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

	public AudioSource Bgm;  
	public AudioSource EnvironmentSound;

	public AudioSource[] SfxSources;

	public void PlaySound(AudioClip clip)
	{
		AudioSource targetSource = null;
		foreach (var sfxSource in SfxSources)
		{
			if (!sfxSource.isPlaying)
			{
				targetSource = sfxSource;
				break;
			}
		}

		if (targetSource == null)
		{
			targetSource = SfxSources[SfxSources.Length - 1];
		}

		targetSource.clip = clip;
		targetSource.Play();
	}

	public void PauseAllSound()
	{
		Bgm.Pause();
		EnvironmentSound.Pause();
	}

	public void UnPauseAllSound()
	{
		Bgm.UnPause();
		EnvironmentSound.UnPause();
	}
	
}

