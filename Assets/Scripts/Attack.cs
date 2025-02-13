using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScaleType { None, Strength, Fortitude, Agility, Intelligence, Charisma };
public enum StatusEffect { None, Burn, Poison, Shock, Chill }

public class Attack : MonoBehaviour {

	public int slot = 0;
	public float baseDamage = 0;
	public float scaling = 0;
	public float armorPenetration = 0;
	public float cooldown = 0.5f;
	public StatusEffect statusEffect = StatusEffect.None;

	public AudioClip soundEffect;

	public ScaleType scaleType = ScaleType.None;

	private Dictionary<GameObject, int> targetLastHit = new Dictionary<GameObject, int>();

	private void Start() {
		if (soundEffect != null) {
			AudioSource.PlayClipAtPoint(soundEffect, transform.position + (Vector3.back * 8), 2f);
		}
	}
	
	public int CalculateDamage(GameObject target) {
		Attributes attributes = target.GetComponentInChildren<Attributes>();
		if (attributes == null) { return 0; }

		float damage = baseDamage;

		Inventory inv = target.GetComponentInChildren<Inventory>();
		if (inv != null) {
			Relic curse = inv.GetEquippedCurse();
			if (curse != null && curse.relicId == "Blood Dagger") {
				damage *= 1.2f;
			}
		}

		return (int)baseDamage;
	}

	public bool HasHitTarget(GameObject target) {
		// Target hasn't been hit at all
		if (!targetLastHit.ContainsKey(target)) { return false; }

		return true;
	}

	public void HitTarget(GameObject target) {
		target.SendMessage("HitStun", statusEffect);
		if (targetLastHit.ContainsKey(target)) {
			targetLastHit[target] = Time.frameCount;
		} else {
			targetLastHit.Add(target, Time.frameCount);
		}
	}
}
