using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

class TilePlacer : MonoBehaviour
{
	public Camera mainCamera;
	public CustomRuleTile[] allTileOptions;
	public Tilemap tilemap;
	public int[] tileAmounts;
	public GameObject tileCounter;
	List<TMPro.TextMeshProUGUI> tileCounterCounts = new();
	int currentTile = -1;


	void Start()
    {
		if(allTileOptions.Length != tileAmounts.Length)
			Debug.LogError("Tile options and amounts do not match");
		int counter = 0;
		for(int i = tileAmounts.Length; i > 0; i--)
			if(tileAmounts[i] >= 0) {
				currentTile = i;
				counter++;
			}
		if(currentTile == -1)
			Debug.LogError("No tiles in the list");
		for(int i = 0; i < tileAmounts.Length; i++)
			if(tileAmounts[i] > 0) {
				Transform tileCounterObj = Instantiate(tileCounter, new Vector3(10 * counter - 5 * i, 0, 0), Quaternion.identity).transform.GetChild(0);
				tileCounterObj.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = tileAmounts[i].ToString();
				tileCounterObj.GetChild(1).GetComponent<SpriteRenderer>().sprite = allTileOptions[i].m_DefaultSprite;
				tileCounterCounts.Add(tileCounterObj.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>());
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
    }

	void PlaceTile(int index, Vector3Int position)
	{
		tilemap.SetTile(position, allTileOptions[index]);
		tileAmounts[index]--;
		UpdateOneTileCounter();
	}

	void UpdateOneTileCounter() {
		int counter = 0;
		for(int i = 0; i < tileAmounts.Length; i++)
			if(tileAmounts[i] > 0) {
				if(tileAmounts[i].ToString() != tileCounterCounts[counter].text) {
					tileCounterCounts[counter].text = tileAmounts[i].ToString();
					return;
				}
				counter++;
			}
	}

	bool CanPlaceTile(int index, Vector3Int position) {
		if(tileAmounts[index] <= 0)
			return false;
		if(tilemap.HasTile(position))
			return false;
		if(tilemap.GetColliderType(position) == Tile.ColliderType.None)
			return false;
		return true;
	}
}

