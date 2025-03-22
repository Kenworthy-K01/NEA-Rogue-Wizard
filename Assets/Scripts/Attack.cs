using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScaleType { None, Strength, Fortitude, Agility, Intelligence, Charisma };
public enum StatusEffect { None, Burn, Poison, Shock, Chill }

public class Attack : MonoBehaviour {

	// Basic Attack properties
	public int slot = 0;
	public float baseDamage = 0;
	public float scaling = 0;
	public float armorPenetration = 0;
	public float cooldown = 0.5f;
	public StatusEffect statusEffect = StatusEffect.None;

	// Object references
	public AudioClip soundEffect;
	public ScaleType scaleType = ScaleType.None;
	private Dictionary<GameObject, int> targetLastHit = new Dictionary<GameObject, int>();

	// Instantly play sound effect when attack created
	private void Start() {
		if (soundEffect != null) {
			AudioSource.PlayClipAtPoint(soundEffect, transform.position + (Vector3.back * 8), 2f);
		}
	}
	
	// Calculate scaled damage based on Attack properties
	public int CalculateDamage(GameObject target) {
		Attributes attributes = target.GetComponentInChildren<Attributes>();
		if (attributes == null) { return 0; }

		float damage = baseDamage;

		// Apply debuff damage multiplier
		Inventory inv = target.GetComponentInChildren<Inventory>();
		if (inv != null) {
			Relic curse = inv.GetEquippedCurse();
			if (curse != null && curse.relicId == "Blood Dagger") {
				damage *= 1.2f;
			}
		}

		return (int)damage;
	}

	// Return true if the attack has already hit this target
	public bool HasHitTarget(GameObject target) {
		// Target hasn't been hit at all
		if (!targetLastHit.ContainsKey(target)) { return false; }

		return true;
	}

	// Register a hit
	public void HitTarget(GameObject target) {
		// Tell the target they got hit
		target.SendMessage("HitStun", statusEffect);
		if (targetLastHit.ContainsKey(target)) {
			targetLastHit[target] = Time.frameCount;
		} else {
			targetLastHit.Add(target, Time.frameCount);
		}
	}
}
