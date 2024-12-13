using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour {

	public GameObject effector;
	public string message;

	void OnCollisionEnter2D(Collision2D hit) {
		if (effector == null || message == null) { return; }
		if (hit.gameObject.CompareTag("Hurtbox")) { return; }
		effector.SendMessage(message, hit);
	}

}
