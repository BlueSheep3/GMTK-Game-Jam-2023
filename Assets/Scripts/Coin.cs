using UnityEngine;

class Coin : MonoBehaviour, IRetryable
{
	// self refs
	public SpriteRenderer sr;
	public Rigidbody2D rb;

	// fields
	Vector3 startPos;
	float alpha = 1;


	void Start() {
		if(Savedata.savefile.collectedCoins[Player.inst.currentLevelId - 1])
			sr.color = new Color(1/3f, 1/3f, 2/3f, 1f);
		startPos = transform.position;
	}

	void FixedUpdate() {
		if(alpha < 1) {
			if(alpha <= 0) return;
			alpha -= 0.04f;
			Color color = sr.color;
			color.a = alpha;
			sr.color = color;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.tag != "Player" || sr.color.a != 1)
			return;
		rb.velocity = new Vector2(0, 0.4f);
		alpha -= 0.04f;
	}

	public void Retry() {
		transform.position = startPos;
		sr.color = Color.white;
		rb.velocity = Vector2.zero;
	}
}