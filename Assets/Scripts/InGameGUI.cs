using UnityEngine;
using UnityEngine.SceneManagement;

class InGameGUI : MonoBehaviour
{
	public void BackToMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}
}