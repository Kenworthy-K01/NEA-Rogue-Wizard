using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGeneration : MonoBehaviour {

	// scatter exhaust method:
	// pick random n points on a 5x5 grid
	// check all points are adjacent to >0 other points
	// if points are solo, add an adjacent point
	// repeat until all points have >0 adjacents
	// create every possible connection between points
	
	public int complexity = 10; // integer 1-20 describing how many scatter points
	public int level = 1;

	private readonly int[] alwaysAdjacent = { 8, 12, 13, 14, 18 };
	
	
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
		GameObject startRoom = AddLevelRoom("Start", Vector3.zero);
		List<int> scatterPoints = new List<int> { 8, 12, 13, 14, 18};
		
		// Scatter
		for (int i = 1; i <= complexity; i++) {
			int x;
			do {
				x = Random.Range(1, 25);
			} while (scatterPoints.Contains(x));
			scatterPoints.Add(x);
		}
		
		// Adjacent Check
		// If a point has no adjacent points, add one.
		for (int i = 0; i < scatterPoints.Count; i++) {
			int point = scatterPoints[i];
			if (Find(alwaysAdjacent, point)) { continue; }

			int[] adjCells = GetAdjacentCellNumbers(point);
			int upPoint = adjCells[0];
			int downPoint = adjCells[1];
			int leftPoint = adjCells[2];
			int rightPoint = adjCells[3];

			if (scatterPoints.Contains(downPoint) || scatterPoints.Contains(upPoint) || scatterPoints.Contains(leftPoint) || scatterPoints.Contains(rightPoint)) { continue; }

			if (upPoint > 0) {
				scatterPoints.Add(upPoint);
			} else if (downPoint > 0) {
				scatterPoints.Add(downPoint);
			} else if (leftPoint > 0) {
				scatterPoints.Add(leftPoint);
			} else if (rightPoint > 0) {
				scatterPoints.Add(rightPoint);
			}
		}

		// Exhaust
		foreach (int i in scatterPoints) {
			if (i == 13) { continue; }
			
			string roomShape = GetRoomShape(scatterPoints, i);
			Vector3 position = new Vector3(12 * (i%6), 12 * (i/6), 0);
			AddLevelRoom(roomShape, position);
		}
	}

	private string GetRoomShape(List<int> scatterPoints, int point) {
		int[] adjCells = GetAdjacentCellNumbers(point);
		int exits = 0;
		List<string> exitDirs = new List<string>();
		
		for (int p = 0; p < adjCells.Length; p++) {
			int cell = adjCells[p];
			if (!scatterPoints.Contains(cell)) { continue; }
			exits += 1;
			switch (p) {
				case 0:
					exitDirs.Add("up");
					break;
				case 1:
					exitDirs.Add("down");
					break;
				case 2:
					exitDirs.Add("left");
					break;
				case 3:
					exitDirs.Add("right");
					break;
			}
		}
		
		string roomShape = "";
		switch (exits) {
			case 1:
				roomShape = "U";
				break;
			case 2:
				if ((exitDirs.Contains("up") && exitDirs.Contains("down")) || (exitDirs.Contains("left") && exitDirs.Contains("right"))) {
					roomShape = "I";
				} else {
					roomShape = "L";
				}
				break;
			case 3:
				roomShape = "T";
				break;
			case 4:
				roomShape = "X";
				break;
		}

		string id = roomShape;
		foreach (string dir in exitDirs) {
			id = id + dir;
		}
		
		return id;
	}

	private int[] GetAdjacentCellNumbers(int cell) {
		int upPoint = cell - 5;
		int downPoint = cell + 5;
		int leftPoint = cell - 1;
		int rightPoint = cell + 1;
		if ((cell + 1) % 5 == 0) {
			leftPoint = -1;
		}
		if (cell % 5 == 0) {
			rightPoint = -1;
		}

		int[] nums = { upPoint, downPoint, leftPoint, rightPoint };
		return nums;
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
