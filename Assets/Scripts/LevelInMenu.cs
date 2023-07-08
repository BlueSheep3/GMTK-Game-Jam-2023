using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

class LevelInMenu : MonoBehaviour
{
	// self refs
	public Image image;

	// fields
	public int levelId;

	void Start() {
		if(levelId > Savedata.savefile.maxLevelCompleted + 1) {
			image.color = Color.gray;
		}
	}

	public void Clicked() {
		if(levelId > Savedata.savefile.maxLevelCompleted + 1) return;
		SceneManager.LoadScene(levelId);
	}
}