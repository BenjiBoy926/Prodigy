using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * CLASS GridObjectEffects : MonoBehaviour
 * ---------------------------------------
 * Manages higher-level effects for a grid object, such as
 * randomly producing an effect for movement
 * ---------------------------------------
 */ 
public class GridObjectEffects : MonoBehaviour 
{
	[SerializeField]
	private List<AudioClip> footstepClips;	// Possible audio clips for a footstep

	// Randomly play a footstep effect
	public void FootstepEffect ()
	{
		SoundPlayer.Instance.PlayRandomEffect (footstepClips);
	}
}
