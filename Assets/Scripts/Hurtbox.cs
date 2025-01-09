using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour {

	public GameObject effector;
	public string message;

	void OnCollisionEnter2D(Collision2D hit) {
		Debug.Log("collision enter");
		if (effector == null || message == null) { return; }
		if (hit.gameObject.CompareTag("Hurtbox")) { return; }
		Debug.Log("msg sent");
		effector.SendMessage(message, hit);
	}

}
