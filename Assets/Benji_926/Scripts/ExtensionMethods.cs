using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
	// Returns a vector 2 integer representation of the position of a transform in the x-y plane
	public static Vector2Int IntPos2D (this Transform trans)
	{
		return new Vector2Int (Mathf.RoundToInt (trans.position.x), Mathf.RoundToInt (trans.position.y));
	}
}
