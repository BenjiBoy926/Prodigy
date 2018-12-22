using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * CLASS Distraction : MonoBehaviour
 * ---------------------------------
 * Describes the object that distracts enemies.  Moves to grid position
 * nearest to the mouse and distracts on click if enemies are in range
 * ---------------------------------
 */ 
public class Distraction : MonoBehaviour 
{
	private Rigidbody2D rb2D;	// Rigidbody on this object
	[SerializeField]
	private DistractionEffects effects;	// Handles visual/audio effects for the object

	// List of enemies within range of the distraction
	private List<Enemy> enemiesInRange = new List<Enemy> ();
	private Camera eventCamera;	// Position of the mouse is measured relative to this camera
	private bool distractReady;	// True if distract is ready for use again
	[SerializeField]
	private float recharge;	// Time it takes for the distraction to recharge after use

	void Start ()
	{
		rb2D = GetComponent <Rigidbody2D> ();
		DistractMode (false);
		eventCamera = Camera.main;
	}

	void Update ()
	{
		bool mouseDown;	// True in the frame the mouse is pressed
		Vector3 mousePos;	// Position of the mouse in world coordinates relative to the specified camera
		Vector2 mouseGridPos;	// 2D integer grid position the mouse is closest to
		float sqrDistToMouse;	// Square distance between the mouse's grid position and the current position

		mouseDown = Input.GetButtonDown ("Fire1");

		// Allow object to move only if distraction is ready and game hasn't ended
		if (distractReady && !GameManager.GameOver) {
			// Get mouse position and distance between it and this position
			mousePos = eventCamera.ScreenToWorldPoint (Input.mousePosition);
			mouseGridPos = new Vector2 (Mathf.Round (mousePos.x), Mathf.Round (mousePos.y));
			sqrDistToMouse = (mouseGridPos - (Vector2)transform.position).sqrMagnitude;

			// If distance to mouse is not negligible, move the object
			if (sqrDistToMouse > 0.5f) {
				rb2D.MovePosition (new Vector2 (Mathf.Round (mousePos.x), Mathf.Round (mousePos.y)));
			}

			// If mouse was pressed and enemies are in range, cause distraction
			if (mouseDown && enemiesInRange.Count > 0) {
				StopCoroutine ("Distract");
				StartCoroutine ("Distract");
			}
		}
	}

	// Disracts enemies by activating correct objects
	// for a certain amount of time, then returns to preview mode
	IEnumerator Distract ()
	{
		// Distract each enemy in the list of enemies in range
		foreach (Enemy enemy in enemiesInRange) {
			enemy.Distract ((Vector2)transform.position);
		}

		// Go into distract mode
		DistractMode (true);

		// Wait for time it takes to recharge distraction
		yield return new WaitForSeconds (recharge);

		// Deactivate distract mode
		DistractMode (false);
	}

	// Set certain object active or inactive depending on whether or not the object is distracting enemies
	private void DistractMode (bool active)
	{
		distractReady = !active;
		effects.DistractEffect (active);
	}

	// Add enemies that enter the trigger to list of enemies in range
	private void OnTriggerEnter2D (Collider2D other)
	{
		Enemy attemptEnemy;	// Stores the result of an attempt to get an enemy script on the object that entered this trigger
		attemptEnemy = other.GetComponent <Enemy> ();

		// If we successfully got an enemy and it isn't already in the list, add it to the list
		if (attemptEnemy != null && !enemiesInRange.Contains (attemptEnemy))
			enemiesInRange.Add (attemptEnemy);
	}

	// Remove enemies from the list of enemies in range
	private void OnTriggerExit2D (Collider2D other)
	{
		Enemy attemptEnemy;	// Stores the result of an attempt to get an enemy script on the object that enters the trigger
		attemptEnemy = other.GetComponent <Enemy> ();

		// If we successfully got an enemy and it is in the list, remove it
		if (attemptEnemy != null && enemiesInRange.Contains (attemptEnemy))
			enemiesInRange.Remove (attemptEnemy);
	}
}
