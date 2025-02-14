using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relic : MonoBehaviour {

	public string relicId;
	public bool cursed = false;
	public string description = "A Relic from the Past.";

	private SpriteRenderer image;
	private Sprite icon;
	//private BoxCollider2D hitbox;

	private void Start() {
		//hitbox = GetComponent<BoxCollider2D>();
		image = GetComponent<SpriteRenderer>();
		icon = image.sprite;
	}

	private void OnTriggerEnter2D(Collider2D hit) {
		if (hit.gameObject.CompareTag("Player")) {
			Inventory playerInv = hit.gameObject.GetComponent<Inventory>();
			playerInv.EquipRelic(this);
			DontDestroyOnLoad(this);
			Destroy(image);
		}
	}

	public Sprite GetIconSprite() {
		return icon;
	}

}
