using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * CLASS DistractionEffects : MonoBehaviour
 * ----------------------------------------
 * Handles higher-level visual-audio effects for the distraction object
 * ----------------------------------------
 */ 
public class DistractionEffects : MonoBehaviour 
{
	[SerializeField]
	private Animator distractAnim;	// Animators that animate the various visuals of the distraction effect
	[SerializeField]
	private List<SpriteRenderer> crossRends;	// Sprite renderers on the cross object that indicates the range of the distraction effect
	[SerializeField]
	private List<AudioClip> distractClips;	// Clip that plays when the distraction activates

	// Enable/disable distraction visual and audio
	public void DistractEffect (bool active)
	{
		// Enable/disable animation
		distractAnim.SetBool ("Distracting", active);

		// Enable or disable the cross renderers
		foreach (SpriteRenderer rend in crossRends) {
			rend.enabled = !active;
		}

		// If distraction is activating, play the clip
		if (active) {
			SoundPlayer.Instance.PlayRandomEffect (distractClips);
		} // END if
	} // END method
} // END class
