using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	private int MaxHealth = 100;
	private int CurHealth = 0;

	private Attributes attributes;

	private void Start() {
		attributes = GetComponent<Attributes>();

		MaxHealth = attributes.CalculateMaxHealth();
		CurHealth = MaxHealth;
	}

	public int GetCurrentHealth() {
		return CurHealth;
	}

	public int GetMaxHealth(bool recalculate) {
		if (recalculate) {
			MaxHealth = attributes.CalculateMaxHealth();
		}
		return MaxHealth;
	}

	// returns true amount of damage taken
	public int TakeDamage(int damage) {
		// Cannot use this function for "healing" by taking negative damage
		if (damage <= 0) { return 0; }

		if (damage > CurHealth) {
			int truedamage = CurHealth;
			CurHealth = 0;
			return truedamage;
		} else {
			CurHealth -= damage;
			return damage;
		}
	}

	// Returns true amount of healing
	public int ApplyHealing(int amount) {
		// Cannot use this function for taking damage by negative healing
		if (amount <= 0) { return 0; }

		if (CurHealth + amount > MaxHealth) {
			int trueheal = MaxHealth - CurHealth;
			CurHealth = MaxHealth;
			return trueheal;
		} else {
			CurHealth += amount;
			return amount;
		}
	}
	
	public void HealToMax() {
		ApplyHealing(MaxHealth);
	}

}
