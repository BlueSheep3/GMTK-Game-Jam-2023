using UnityEngine;
using UnityEngine.SceneManagement;

class Coin : MonoBehaviour
{
	void Start() {
		if(Savedata.savefile.collectedCoins[Player.inst.currentLevelId])
			Destroy(gameObject);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag != "Player" || transform.GetComponent<SpriteRenderer>().color != new Color(1,1,1,1))
			return;
		transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0.2f);
		Fade(1);
	}

	void Fade(float transparency) {
		if(transparency < 0)
			return;
		transform.GetComponent<SpriteRenderer>().color = new Color(1,1,1,transparency - 0.1f);
		Invoke("Fade", 0.1f);
	}

	public void SaveCoin() {//TODO: use this at the end of a level for each coin
		if(transform.GetComponent<SpriteRenderer>().color != new Color(1,1,1,1))
			Savedata.savefile.collectedCoins[Player.inst.currentLevelId] = true;
	}

	public static int GetTotalCoins() {//TODO: use this in the UI
		int total = 0;
		foreach(bool coin in Savedata.savefile.collectedCoins)
			if(coin)
				total++;
		return total;
	}
}