using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * CLASS PlayerEffects : MonoBehaviour
 * -----------------------------------
 * Handles higher-level effects of the player, such as displaying
 * number of keys collected and sound effects
 * -----------------------------------
 */ 
public class PlayerEffects : MonoBehaviour 
{
	[SerializeField]
	private List<Image> keyImages;	// Images of the keys displayed to show how many keys the player has
	[SerializeField]
	private AudioClip endingClip;	// Clip that plays when player reaches ending tile

	// Update key display by activating the number of key images as keys
	public void UpdateKeyDisplay (int keys)
	{
		// For each key we have, activate a key image
		for (int index = 0; index < keyImages.Count; index++) {
			keyImages [index].enabled = index < keys;
		}

		// Log a warning if we don't have enough key images to display the keys
		if (keys >= keyImages.Count) {
			Debug.LogWarning ("You don't have enough key images to display the number of keys you have");
		}
	}

	// Play effect that plays when player reaches end tile
	public void PlayEndingEffect ()
	{
		SoundPlayer.Instance.PlaySoundEffect (endingClip);
	}
}
