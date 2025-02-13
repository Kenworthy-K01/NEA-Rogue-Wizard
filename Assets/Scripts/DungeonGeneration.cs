using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DungeonGeneration : MonoBehaviour {

	// path exhaust method:
	// start at centre room (0, 0)
	// move in random directions outward (no doubling back)
	// record coordinates travelled
	// choose room shapes
	
	public int complexity = 4; // integer describing length of paths
	public int level = 1;
	public string[] enemies;

	private List<GameObject> enemyObjects = new List<GameObject>();
	private GameObject player;

	public int levelStartEnemies = 0;
	private int levelClearedFrame = 0;

	private void Start () {
		// Generate rooms
		if (level == 0 || complexity == 0) { return; }

		AddLevelRoom("Start", Vector3.zero);

		// Container of all rooms
		List<Vector2> cells = new List<Vector2>();
		cells.Add(Vector2.zero);

		// Cells next to start room must be added
		cells.Add(Vector2.right);
		cells.Add(Vector2.left);
		cells.Add(Vector2.up);
		cells.Add(Vector2.down);

		// Create paths
		for (int i = 0; i < 5; i++) {
			// Each path begins at 0, 0
			Vector2 currentCell = new Vector2(0, 0);
			Vector2 lastMove = new Vector2(0, 0);

			// Each path is [complexity] rooms long
			for (int j = 0; j < complexity; j++) {
				Vector2 moveDirection = GetRandomMove(lastMove);
				lastMove = moveDirection;
				Vector2 newCell = currentCell + moveDirection;
				currentCell = newCell;

				// Don't add a cell that already exists
				if (!cells.Contains(newCell)) {
					cells.Add(newCell);
				}
			}
		}

		// Exhaust
		player = GameObject.FindGameObjectWithTag("Player");
		Inventory plrInv = player.GetComponent<Inventory>();
		Relic curse = plrInv.GetEquippedCurse();
		int maxEnemyPerRoom = 5;

		if (curse != null && curse.relicId == "Marked Skull") {
			maxEnemyPerRoom = 10;
		}

		foreach (Vector2 cell in cells) {
			if (cell == Vector2.zero) { continue; }
			string roomId = GetRoomShape(cell, cells);
			GameObject room = AddLevelRoom(roomId, cell*12);
			if (Random.Range(1, 100) <= 40) {
				PopulateRoom(room, Random.Range(1, maxEnemyPerRoom));
			}
		}

		// Initially count the total number of enemies
		levelStartEnemies = CountRemainingEnemies();
	}

	private void FixedUpdate() {
		int now = Time.frameCount;
		if (levelClearedFrame != 0 && now - levelClearedFrame > 360) {
			string nextLevelId = "DungeonLevel"+ (level+1);

			// Check if the next level exists
			int buildIndex = SceneUtility.GetBuildIndexByScenePath(nextLevelId);
			if (buildIndex == -1) {
				SceneManager.LoadScene("LegendaryWizardScene");
				return;
			}

			// REFERENCE: SceneManager.MoveGameObjectToScene
			TravelToScene(nextLevelId);
		}
	}

	// REFERENCE: SceneManager.MoveGameObjectToScene
	private void TravelToScene(string sceneId) {
		/*Scene currentScene = SceneManager.GetActiveScene();

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);

		while (!asyncLoad.isDone) {
			yield return null;
		}

		SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(sceneId));
		SceneManager.UnloadSceneAsync(currentScene);*/

		SceneManager.LoadScene(sceneId);
	}
	// END REFERENCE

	private string GetRoomShape(Vector2 cell, List<Vector2> cellList) {
		string roomShape = "U";
		int exitNums = 0;
		List<string> exitDirs = new List<string>();

		// Get adjacent cell coordinates
		Vector2 leftCell = cell + Vector2.left;
		Vector2 rightCell = cell + Vector2.right;
		Vector2 upCell = cell + Vector2.up;
		Vector2 downCell = cell + Vector2.down;

		// Check if these cells exist
		if (cellList.Contains(upCell)) {
			exitNums += 1;
			exitDirs.Add("up");
		}
		if (cellList.Contains(downCell)) {
			exitNums += 1;
			exitDirs.Add("down");
		}
		if (cellList.Contains(leftCell)) {
			exitNums += 1;
			exitDirs.Add("left");
		}
		if (cellList.Contains(rightCell)) {
			exitNums += 1;
			exitDirs.Add("right");
		}

		// Find correct room shape
		switch (exitNums) {
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

		// Add direction of adjacent rooms
		string exitString = "";
		foreach (string exit in exitDirs) {
			exitString += exit;
		}

		return roomShape + exitString;
	}

	public void OnLevelCleared() {
		int now = Time.frameCount;
		levelClearedFrame = now;
	}

	private Vector2 GetRandomMove(Vector2 lastMove) {
		// Get a random direction that is not the opposite of lastMove (backtrack prevention)
		// Also cannot move by 0, 0
		Vector2 newMove = Vector2.zero;
		do {
			int dir = Random.Range(-1, 2);
			int isleftright = Random.Range(1, 3);
			if (isleftright == 1) {
				newMove = Vector2.right*dir;
			} else {
				newMove = Vector2.up*dir;
			}
		} while (newMove == lastMove*-1 || newMove == Vector2.zero);

		return newMove;
	}

	private void PopulateRoom(GameObject room, int num) {
		for (int i = 1; i <= num; i++) {
			string enemyId = enemies[Random.Range(0, enemies.Length)];
			GameObject entityOriginal = Resources.Load<GameObject>("Entities/" + enemyId);
			Vector3 atPosition = room.transform.Find("SpawnPoint" + Random.Range(1, 3)).position + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);
			GameObject entityInstance = Instantiate(entityOriginal, atPosition, Quaternion.identity);
			AddEnemy(entityInstance);
		}
	}

	public int CountRemainingEnemies() {
		return enemyObjects.Count;
	}

	public void EnemyKilled(GameObject enemy) {
		if (!enemyObjects.Contains(enemy)) { return; } // Enemy not in table

		enemyObjects.Remove(enemy);
	}

	public void AddEnemy(GameObject enemy) {
		enemyObjects.Add(enemy);
		levelStartEnemies += 1;
	}

	private GameObject AddLevelRoom(string roomId, Vector3 atPosition) {
		string levelId = "Level0" + level;

		// Load room prefab from resources folder
		GameObject roomOriginal = Resources.Load<GameObject>(levelId + "/" + roomId);
		if (roomOriginal == null) {
			Debug.Log("Missing room id: " + roomId);
			return null;
		}
		GameObject roomInstance = Instantiate(roomOriginal, atPosition, Quaternion.identity);

		return roomInstance;
	}
}
