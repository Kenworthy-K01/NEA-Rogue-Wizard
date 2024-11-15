using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour {
	public int strength = 0;
	public int fortitude = 0;
	public int agility = 0;
	public int intelligence = 0;
	public int charisma = 0;

	public int GetPower() {
		int power = 0;

		power += strength;
		power += fortitude;
		power += agility;
		power += intelligence;
		power += charisma;

		return power;
	}

	public void SetStrength(int newStrength) {
		int power = GetPower() - strength;

		int remainingPower = 200 - power;
		strength = ClampStat(newStrength, remainingPower);
	}

	public void SetFortitude(int newFortitude) {
		int power = GetPower() - fortitude;

		int remainingPower = 200 - power;
		fortitude = ClampStat(newFortitude, remainingPower);
	}

	public void SetAgility(int newAgility) {
		int power = GetPower() - agility;

		int remainingPower = 200 - power;
		agility = ClampStat(newAgility, remainingPower);
	}

	public void SetIntelligence(int newIntelligence) {
		int power = GetPower() - intelligence;

		int remainingPower = 200 - power;
		intelligence = ClampStat(newIntelligence, remainingPower);
	}

	public void SetCharisma(int newCharisma) {
		int power = GetPower() - charisma;

		int remainingPower = 200 - power;
		charisma = ClampStat(newCharisma, remainingPower);
	}

	public int CalculateWalkSpeed() {
		return Mathf.RoundToInt(2+(agility/25));
	}

	private int ClampStat(int stat, int max) {
		return Mathf.Clamp(stat, 0, max);
	}

}
