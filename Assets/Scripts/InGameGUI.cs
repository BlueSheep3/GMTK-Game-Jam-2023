using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

class InGameGUI : MonoBehaviour
{
	// self refs
	public GameObject whileBuildingButtons;
	public GameObject whilePlayingButtons;
	public GameObject youWinPoppup;
	public TMP_Text youWinNextText;
	public GameObject thxText;

	void Start() {
		if(Player.inst.currentLevelId == 8) {
			youWinNextText.text = "Main Menu";
			youWinNextText.fontSize = 55;
			thxText.SetActive(true);
		}
	}

	void Update() {
		if(Player.inst.hasWon) return;

		if(Input.GetKeyDown(KeyCode.Space)) {
			ClickedPlay();
		}
		if(Input.GetKeyDown(KeyCode.R)) {
			ClickedRetry();
		}
	}

	// NOTE also used by hotkey
	public void ClickedPlay() {
		if(Player.inst.isPlaying || Player.inst.hasWon) return;
		Player.inst.StartPlayback();
		whileBuildingButtons.SetActive(false);
		whilePlayingButtons.SetActive(true);
		SoundHandler.PlaySound("Click", 1);
	}

	// NOTE also used by hotkey
	public void ClickedRetry() {
		if(!Player.inst.isPlaying || Player.inst.hasWon) return;

		foreach(GameObject o in GameObject.FindObjectsOfType<GameObject>()) {
			if(o.TryGetComponent<IRetryable>(out var r))
				r.Retry();
		}
		whileBuildingButtons.SetActive(true);
		whilePlayingButtons.SetActive(false);
		SoundHandler.PlaySound("ClickRetry", 1);
	}

	public void BackToMainMenu() {
		SceneManager.LoadScene("MainMenu");
		SoundHandler.PlaySound("Click", 1);
	}

	public void ClickedWonNext() {
		if(Player.inst.currentLevelId == 8)
			SceneManager.LoadScene("MainMenu");
		else
			SceneManager.LoadScene(Player.inst.currentLevelId + 1);
		SoundHandler.PlaySound("Click", 1);
	}

	public void ClickedWonRetry() {
		Player.inst.hasWon = false;
		youWinPoppup.SetActive(false);
		ClickedRetry();
		SoundHandler.PlaySound("ClickRetry", 1);
	}
}