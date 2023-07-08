using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

class TilePlacer : MonoBehaviour
{
	public Camera mainCamera;
	public Tilemap tilemap;
	public GameObject canvas;
	public CustomRuleTile[] allTileOptions;
	public int[] tileAmounts;
	public GameObject tileCounter;
	List<TMPro.TextMeshProUGUI> tileCounterCounts = new();
	int currentTile = -1;


	void Start()
    {
		if(allTileOptions.Length != tileAmounts.Length)
			Debug.LogError("Tile options and amounts do not match");
		int counter = 0;
		for(int i = tileAmounts.Length - 1; i >= 0; i--)
			if(tileAmounts[i] > 0) {
				currentTile = i;
				counter++;
			}
		if(currentTile == -1)
			Debug.LogError("No tiles in the list");
		int counter2 = 0;
		for(int i = 0; i < tileAmounts.Length; i++) {
			if(tileAmounts[i] > 0) {
				Transform tileCounterObj = Instantiate(tileCounter, canvas.transform).transform;
				tileCounterObj.localPosition += new Vector3((counter2++ - counter/2f) * tileCounter.GetComponent<RectTransform>().rect.width, -250, 0);
				tileCounterObj.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = tileAmounts[i].ToString();
				tileCounterObj.GetChild(1).GetComponent<Image>().sprite = allTileOptions[i].m_DefaultSprite;
				tileCounterCounts.Add(tileCounterObj.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>());
			}
		}
	}
    

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            Vector3Int cellPos = tilemap.WorldToCell(worldPos);

            Debug.Log("Grid position: " + cellPos);
			if(CanPlaceTile(currentTile, cellPos))
				PlaceTile(currentTile, cellPos);
        }
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

	void SetCurrentTile(int index) {
		int p = -1;
		for(int i = 0; i < tileAmounts.Length; i++) {
			if(tileAmounts[i] > 0) {
				p++;
			}
			if(p == index) {
				currentTile = i;
				return;
			}
		}
	}

	void PlaceTile(int index, Vector3Int position)
	{
		tilemap.SetTile(position, allTileOptions[index]);
		SubtractFromTileCounter(index);
	}

	void SubtractFromTileCounter(int index) {
		int counter = 0;
		for(int i = 0; i < tileAmounts.Length; i++) {
			if(tileAmounts[i] > 0) {
				if(i == index) {
					tileAmounts[index]--;
					tileCounterCounts[counter].text = tileAmounts[i].ToString();
					if(tileAmounts[index] == 0) {
						tileCounterCounts.RemoveAt(counter);
						currentTile = GetFirstTileIndex();
					}
					return;
				}
				counter++;
			}
		}
	}
	
	int GetFirstTileIndex() {
		for(int i = 0; i < tileAmounts.Length; i++) {
			if(tileAmounts[i] > 0)
				return i;
		}
		return 0;
	}

	bool CanPlaceTile(int index, Vector3Int position) {
		if(tileAmounts[index] <= 0)
			return false;
		if(tilemap.HasTile(position))
			if(tilemap.GetColliderType(position) != Tile.ColliderType.None)
				return false;
		return true;
	}
}

