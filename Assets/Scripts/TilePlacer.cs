using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

class TilePlacer : MonoBehaviour
{
	public Camera mainCamera;
	public CustomRuleTile[] allTileOptions;
	public Tilemap tilemap;
	List<int> tileAmounts = new();
	int currentTile = -1;


	void Start()
    {
		for(int i = 0; i < tileAmounts.Count; i++) {
			if(tileAmounts[i] >= 0) {
				currentTile = i;
				break;
			}
		}
		if(currentTile == -1)
			Debug.LogError("No tiles in the list");
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

