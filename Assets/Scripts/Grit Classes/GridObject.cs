using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * CLASS GridObject : MonoBehaviour
 * --------------------------------
 * Base class for all objects that can move within the board
 * --------------------------------
 */ 
public class GridObject : MonoBehaviour 
{
	[SerializeField]
	protected float moveTime;	// Time it takes for the object to move between  
	[SerializeField]
	protected float moveLag;	// Object must wait this long after moving before moving again
	protected float inverseMoveTime;	// Inverse of move time; queued to increase efficiency
	[SerializeField]
	protected LayerMask obscuringLayer;	// Layer for objects that obscure vision
	private Rigidbody2D rb2D;	// Reference to the rigidbody on the object
	[SerializeField]
	protected List<NodeType> traversableNodes;	// Node types that this object is allowed to occupy
	protected Node currentNode;	// Node that is currently inhabited by the grid object
	[SerializeField]
	protected NodeType occupantType;	// Specifies what type of occupation the grid object gives to the node it moves into
	[SerializeField]
	private GridObjectEffects gridObjEffects;	// Script that manages effects for a grid object

	protected virtual void Start ()
	{
		// Initialize fields
		rb2D = GetComponent <Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime;

		// Get the current node at the current position and occupy it
		currentNode = GameManager.nodes [transform.IntPos2D ()];
		currentNode.Occupy (true, occupantType);
	}

	// Move to the specified node with the specified speed
	protected void MoveToNode (Node endNode)
	{
		// Tell the current node you're leaving,
		// and tell the end node you're arriving
		currentNode.Occupy (false);
		currentNode = endNode;

		// Play the footstep effect
		gridObjEffects.FootstepEffect ();

		// Start coroutine to smoothly move to destination node
		StartCoroutine ("SmoothMove", endNode.transform.position);
	}

	// Smoothly move to the end vector
	IEnumerator SmoothMove (Vector3 end)
	{
		Vector3 alongPath;	// A vector along the path that the object is moving
		float sqrRemainingDist;	// The square of the remaining distance to the end position

		// Calculate remaining distance to target
		sqrRemainingDist = (transform.position - end).sqrMagnitude;

		// Move object while square remaining distance is not negligibly small
		while (sqrRemainingDist > 0.01f) {
			// Don't bother with vector math unless we're unpaused
			if (!Timekeeper.Paused) {
				// Calculate a vector a little further along the path and move the rigidbody to that vector
				alongPath = Vector3.MoveTowards (transform.position, end, Time.deltaTime * inverseMoveTime);
				rb2D.MovePosition (alongPath);

				// Recaltulate remaining distance
				sqrRemainingDist = (transform.position - end).sqrMagnitude;
			}

			// Wait a frame, then check again
			yield return null;
		}

		// Occupy current node once we're finished moving to it
		currentNode.Occupy (true, occupantType);
	}

	// Stop smooth movement
	protected virtual void Stop ()
	{
		StopCoroutine ("SmoothMove");
	}
}
