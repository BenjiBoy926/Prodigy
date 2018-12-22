using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * CLASS PatrolPoint
 * -----------------
 * Gives the enemy essential information about their patrol route,
 * including the node to which they are going and how much time they
 * will wait at it
 * -----------------
 */ 
[Serializable]
public class PatrolPoint 
{
	[SerializeField]
	private Vector2Int waypointIndex;	// Index on the map to which the enemy is headed
	private Node waypoint;	// Node to which the enemy is headed
	[SerializeField]
	private float waitTime;	// Time for which enemy will wait at the patrol point

	public Node Waypoint { get { return waypoint; } }
	public float WaitTime { get { return waitTime; } }

	// Get the waypoint node based on the central dictionary on the game manager
	public void Initialize ()
	{
		Node tempNode;

		if (GameManager.nodes.TryGetValue (waypointIndex, out tempNode)) {
			waypoint = tempNode;
		} else {
			Debug.LogError ("Patrol point didn't find a node at: " + waypointIndex);
		}
	}
}
