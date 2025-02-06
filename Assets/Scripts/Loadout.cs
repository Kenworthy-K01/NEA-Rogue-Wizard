using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadout : MonoBehaviour {

	public string[] spells;

	private float[] cooldowns = new float[] {0f, 0f, 0f, 0f};

	private void Update() {
		for (int i = 0; i < cooldowns.Length; i++) {
			if (cooldowns[i] == 0) { continue; }
			cooldowns[i] = cooldowns[i] - Time.deltaTime;
			if (cooldowns[i] <= 0) {
				cooldowns[i] = 0;
			}
		}
	}

	public string GetSpellIdFromSlot(int slot) {
		return spells[slot];
	}

	public void SpellStartCooldown(int slot, float cooldown) {
		cooldowns[slot] = cooldown;
	}

	public float GetSpellCooldown(int slot) {
		return cooldowns[slot];
	}
}
