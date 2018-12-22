using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * CLASS Node : MonoBehaviour
 * --------------------------
 * Base class for all tiles.  Allows information about
 * traversability of tiles to be neatly sorted
 * --------------------------
 */ 
public class Node : MonoBehaviour 
{
	private NodeType currentType;	// Type for this current node
	[SerializeField]
	private NodeType defaultType;	// Type of thise node when unoccupied

	public NodeType CurrentType { get { return currentType; } }

	// Initialize variables
	private void Awake ()
	{
		currentType = defaultType;

		// Add this node to the central dictionary of nodes
		GameManager.nodes.Add (transform.IntPos2D (), this);
	}

	// Method for making this node occupied.  Allows the calling class to
	// specify what type of object is entering by "newNodeType"
	public void Occupy (bool entering, NodeType newNodeType = NodeType.Plain)
	{
		if (entering) {
			currentType = newNodeType;
		} else {
			currentType = defaultType;
		}
	}

	// Set the default type of the node to the new one specified
	public void DefaultToType (NodeType newDefaultType)
	{
		defaultType = newDefaultType;
	}

	// Returns a list of the nodes above, below, to the left and to the right of the current node
	public List<Node> GetCardinalNeighbors ()
	{
		List<Node> neighbors = new List<Node> ();	// List of neighbors to be returned
		Node tryNode = null;	// Dummy variable used to see if the node being checked exists in the dictionary in the first place

		// Add each of the neighbors to the list, if they exist in the dictionary
		if (GameManager.nodes.TryGetValue (transform.IntPos2D () + Vector2Int.up, out tryNode)) {
			neighbors.Add (tryNode);
		}
		if (GameManager.nodes.TryGetValue (transform.IntPos2D () + Vector2Int.down, out tryNode)) {
			neighbors.Add (tryNode);
		}
		if (GameManager.nodes.TryGetValue (transform.IntPos2D () + Vector2Int.left, out tryNode)) {
			neighbors.Add (tryNode);
		}
		if (GameManager.nodes.TryGetValue (transform.IntPos2D () + Vector2Int.right, out tryNode)) {
			neighbors.Add (tryNode);
		}

		return neighbors;
	}

	// Returns the square of the distance between two nodes
	public static float SqrDistBetweenNodes (Node node1, Node node2)
	{
		return (node1.transform.position - node2.transform.position).sqrMagnitude;
	}

	// Returns "rect" distance between nodes as the sum of vertical and
	// horizontal offset, corresponding to the number of up-down left-right
	// moves required to get between the two nodes
	public static int RectDistBetweenNodes (Node node1, Node node2)
	{
		Vector2Int node1Rect;	// Position of node1 as a v2Int
		Vector2Int node2Rect;	// Position of node2 as a v2Int
		int dx;	// Horizontal offset of nodes
		int dy;	// Vertical offset of nodes

		// Convert positions of nodes to integer 2D coordinates
		node1Rect = node1.transform.IntPos2D ();
		node2Rect = node2.transform.IntPos2D ();

		// Get horizontal and vertical offsets and return the sum
		dx = Mathf.Abs (node1Rect.x - node2Rect.x);
		dy = Mathf.Abs (node1Rect.y - node2Rect.y);
		return dx + dy;
	}
}

public enum NodeType
{
	Plain,
	Wall,
	Hiding,
	Locked,
	HasPlayer,
	HasEnemy,
	HasKey
}