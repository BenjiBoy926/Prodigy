using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * CLASS Enemy : GridObject
 * ------------------------
 * Describes patrolling enemies.  Allows them to have preset patrol patterns,
 * interupt their patterns and look towards distractions, and return to patterns again.
 * ------------------------
 */ 
public class Enemy : GridObject 
{
	[SerializeField]
	private EnemyEffects effects;	// Reference to script that manages this enemy's sound and visual effects
	[SerializeField]
	private float distractTime;	// Time for which enemy is distracted
	[SerializeField]
	private List<PatrolPoint> waypoints = new List<PatrolPoint> ();	// List of patrol points that the enemy goes to on patrol
	[SerializeField]
	private Vector2 defaultSightDir;	// Default direction for the enemy's line of sight, if it has one
	private Vector2 currentSightDir;	// Current direction for the enemy's line of sight
	private int currentWaypoint;	// Current patrol point to which the enemy is patrolling
	private Node previousNode;	// Node visited previously by the enemy
	[SerializeField]
	private SightType sight;	// Type of line of sight for this enemy

	[SerializeField]
	private int sightRefreshRate;	// Number of times that sightlines refresh per physics update
	private int physicsUpdates;	// Number of times the physics engine has updated since the last time the sight line was refreshed

	private bool distracted;	// True when the enemy is distracted
	public bool Distracted { get { return distracted; } }

	protected override void Start ()
	{
		base.Start ();
		physicsUpdates = 0;
		currentWaypoint = 0;
		distracted = false;

		// If waypoints were specified for the enemy's patrol route...
		if (waypoints.Count > 0) {
			//...initialize the patrol points
			foreach (PatrolPoint waypoint in waypoints) {
				waypoint.Initialize ();
			}

			// Start patrolling
			StartCoroutine ("Patrol");
		}

		// Calculate sight direction
		currentSightDir = CalcSightDir ();
	}

	private void FixedUpdate ()
	{
		// Increase counter of physics updates
		physicsUpdates++;

		// Use mod to refresh line of sight only on certain updates
		if (sightRefreshRate % physicsUpdates == 0 && !GameManager.GameOver) {
			physicsUpdates = 0;
			CastLineOfSight (currentSightDir);
		}
	}

	// Infinite loop continually moves the enemy along their preset patrol route
	IEnumerator Patrol ()
	{
		// Infinite loop keeps the enemy patrolling unless coroutine is manually stopped
		while (true) {
			// If we haven't already reached the current waypoint...
			if (waypoints [currentWaypoint].Waypoint != currentNode) {
				//...move closer to it.  Wait until we're done moving to continue
				MoveCloserToNode (waypoints [currentWaypoint].Waypoint);
				yield return new WaitForSeconds (moveTime + moveLag);
			}
			// Otherwise, if we did reach the current waypoint...
			else {
				//...start by waiting for the time specified by the patrol point data
				yield return new WaitForSeconds (waypoints [currentWaypoint].WaitTime);

				// Go to next waypoint
				currentWaypoint++;

				// If current waypoint is equal to total waypoints, reset it back to zero
				if (currentWaypoint >= waypoints.Count) {
					currentWaypoint = 0;
				}
			}
		}
	}

	// A simple wayfinding method that works by moving the enemy to the neighboring node
	// closest to the specified node.  Can move enemies around obstacles, but only very simple ones
	private void MoveCloserToNode (Node end)
	{
		List<Node> neighbors = new List<Node> ();	// List of nodes neighboring the enemy at the moment
		List<NodeCost> nodeCostPairs = new List<NodeCost> ();	// A sorted list that sorts traversable neighbors based on distance from end node

		// Fill list with neighbors of the current node
		neighbors = currentNode.GetCardinalNeighbors ();

		// Pair neighboring nodes with their distance from end node if they are traversable or contain the player
		foreach (Node node in neighbors) {
			if (traversableNodes.Contains (node.CurrentType) || node.CurrentType == NodeType.HasPlayer) {
				nodeCostPairs.Add (new NodeCost (node, Node.SqrDistBetweenNodes (node, end)));
			}
		}

		// Sort the list least to greatest
		nodeCostPairs.Sort ();

		// Attempt to move to the first node in the list,
		// since it is the closet to the end
		AttemptMove (nodeCostPairs [0].PairedNode);
	}

