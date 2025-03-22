using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relic : MonoBehaviour {

	// Relic properties
	public string relicId;
	public bool cursed = false;
	public string description = "A Relic from the Past.";

	// Object references
	private SpriteRenderer image;
	private Sprite icon;
	//private BoxCollider2D hitbox;

	// Get objects on start
	private void Start() {
		//hitbox = GetComponent<BoxCollider2D>();
		image = GetComponent<SpriteRenderer>();
		icon = image.sprite;
	}

	// Pickup relic when player walks into it
	private void OnTriggerEnter2D(Collider2D hit) {
		if (hit.gameObject.CompareTag("Player")) {
			Inventory playerInv = hit.gameObject.GetComponent<Inventory>();
			playerInv.EquipRelic(this);
			DontDestroyOnLoad(this);
			Destroy(image);
		}
	}

	// Return sprite image (for UI)
	public Sprite GetIconSprite() {
		return icon;
	}
}
