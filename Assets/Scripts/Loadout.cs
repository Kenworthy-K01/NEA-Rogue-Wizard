using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadout : MonoBehaviour {

	// Equipped spell Ids
	public string[] spells;
	private float[] cooldowns = new float[] {0f, 0f, 0f, 0f};

	// Reduce cooldowns by deltatime each frame
	private void Update() {
		for (int i = 0; i < cooldowns.Length; i++) {
			if (cooldowns[i] == 0) { continue; }
			cooldowns[i] = cooldowns[i] - Time.deltaTime;
			if (cooldowns[i] <= 0) {
				cooldowns[i] = 0;
			}
		}
	}

	// Return equipped spell in given slot
	public string GetSpellIdFromSlot(int slot) {
		return spells[slot];
	}

	// Set cooldown period
	public void SpellStartCooldown(int slot, float cooldown) {
		cooldowns[slot] = cooldown;
	}

	// Return time left on cooldown
	public float GetSpellCooldown(int slot) {
		return cooldowns[slot];
	}
}
