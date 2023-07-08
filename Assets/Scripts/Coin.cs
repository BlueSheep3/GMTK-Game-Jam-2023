using UnityEngine;
using UnityEngine.SceneManagement;

class Coin : MonoBehaviour
{
	void Start() {
		string levelName = SceneManager.GetActiveScene().name;
		if(levelName.StartsWith("Level")) {
			int levelId = int.Parse(levelName.Substring(5));
			if(Savedata.savefile.collectedCoins[levelId])
				Destroy(gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag != "Player")
			return;
		string levelName = SceneManager.GetActiveScene().name;
		if(levelName.StartsWith("Level")) {
			int levelId = int.Parse(levelName.Substring(5));
			Savedata.savefile.collectedCoins[levelId] = true;
			Destroy(gameObject);
		}
	}
}