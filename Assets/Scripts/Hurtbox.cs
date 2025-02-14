using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour {

	public string canHitTag = "Player";

	private List<GameObject> colliding = new List<GameObject>();

	void OnTriggerEnter2D(Collider2D hit) {
		if (hit.gameObject.CompareTag("Hurtbox")) { return; }
		if (!hit.gameObject.CompareTag(canHitTag)) { return; }

		colliding.Add(hit.gameObject);
	}

	void OnTriggerExit2D(Collider2D hit) {
		if (hit.gameObject.CompareTag("Hurtbox")) { return; }
		if (!hit.gameObject.CompareTag(canHitTag)) { return; }

		colliding.Remove(hit.gameObject);
	}

	public void FlushCollidingList() {
		colliding.Clear();
	}

	public List<GameObject> GetObjectsInBoxBounds() {
		return colliding;
	}

}
