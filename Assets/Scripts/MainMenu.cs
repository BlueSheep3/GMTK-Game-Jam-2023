using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

class MainMenu : MonoBehaviour
{
	// self refs
	public GameObject mainMenu;
	public GameObject levelsMenu;
	public GameObject settingsMenu;
	public GameObject creditsMenu;
	public Slider qualitySlider;
	public Slider volumeSlider;
	public TMP_Text screenSizeText;
	public TMP_Text coinCounterText;

	void Awake() {
		if(Savedata.savefile == null)
			Savedata.Load();

		qualitySlider.value = Savedata.savefile.quality;
		volumeSlider.value = Savedata.savefile.volume;
		SetScreenSizeText();

		coinCounterText.text = Savedata.savefile.coinCount.ToString();

		Cursor.visible = true;
	}

	#region: levels
	public void ClickedLevels() {
		SoundHandler.PlaySound("Click", 1);
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
		SoundHandler.PlaySound("Click", 1);
	}
	#endregion

	#region: settings
	public void ClickedSettings() {
		mainMenu.SetActive(false);
		settingsMenu.SetActive(true);
		SoundHandler.PlaySound("Click", 1);
	}

	public void SettingsBack() {
		mainMenu.SetActive(true);
		settingsMenu.SetActive(false);
		SoundHandler.PlaySound("Click", 1);
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
			float aspectRatio = width / (float)height;

			Debug.Log($"{width} / {height} = {aspectRatio}");

			if(Mathf.Abs(aspectRatio - 1.777777f) < 0.01)
				Screen.SetResolution(width, height, true);
			else if(aspectRatio < 1.777777f)
				Screen.SetResolution(width, (int)(width / 1.777777f), true);
			else
				Screen.SetResolution((int)(height * 1.777777f), height, true);
		} else
			Screen.SetResolution(960 * size, 540 * size, false);
		
		SetScreenSizeText();
		SoundHandler.PlaySound("Click", 1);
	}

	void SetScreenSizeText() {
		if(Savedata.savefile.screenSize == 0)
			screenSizeText.text = "Fullscreen";
		else
			screenSizeText.text = $"Screensize: x{Savedata.savefile.screenSize}";
	}
	#endregion

	#region: credits
	public void ClickedCredits() {
		mainMenu.SetActive(false);
		creditsMenu.SetActive(true);
		SoundHandler.PlaySound("Click", 1);
	}

	public void CreditsBack() {
		mainMenu.SetActive(true);
		creditsMenu.SetActive(false);
		SoundHandler.PlaySound("Click", 1);
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
