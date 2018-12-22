using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * CLASS Player : GridObject
 * -------------------------
 * Player class allows player input to take control of a grid object
 * -------------------------
 */ 
public class Player : GridObject
{
	[SerializeField]
	private PlayerEffects effects;	// Reference to script that manages effects for the player
	private bool moveReady;	// True if it is possible for the player to move
	private float moveReadyTimer;	// Used to see if player is done moving so they can move again
	private int keys;	// Number of keys the player owns

	protected override void Start ()
	{
		moveReadyTimer = 0f;
		keys = 0;
		effects.UpdateKeyDisplay (keys);
		base.Start ();
	}

	protected void Update ()
	{
		int horizontal;	// Integer reads horizontal input
		int vertical;	// Integer reads vertical input
		Vector2Int dir;	// Stores directional input as an integer vector

		// Player is ready to move when they have stopped moving already
		moveReady = moveReadyTimer < Time.time;

		// Set horizontal and vertical based on input
		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		// If horizontal is non-zero, set vertical to zero
		// Prevents diagonal movement
		if (horizontal != 0) {
			vertical = 0;
		}

		// If horizontal or vertical input was given, attempt to move in that direction
		if (horizontal != 0 || vertical != 0) {
			dir = new Vector2Int (horizontal, vertical);
			AttemptMove (transform.IntPos2D () + dir);
		}
	}

	// Check to see if target node is traversable,
	// if so move to it
	private void AttemptMove (Vector2Int end)
	{
		// Only allow activity if movement is ready and the game hasn't ended
		if (moveReady && !GameManager.GameOver) {
			Node endNode;	// Node to which object is trying to move
			endNode = GameManager.nodes [end];

			// If target node is of a type that is traversable, and we are ready to move again, move to that node
			if (traversableNodes.Contains (endNode.CurrentType)) {
				// If the target node has a key, collect it
				if (endNode.CurrentType == NodeType.HasKey) {
					GetKeyFromNode (endNode);
				}

				// Move to the node and set move ready timer
				MoveToNode (endNode);
				moveReady = false;
				moveReadyTimer = Time.time + moveTime + moveLag;
			}

			// If target node is locked, try to unlock it
			if (endNode.CurrentType == NodeType.Locked) {
				AttemptUnlockNode (endNode);
				moveReady = false;
				moveReadyTimer = Time.time + moveTime + moveLag;
			} // END if locked
		}// END if move ready and not game over
	}// END method

	// Collect key from the node specified, if it has one
	private void GetKeyFromNode (Node nodeWithKey)
	{
		Key key;	// The key on the node specified
		key = nodeWithKey.GetComponentInChildren<Key> ();

		// If we found a key on the target node...
		if (key != null) {
			//...make it leave the node and increase keys
			key.LeaveNode (nodeWithKey);
			keys++;
			effects.UpdateKeyDisplay (keys);
		} else {
			Debug.Log ("No key component was found on node at " + nodeWithKey.transform.IntPos2D ());
		}
	}

	// Try to unlock the lock on the specified node
	private void AttemptUnlockNode (Node lockedNode)
	{
		Lock theLock;	// The lock on the node specified
		bool unlockSuccessful;	// True if we successfully unlocked the specified node
		theLock = lockedNode.GetComponentInChildren<Lock> ();

		// If we found a lock on the node, try to unlock it
		if (theLock != null) {
			unlockSuccessful = theLock.TryUnlock (keys, lockedNode);

			// If we were able to unlock the node, move to it
			if (unlockSuccessful) {
				MoveToNode (lockedNode);
			}
		}
	}

	// If player enters the trigger of the ending node, move on to the next stage
	private void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Finish") {
			GameManager.Instance.EndGame (true);
			effects.PlayEndingEffect ();
		}
	}

	// Called if the player is caught by an enemy
	public void Caught ()
	{
		Stop ();
	}
}
