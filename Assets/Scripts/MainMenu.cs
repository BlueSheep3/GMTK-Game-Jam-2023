using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class MainMenu : MonoBehaviour
{
	// self refs
	public GameObject mainMenu;
	public GameObject levelsMenu;
	public GameObject settingsMenu;
	public GameObject creditsMenu;

	void Awake() {
		if(Savedata.savefile == null)
			Savedata.Load();
	}

	#region: levels
	public void ClickedLevels() {
		if(Savedata.savefile.maxLevelCompleted == 0) {
			SceneManager.LoadScene("Level1");
			return;
		}
		mainMenu.SetActive(false);
		levelsMenu.SetActive(true);
	}

	public void LevelsBack() {
		mainMenu.SetActive(true);
		levelsMenu.SetActive(false);
	}
	#endregion

	#region: settings
	public void ClickedSettings() {
		mainMenu.SetActive(false);
		settingsMenu.SetActive(true);
	}

	public void SettingsBack() {
		mainMenu.SetActive(true);
		settingsMenu.SetActive(false);
	}

	public void ChangedQuality(Slider slider) {
		Savedata.savefile.quality = (int)slider.value;
		QualitySettings.SetQualityLevel((int)slider.value);
	}

	public void ChangedVolume(Slider slider) {
		Savedata.savefile.volume = (int)slider.value;
		AudioListener.volume = slider.value / 100f;
	}

	public void ClickedScreenSize() {
		Savedata.savefile.screenSize = (Savedata.savefile.screenSize + 1) % 4;
		int size = Savedata.savefile.screenSize;
		if(size == 0) {
			int width = Screen.currentResolution.width;
			int height = Screen.currentResolution.height;
			Screen.SetResolution(width, height, true);
		} else
			Screen.SetResolution(640 * size, 480 * size, false);
	}
	#endregion

	#region: credits
	public void ClickedCredits() {
		mainMenu.SetActive(false);
		creditsMenu.SetActive(true);
	}

	public void CreditsBack() {
		mainMenu.SetActive(true);
		creditsMenu.SetActive(false);
	}
	#endregion

	#region: quit
	public void Quit() {
		Savedata.Save();
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.ExitPlaymode();
		#else
			Application.Quit();
		#endif
	}
	#endregion
}
