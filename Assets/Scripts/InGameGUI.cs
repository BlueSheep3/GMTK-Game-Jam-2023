using UnityEngine;
using UnityEngine.SceneManagement;

class InGameGUI : MonoBehaviour
{
	// self refs
	public GameObject whileBuildingButtons;
	public GameObject whilePlayingButtons;

	void Update() {
		if(Input.GetKeyDown(KeyCode.Space)) {
			if(Player.inst.isPlaying) {
				Player.inst.ToggleSpeedUp();
			} else {
				ClickedPlay();
			}
		}
		if(Input.GetKeyDown(KeyCode.R) && Player.inst.isPlaying) {
			ClickedRetry();
		}
	}

	public void BackToMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}

	// NOTE also used by hotkey
	public void ClickedPlay() {
		Player.inst.StartPlayback();
		whileBuildingButtons.SetActive(false);
		whilePlayingButtons.SetActive(true);
	}

	public void ClickedSpeedUp() {
		Player.inst.ToggleSpeedUp();
	}

	// NOTE also used by hotkey
	public void ClickedRetry() {
		foreach(GameObject o in GameObject.FindObjectsOfType<GameObject>()) {
			if(o.TryGetComponent<IRetryable>(out var r))
				r.Retry();
		}
	}
}