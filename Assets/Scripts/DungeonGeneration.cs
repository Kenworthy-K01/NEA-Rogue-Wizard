using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour {

	public int level = 1;

	void Start () {

		GameObject startRoom = AddLevelRoom("Start", Vector3.zero);
		GameObject nextRoom = AddLevelRoom("CorridorUpDown", new Vector3(0, -10, 0));

	}

	private GameObject AddLevelRoom(string roomId, Vector3 atPosition) {
		string levelId = "Level0" + level;

		GameObject roomOriginal = Resources.Load<GameObject>(levelId + "/" + roomId);
		GameObject roomInstance = Instantiate(roomOriginal, atPosition, Quaternion.identity);

		return roomInstance;
	}
}
