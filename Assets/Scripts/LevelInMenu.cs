using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

class LevelInMenu : MonoBehaviour
{
	// self refs
	public Image image;
	public Button button;

	// fields
	public int levelId;

	void Start() {
		if(levelId > Savedata.savefile.maxLevelCompleted + 1) {
			button.interactable = false;
		}
		if(Savedata.savefile.collectedCoins[levelId - 1])
			image.color = Color.yellow;
	}

	public void Clicked() {
		if(levelId > Savedata.savefile.maxLevelCompleted + 1) return;
		SceneManager.LoadScene(levelId);
		SoundHandler.PlaySound("Click", 1);
	}
}