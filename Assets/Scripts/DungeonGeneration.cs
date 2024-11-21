using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour {

	public int level = 1;
	public int complexity = 3;

	// Contains all room ids and types for random selection with the exception of "Start" and "End"
	// U-type: 1 entrance
	// I-type: 2 entrance
	// T-type: 3 entrance
	// X-type: 4 entrance
	private string[] RoomIdByNumber = { "Corridor", "TurnShort", "TurnLong", "DeadSmall", "DeadLarge", "Shop" };
	private string[] roomShapeByNumber = { "I", "L", "L", "U", "U", "U" };

	void Start () {
		// Generate rooms
		// Start is always an X-type
		GameObject startRoom = AddLevelRoom("Start", Vector3.zero);

		string prevRoomType = "X";
		while (true) {
			string roomType = GetNextRoomType(prevRoomType);
			string roomId = GetRandomRoomOfType(roomType);
			prevRoomType = roomType;
		}

	}

	private string GetNextRoomType(string prevType) {

	}

	private string GetRandomRoomOfType(string roomType) {
		string[] rooms = {};
		for (int i = 0; i < RoomIdByNumber.Length; i++) {

		}
	}

	private GameObject AddLevelRoom(string roomId, Vector3 atPosition) {
		string levelId = "Level0" + level;

		GameObject roomOriginal = Resources.Load<GameObject>(levelId + "/" + roomId);
		GameObject roomInstance = Instantiate(roomOriginal, atPosition, Quaternion.identity);

		return roomInstance;
	}
}
