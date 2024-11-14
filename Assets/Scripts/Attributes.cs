using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes {
	public int strength { get { return strength; } set { strength = Mathf.Clamp(value, 0, 100); } }
	public int fortitude = 0;
	public int agility = 0;
	public int intelligence = 0;
	public int charisma = 0;

	private int Clamp(string statName, int stat) {
		int total = 0;
		total += this.strength;
		total += this.fortitude;
		total += this.agility;
		total += this.intelligence;
		total += this.charisma;

		return stat;
	}

}
