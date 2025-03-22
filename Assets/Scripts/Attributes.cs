using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour {
	
	// Attribute values
	public int strength = 0;
	public int fortitude = 0;
	public int agility = 0;
	public int intelligence = 0;
	public int charisma = 0;
	
	// Skill Points
	public int skillPoints = 0;
	
	// Return a value representing total attribute sum
	public int GetPower() {
		int power = 0;

		power += strength;
		power += fortitude;
		power += agility;
		power += intelligence;
		power += charisma;

		return power;
	}

	// Add a skill point
	public void AwardSkillPoints(int num) {
		skillPoints += num;
	}

	// Spend a skill point
	private void SpendSkillPoints(int num) {
		skillPoints -= num;
	}

	// Set new strength value
	public void SetStrength(int newStrength) {
		int delta = newStrength-strength;
		if (delta > 0) { SpendSkillPoints(delta); }

		int power = GetPower() - strength;

		int remainingPower = 200 - power;
		strength = ClampStat(newStrength, remainingPower);
	}

	// Set new strength value
	public void SetFortitude(int newFortitude) {
		int delta = newFortitude-fortitude;
		if (delta > 0) { SpendSkillPoints(delta); }

		int power = GetPower() - fortitude;

		int remainingPower = 200 - power;
		fortitude = ClampStat(newFortitude, remainingPower);
	}

	// Set new strength value
	public void SetAgility(int newAgility) {
		int delta = newAgility-agility;
		if (delta > 0) { SpendSkillPoints(delta); }

		int power = GetPower() - agility;

		int remainingPower = 200 - power;
		agility = ClampStat(newAgility, remainingPower);
	}

	// Set new strength value
	public void SetIntelligence(int newIntelligence) {
		int delta = newIntelligence-intelligence;
		if (delta > 0) { SpendSkillPoints(delta); }

		int power = GetPower() - intelligence;

		int remainingPower = 200 - power;
		intelligence = ClampStat(newIntelligence, remainingPower);
	}

	// Set new strength value
	public void SetCharisma(int newCharisma) {
		int delta = newCharisma-charisma;
		if (delta > 0) { SpendSkillPoints(delta); }

		int power = GetPower() - charisma;

		int remainingPower = 200 - power;
		charisma = ClampStat(newCharisma, remainingPower);
	}

	// Return scaled walk speed with agility stat
	public int CalculateWalkSpeed() {
		Inventory inv = GetComponent<Inventory>();
		float buffMult = 1f;
		if (inv != null) {
			// Apply speed mults from relics
			Relic curse = inv.GetEquippedCurse();
			Relic relic = inv.GetEquippedRelic();
			if (curse != null) {
				switch (curse.relicId) {
					case "Iron Ball":
						buffMult *= 0.5f;
						break;
				}
			}
			if (relic != null) {
				switch(relic.relicId) {
					case "Golden Boots":
						buffMult *= 2f;
						break;
				}
			}
		}
		// Return resultant Walk Speed
		return Mathf.RoundToInt((5+((agility)/25))*buffMult);
	}

	// Return scaled max health with fortitude stat
	public int CalculateMaxHealth() {
		int power = GetPower();
		return Mathf.RoundToInt(100 + (fortitude/4) + (power/2));
	}

	// Helper function to clamp stats between values
	private int ClampStat(int stat, int max) {
		return Mathf.Clamp(stat, 0, max);
	}

}
