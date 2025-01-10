using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScaleType { None, Strength, Fortitude, Agility, Intelligence, Charisma };

public class Attack {

	public float baseDamage = 0;
	public float scaling = 0;
	public float armorPenetration = 0;
	public ScaleType scaleType = ScaleType.None;

	private Dictionary<GameObject, int> targetLastHit = new Dictionary<GameObject, int>();

	public int CalculateDamage(GameObject target) {
		Attributes attributes = target.GetComponentInChildren<Attributes>();
		if (attributes == null) { return 0; }

		// super secret damage formula goes here

		return (int)baseDamage;
	}

	public bool HasHitTargetWithinFrames(GameObject target, int frames) {
		// Target hasn't been hit at all
		if (!targetLastHit.ContainsKey(target)) { return false; }

		int lastHitFrame = targetLastHit[target];
		// Target was hit within this frame
		if (Time.frameCount - lastHitFrame <= frames) {
			return true;
		}
		// Target was hit earlier than this frame
		return false;
	}

	public void HitTarget(GameObject target) {
		if (targetLastHit.ContainsKey(target)) {
			targetLastHit[target] = Time.frameCount;
		} else {
			targetLastHit.Add(target, Time.frameCount);
		}
	}
}
