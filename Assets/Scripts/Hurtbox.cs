using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour {

	// Can hit objects with this tag
	public string canHitTag = "Player";

	// List of current objects within bounds
	private List<GameObject> colliding = new List<GameObject>();

	// Add hit object to colliding list
	void OnTriggerEnter2D(Collider2D hit) {
		if (hit.gameObject.CompareTag("Hurtbox")) { return; }
		if (!hit.gameObject.CompareTag(canHitTag)) { return; }

		colliding.Add(hit.gameObject);
	}
	
	// Remove hit object from colliding list
	void OnTriggerExit2D(Collider2D hit) {
		if (hit.gameObject.CompareTag("Hurtbox")) { return; }
		if (!hit.gameObject.CompareTag(canHitTag)) { return; }

		colliding.Remove(hit.gameObject);
	}

	// Reset colliding list to empty
	public void FlushCollidingList() {
		colliding.Clear();
	}

	// Get objects that are currently within bounds
	public List<GameObject> GetObjectsInBoxBounds() {
		return colliding;
	}

}
