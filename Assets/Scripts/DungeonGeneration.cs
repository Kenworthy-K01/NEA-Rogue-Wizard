using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGeneration : MonoBehaviour {

	// path exhaust method:
	// start at centre room (0, 0)
	// move in random directions outward (no doubling back)
	// record coordinates travelled
	// choose room shapes
	
	public int complexity = 4; // integer describing length of paths
	public int level = 1;

	private bool Find<T>(T[] table, T item) {
		foreach (T i in table) {
			if (EqualityComparer<T>.Default.Equals(i, item)) {
				return true;
			}
		}
		return false;
	}
	
	private void Start () {
		// Generate rooms

		AddLevelRoom("Start", Vector3.zero);

		List<Vector2> cells = new List<Vector2>();
		cells.Add(Vector2.zero);

		// Create paths
		for (int i = 0; i < 5; i++) {
			Vector2 currentCell = new Vector2(0, 0);
			Vector2 lastMove = new Vector2(0, 0);
			for (int j = 0; j < complexity; j++) {
				Vector2 moveDirection = GetRandomMove(lastMove);
				lastMove = moveDirection;
				Vector2 newCell = currentCell + moveDirection;
				currentCell = newCell;
				if (!cells.Contains(newCell)) {
					cells.Add(newCell);
				}
			}
		}

		// Exhaust
		foreach (Vector2 cell in cells) {
			string roomId = "Xupdownleftright";
			AddLevelRoom(roomId, cell*12);
		}
	}

	private Vector2 GetRandomMove(Vector2 lastMove) {
		Vector2 newMove = Vector2.zero;
		do {
			int dir = Random.Range(-1, 2);
			int isleftright = Random.Range(1, 3);
			if (isleftright == 1) {
				newMove = Vector2.right*dir;
			} else {
				newMove = Vector2.up*dir;
			}
		} while (newMove == lastMove*-1 && newMove == Vector2.zero);
		Debug.Log(newMove);
		return newMove;
	}

	private GameObject AddLevelRoom(string roomId, Vector3 atPosition) {
		string levelId = "Level0" + level;

		GameObject roomOriginal = Resources.Load<GameObject>(levelId + "/" + roomId);
		if (roomOriginal == null) {
			Debug.Log("Missing room id: " + roomId);
			return null;
		}
		GameObject roomInstance = Instantiate(roomOriginal, atPosition, Quaternion.identity);

		return roomInstance;
	}
}
