using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour {
	public int strength = 0;
	public int fortitude = 0;
	public int agility = 0;
	public int intelligence = 0;
	public int charisma = 0;
	public int skillPoints = 0;

	public int GetPower() {
		int power = 0;

		power += strength;
		power += fortitude;
		power += agility;
		power += intelligence;
		power += charisma;

		return power;
	}

	public void AwardSkillPoints(int num) {
		skillPoints += num;
	}

	private void SpendSkillPoints(int num) {
		skillPoints -= num;
	}

	public void SetStrength(int newStrength) {
		int delta = newStrength-strength;
		if (delta > 0) { SpendSkillPoints(delta); }

		int power = GetPower() - strength;

		int remainingPower = 200 - power;
		strength = ClampStat(newStrength, remainingPower);
	}

	public void SetFortitude(int newFortitude) {
		int delta = newFortitude-fortitude;
		if (delta > 0) { SpendSkillPoints(delta); }

		int power = GetPower() - fortitude;

		int remainingPower = 200 - power;
		fortitude = ClampStat(newFortitude, remainingPower);
	}

	public void SetAgility(int newAgility) {
		int delta = newAgility-agility;
		if (delta > 0) { SpendSkillPoints(delta); }

		int power = GetPower() - agility;

		int remainingPower = 200 - power;
		agility = ClampStat(newAgility, remainingPower);
	}

	public void SetIntelligence(int newIntelligence) {
		int delta = newIntelligence-intelligence;
		if (delta > 0) { SpendSkillPoints(delta); }

		int power = GetPower() - intelligence;

		int remainingPower = 200 - power;
		intelligence = ClampStat(newIntelligence, remainingPower);
	}

	public void SetCharisma(int newCharisma) {
		int delta = newCharisma-charisma;
		if (delta > 0) { SpendSkillPoints(delta); }

		int power = GetPower() - charisma;

		int remainingPower = 200 - power;
		charisma = ClampStat(newCharisma, remainingPower);
	}

	public int CalculateWalkSpeed() {
		return Mathf.RoundToInt(5+(agility/25));
	}

	public int CalculateMaxHealth() {
		int power = GetPower();
		return Mathf.RoundToInt(100 + (fortitude/4) + (power/2));
	}

	private int ClampStat(int stat, int max) {
		return Mathf.Clamp(stat, 0, max);
	}

}
