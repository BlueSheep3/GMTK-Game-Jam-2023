using UnityEngine;
using UnityEngine.SceneManagement;

class Coin : MonoBehaviour
{
	void Start() {
		if(Savedata.savefile.collectedCoins[Player.inst.currentLevelId])
			Destroy(gameObject);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag != "Player")
			return;
		Savedata.savefile.collectedCoins[Player.inst.currentLevelId] = true;
		Destroy(gameObject);
	}
}