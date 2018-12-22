using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/*
 * CLASS LevelLoader : MonoBehaviour
 * ---------------------------------
 * Describes a button that loads a level when pressed.  Also displays
 * crucial information associated with the level, like whether it is
 * completed and lore associated with that level
 * ---------------------------------
 */ 

public class LevelLoader : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private int lvl;	// Level this loader loads
	[SerializeField]
	private Button loader;	// Button that loads the level when pressed
	[SerializeField]
	private Text levelDisplay;	// Displays the level this loader loads
	[SerializeField]
	private Image completedDisplay;	// Text object that displays how quickly the player has solved the puzzle
	private LoreDisplay theScroll;	// Reference to the script that displays the Sacred Wisdom

	// Initilialize the level loader with the specified values
	public void Initialize (int level, bool playable, bool completed, LoreDisplay scroll)
	{
		lvl = level;
		loader.interactable = playable;
		levelDisplay.text = level.ToString ();
		completedDisplay.enabled = completed;
		theScroll = scroll;
	}

	// Load level associated with the script
	public void LoadLevel ()
	{
		SceneManager.LoadScene ("Level" + lvl);
	}

	// When the pointer hovers over the object, display the verse associated with it
	public void OnPointerEnter (PointerEventData data)
	{
		if (completedDisplay.enabled) {
			theScroll.FadeVerse (true, lvl);
		}
	}

	// When the pointer exits the object, fade the verse out
	public void OnPointerExit (PointerEventData data)
	{
		if (completedDisplay.enabled) {
			theScroll.FadeVerse (false);
		}
	}
}
