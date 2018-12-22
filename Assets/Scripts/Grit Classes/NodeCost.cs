using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A very basic class that pairs a node with a cost value,
// which could represent its distance from another node
public class NodeCost : IComparable<NodeCost> 
{
	Node pairedNode;
	float cost;

	public Node PairedNode { get { return pairedNode; } }
	public float Cost { get { return cost; } }

	// Constructor initializes local variable pair
	public NodeCost (Node theNode, float theCost)
	{
		pairedNode = theNode;
		cost = theCost;
	}

	// Lists of NodeCost use this method to sort
	// node costs from least to greatest cost
	public int CompareTo (NodeCost other)
	{
		int thisCost;	// Cost of this node rounded to an integer
		int otherCost;	// Cost of other node rounded to an integer

		if (other == null) {
			return 1;
		}

		// Round costs to integers and return the difference
		thisCost = Mathf.RoundToInt (cost);
		otherCost = Mathf.RoundToInt (other.cost);
		return thisCost - otherCost;
	}
}
