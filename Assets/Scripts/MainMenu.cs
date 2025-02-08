using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	private GameObject controlScrn;
	private GameObject creditScrn;

	private bool controlsOpen = false;
	private bool creditsOpen = false;

	private void Start() {
		// Get credits and controls frames
		controlScrn = transform.GetChild(5).gameObject;
		creditScrn = transform.GetChild(6).gameObject;
	}

	public void OnPlayBtnClicked() {
		SceneManager.LoadScene("Tutorial");
	}

	public void OnControlsBtnClicked() {
		// Toggle visibility of control frame
		if (controlsOpen) {
			controlsOpen = false;
			controlScrn.transform.position = new Vector3(-1000, 0, 0) + transform.position;
		} else {
			controlsOpen = true;
			controlScrn.transform.position = new Vector3(-400, 0, 0) + transform.position;
		}
	}

	public void OnCreditsBtnClicked() {
		// Toggle visibility of credits frame
		if (creditsOpen) {
			creditsOpen = false;
			creditScrn.transform.position = new Vector3(1000, 0, 0) + transform.position;
		} else {
			creditsOpen = true;
			creditScrn.transform.position = new Vector3(400, 0, 0) + transform.position;
		}
	}

}
