using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

class TilePlacer : MonoBehaviour
{
	public Tilemap tilemap;
	public Tilemap tileHasBeenPlacedHere;
	public Tilemap previewLayer;
	public GameObject canvas;
	public GameObject tileCounter;
	public CustomRuleTile InvisibleTile;
	public CustomRuleTile[] allTileOptions;
	public int[] tileAmounts;
	public GameObject eraser;
	List<TMPro.TextMeshProUGUI> tileCounterCounts = new();
	int currentTile = -1;
	GameObject eraserInstance;


	void Start()
    {
		eraserInstance = Instantiate(eraser, canvas.transform);
		if(allTileOptions.Length != tileAmounts.Length)
			Debug.LogError("Tile options and amounts do not match");
		int counter = 0;
		for(int i = 0; i < tileAmounts.Length; i++)
			if(tileAmounts[i] >= 0)
				counter++;
		currentTile = GetFirstValidTileIndex();
		if(currentTile == -1)
			Debug.LogError("No tiles in the list");
		int counter2 = 0;
		for(int i = 0; i < tileAmounts.Length; i++) {
			if(tileAmounts[i] >= 0) {
				Transform tileCounterObj = Instantiate(tileCounter, canvas.transform).transform;
				tileCounterObj.GetComponent<TileCounter>().tilePlacer = this;
				tileCounterObj.GetComponent<TileCounter>().tileIndex = counter2;
				tileCounterObj.localPosition += new Vector3((counter2++ - counter/2f - 0.5f) * tileCounter.GetComponent<RectTransform>().rect.width, -250, 0);
				tileCounterObj.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = tileAmounts[i].ToString();
				tileCounterObj.GetChild(1).GetComponent<Image>().sprite = allTileOptions[i].m_DefaultSprite;
				tileCounterCounts.Add(tileCounterObj.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>());
			}
		}
		Transform eraserObj = Instantiate(tileCounter, canvas.transform).transform;
		eraserObj.GetComponent<TileCounter>().tilePlacer = this;
		eraserObj.GetComponent<TileCounter>().tileIndex = counter2;
		eraserObj.localPosition += new Vector3((counter2++ - counter/2f - 0.5f) * tileCounter.GetComponent<RectTransform>().rect.width, -250, 0);
		eraserObj.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = "";
		eraserObj.GetChild(1).GetComponent<Image>().sprite = eraser.GetComponent<SpriteRenderer>().sprite;
	}
    

    void Update()
    {
		if(Player.inst.isPlaying) {
			previewLayer.ClearAllTiles();
			Cursor.visible = true;
			eraserInstance.transform.position = new Vector3(0, -1000, 0);
			return;
		}
		Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        if (Input.GetMouseButtonDown(0))
			if(CanPlaceTile(currentTile, cellPos))
				PlaceTile(currentTile, cellPos);
		previewTiles(cellPos);
		if(Input.GetKeyDown(KeyCode.Alpha1))
			SetCurrentTile(0);
		if(Input.GetKeyDown(KeyCode.Alpha2))
			SetCurrentTile(1);
		if(Input.GetKeyDown(KeyCode.Alpha3))
			SetCurrentTile(2);
		if(Input.GetKeyDown(KeyCode.Alpha4))
			SetCurrentTile(3);
		if(Input.GetKeyDown(KeyCode.Alpha5))
			SetCurrentTile(4);
		if(Input.GetKeyDown(KeyCode.Alpha6))
			SetCurrentTile(5);
		if(Input.GetKeyDown(KeyCode.Alpha7))
			SetCurrentTile(6);
		if(Input.GetKeyDown(KeyCode.Alpha8))
			SetCurrentTile(7);
		if(Input.GetKeyDown(KeyCode.Alpha9))
			SetCurrentTile(8);
    }

	void previewTiles(Vector3Int position) {
		previewLayer.ClearAllTiles();

		eraserInstance.transform.position = new Vector3(0, -1000, 0);
		if(currentTile == tileAmounts.Length) {
			Vector3 mousePos = Input.mousePosition;
        	Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
			Vector3 newPos = new Vector3(worldPos.x + 0.3f, worldPos.y + 0.35f, 0);
			eraserInstance.transform.position = newPos;
			Cursor.visible = false;
			return;
		}
		Cursor.visible = true;

		if(!CanPlaceTile(currentTile, position))
			return;
		
		for(int i = -1; i <= 1; i++) {
			for(int j = -1; j <= 1; j++) {
				Vector3Int localPlace = position + new Vector3Int(i, j, 0);
				if (tilemap.HasTile(localPlace))
					previewLayer.SetTile(localPlace, tilemap.GetTile(localPlace));
			}
		}

		previewLayer.SetTile(position, allTileOptions[currentTile]);
	}

	bool pointingAtUIElement() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	public void SetCurrentTile(int index) {
		int p = -1;
		for(int i = 0; i < tileAmounts.Length; i++) {
			if(tileAmounts[i] >= 0)
				p++;
			if(p == index) {
				currentTile = i;
				return;
			}
		}
		if(index == tileCounterCounts.Count) {
			currentTile = tileAmounts.Length;
		}
	}

	void PlaceTile(int index, Vector3Int position)
	{
		if(index == tileAmounts.Length) {
			AddToTileCounter(tilemap.GetTile(position));
			tilemap.SetTile(position, null);
			tileHasBeenPlacedHere.SetTile(position, null);
			return;
		}
		tilemap.SetTile(position, allTileOptions[index]);
		tileHasBeenPlacedHere.SetTile(position, InvisibleTile);
		SubtractFromTileCounter(index);
	}

	void AddToTileCounter(TileBase tile) {
		int counter = 0;
		for(int i = 0; i < tileAmounts.Length; i++) {
			if(tileAmounts[i] >= 0) {
				if(tile == allTileOptions[i]) {
					tileAmounts[i]++;
					tileCounterCounts[counter].text = tileAmounts[i].ToString();
					return;
				}
				counter++;
			}
		}
	}

	void SubtractFromTileCounter(int index) {
		int counter = 0;
		for(int i = 0; i < tileAmounts.Length; i++) {
			if(tileAmounts[i] >= 0) {
				if(i == index) {
					tileAmounts[index]--;
					tileCounterCounts[counter].text = tileAmounts[i].ToString();
					if(tileAmounts[index] == 0)
						currentTile = GetFirstValidTileIndex();
					return;
				}
				counter++;
			}
		}
	}
	
	int GetFirstValidTileIndex() {
		for(int i = 0; i < tileAmounts.Length; i++) {
			if(tileAmounts[i] > 0)
				return i;
		}
		return tileAmounts.Length;
	}

	bool CanPlaceTile(int index, Vector3Int position) {
		if(Player.inst.transform.position == position)
			return false;
		if(index == tileAmounts.Length)
			return tileHasBeenPlacedHere.HasTile(position);
		if(tileAmounts[index] <= 0)
			return false;
		if(tileHasBeenPlacedHere.HasTile(position))
			return false;
		if(tilemap.HasTile(position))
			if(tilemap.GetColliderType(position) != Tile.ColliderType.None)
				return false;
		if(pointingAtUIElement())
			return false;
		return true;
	}
}

