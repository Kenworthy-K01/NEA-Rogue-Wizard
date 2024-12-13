using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadout : MonoBehaviour {

	public string spellId1 = "";
	public string spellId2 = "";
	public string spellId3 = "";
	public string spellId4 = "";

	public string GetSpellIdFromSlot(int slot) {
		switch (slot) {
			case 1:
				return spellId1;
			case 2:
				return spellId2;
			case 3:
				return spellId3;
			case 4:
				return spellId4;
			default:
				return spellId1;
		}
	}

}
