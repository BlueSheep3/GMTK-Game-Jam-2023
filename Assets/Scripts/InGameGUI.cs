using UnityEngine;
using UnityEngine.SceneManagement;

class InGameGUI : MonoBehaviour
{
	// self refs
	public GameObject whileBuildingButtons;
	public GameObject whilePlayingButtons;

	void Update() {
		if(Input.GetKeyDown(KeyCode.Space) && !Player.inst.isPlaying) {
			ClickedPlay();
		}
		if(Input.GetKeyDown(KeyCode.R) && Player.inst.isPlaying) {
			ClickedRetry();
		}
	}

	// NOTE also used by hotkey
	public void ClickedPlay() {
		Player.inst.StartPlayback();
		whileBuildingButtons.SetActive(false);
		whilePlayingButtons.SetActive(true);
	}

	// NOTE also used by hotkey
	public void ClickedRetry() {
		foreach(GameObject o in GameObject.FindObjectsOfType<GameObject>()) {
			if(o.TryGetComponent<IRetryable>(out var r))
				r.Retry();
		}
		whileBuildingButtons.SetActive(true);
		whilePlayingButtons.SetActive(false);
	}

	public void BackToMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}
}