	// Enemy attempts to move to the node specified
	private void AttemptMove (Node end)
	{
		// If end node is traversable, move to it 
		// and recalculate current sight direction
		if (traversableNodes.Contains (end.CurrentType)) {
			MoveToNode (end);
			currentSightDir = CalcSightDir ();
		}
		// If the node we're attempting to move to is occupied by the player...
		else if (end.CurrentType == NodeType.HasPlayer) {
			//...cast line of sight towards the player to catch them
			CastLineOfSight ((Vector2)(end.transform.position - transform.position));
		}
	}

	// Initiate physics logic that tries to hit the player with a line cast
	// Calls another method to tweak UI objects to visualize the line cast
	private void CastLineOfSight (Vector2 dir)
	{
		RaycastHit2D hit;	// Queues results of raycast
		Player playerSeen;	// Player caught by the enemy's line of sight

		// Cast a circle into the scene in the direction specified
		hit = Physics2D.Raycast ((Vector2)transform.position, dir, 100f, obscuringLayer);

		// If we hit something, recalculate line of sight visual
		if (hit.transform != null) {
			effects.VisualizeLineOfSight (dir, hit.distance);

			// Try to get a player script on the object hit
			playerSeen = hit.transform.GetComponent <Player> ();

			// If we successfully got a player script, catch them
			if (playerSeen != null) {
				CatchPlayer (playerSeen);
			}
		}
	}

	// Distract enemy by making them look towards the distraction, then resume patrolling after distract time
	public void Distract (Vector2 distractPos)
	{
		Vector2 diff;	// Difference between this objects position and the position of the distraction

		// Stop movement and set distracted to true
		distracted = true;
		Stop ();

		// Calculate difference
		diff = distractPos - (Vector2)(transform.position);

		// If difference is not negligibly small, make enemy look towards it
		if (diff.sqrMagnitude > 0.2f) {
			currentSightDir = diff;
		}
		// Otherwise, just make them turn around
		else {
			currentSightDir = -currentSightDir;
		}

		// Make enemy look confused
		effects.Emote (true, EnemyEmotes.Confused);
		effects.EnableDistractSlider (true, distractTime);

		// Cause enemy to resume patrol after distraction is up
		Invoke ("ResumePatrol", distractTime);
	}

	// Resume patrolling
	private void ResumePatrol ()
	{
		distracted = false;
		currentSightDir = CalcSightDir ();

		// Resume patrol if waypoints are specified
		if (waypoints.Count > 0) {
			StopCoroutine ("Patrol");
			StartCoroutine ("Patrol");
		}

		// Stop emoting
		effects.Emote (false);
		effects.EnableDistractSlider (false);
	}

	// Catch player by telling them its game over and visualize line of sight to look directly at them
	private void CatchPlayer (Player playerCaught)
	{
		Stop ();
		currentSightDir = (Vector2)(playerCaught.transform.position - transform.position);

		// Visualize line of sight and make enemy look excited
		effects.VisualizeLineOfSight (currentSightDir, currentSightDir.magnitude + 1f);
		effects.Emote (true, EnemyEmotes.Astonished);
		effects.EnableDistractSlider (false);

		// Catch player and cause game over
		playerCaught.Caught ();
		GameManager.Instance.EndGame (false);
	}

	// Calculate the line of sight via selection statements that
	// catch and calculate the direction based on all possible circumstances
	private Vector2 CalcSightDir ()
	{
		Vector2 dir;	// Direction returned by the method

		// Default direction in case selections don't calculate it
		dir = Vector2.right;

		// If sight type is fixed, default it
		if (sight == SightType.Fixed) {
			dir = defaultSightDir;
		}
		// Otherwise, if sight type is towards waypoint 
		// and waypoints have been specified,
		// calculate direction based on its position
		else if (waypoints.Count > 0) {
			Node lookTo;	// Node of the current waypoint
			lookTo = waypoints [currentWaypoint].Waypoint;

			// If we aren't already at the node we want to look towards...
			if (currentNode != lookTo) {
				//...calcuate direction based on distance between this position
				// and look to position
				dir = (Vector2)(lookTo.transform.position - transform.position);
			}
			// Otherwise, if we are already at the node we want to look at,
			// simply keep direction of the current sight direction
			else {
				dir = currentSightDir;
			}
		} else {
			Debug.LogError ("From " + gameObject.name + ": You've set 'sight' to 'SightType.ToWaypoint' without specifying any waypoints!");
		}

		return dir;
	}

	// Stop patrolling and smoothly moving
	protected override void Stop ()
	{
		base.Stop ();
		CancelInvoke ();
		StopCoroutine ("Patrol");
	}
}

// Enumerated type for line of sight of enemies.
// Determines if it is fixed or if it faces 
// towards the direction of motion
public enum SightType
{
	Fixed,
	ToWaypoint
}
