using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScaleType { None, Strength, Fortitude, Agility, Intelligence, Charisma };

public class Attack {

	public float baseDamage = 0;
	public float scaling = 0;
	public float armorPenetration = 0;
	public ScaleType scaleType = ScaleType.None;

	public int CalculateDamage(GameObject target) {
		Attributes attributes = target.GetComponentInChildren<Attributes>();
		if (attributes == null) { return 0; }

		// super secret dmaage formula goes here

		return (int)baseDamage;
	}

}
