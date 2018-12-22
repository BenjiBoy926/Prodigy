using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * CLASS EnemyEffects : MonoBehaviour
 * ----------------------------------
 * Manages higher-level tasks that add polish to the game enemy,
 * such as displaying enemy line of sight, questions marks and
 * exclamation points as well as a slider to show how long the 
 * enemy will remain distracted
 * ----------------------------------
 */ 
public class EnemyEffects : MonoBehaviour 
{
	[SerializeField]
	private RectTransform sightRect;	// Rect transform of the object that visualizes the enemy's line of sight
	[SerializeField]
	private Animator emote;	// Object that emotes for the enemy by displaying question mark / exclamation point
	[SerializeField]
	private Slider distractSlider;	// Slider displaying how much time is left for enemy to be distracted 
	[SerializeField]
	private Sprite confusedSprite;	// Sprite displayed when enemy is confused
	[SerializeField]
	private Sprite astonishedSprite;	// Sprite displayed when the enemy discovers the player
	[SerializeField]
	private List<AudioClip> distractClips;	// Possible clips to play when enemy is distracted
	[SerializeField]
	private List<AudioClip> catchClips;	// Possible clips to play when enemy successfully catches the player
	[SerializeField]
	private List<AudioClip> returnClips;	// Possible clips to play when enemy returns to patrol

	void Update ()
	{
		if (distractSlider.gameObject.activeInHierarchy) {
			distractSlider.value -= Time.deltaTime;
		}
	}

	// Visualize line of sight by assigning given values into the rect transform
	public void VisualizeLineOfSight (Vector2 dir, float dist)
	{
		float angleFromRight;	// Signed angle between the direction specified and the right vector

		// Get angle between right and directional vectors and set the negative to the sight rect's z-rotation
		angleFromRight = Vector2.SignedAngle (dir, Vector2.right);
		sightRect.localRotation = Quaternion.Euler (0f, 0f, -angleFromRight);

		// Set length of rectangle according to distance specified
		sightRect.sizeDelta = new Vector2 (dist, 1f);
	}

	// Enable the given emotion, or disable whichever one is being displayed
	public void Emote (bool emoting, EnemyEmotes thisEmote = EnemyEmotes.Confused)
	{
		// If we're going to emote, set the right sprite and cause object to fade in
		if (emoting) {
			SpriteRenderer emoteRend;	// Renderer on the emoting object
			Sprite spriteToDisplay;	// Sprite to display depending on emotion specified

			// Get sprite renderer
			emoteRend = emote.GetComponent <SpriteRenderer> ();

			// Set sprite to display based on emote specified
			if (thisEmote == EnemyEmotes.Confused) {
				spriteToDisplay = confusedSprite;
				SoundPlayer.Instance.PlayRandomEffect (distractClips);
			} else {
				spriteToDisplay = astonishedSprite;
				SoundPlayer.Instance.PlayRandomEffect (catchClips);
			}

			// Display the sprite
			emoteRend.sprite = spriteToDisplay;
			emote.SetBool ("Emoting", true);
		}
		// Otherwise, deactivate emoting object
		else {
			emote.SetBool ("Emoting", false);
			SoundPlayer.Instance.PlayRandomEffect (returnClips);
		}
	}

	// Enable or disable the distract slider
	public void EnableDistractSlider (bool active, float distractTime = 1f)
	{
		distractSlider.gameObject.SetActive (active);

		// If we're activating the slider, set the max and current value to parameter value
		if (active) {
			distractSlider.maxValue = distractTime;
			distractSlider.value = distractTime;
		}
	}
}

public enum EnemyEmotes
{
	Confused,
	Astonished
}
