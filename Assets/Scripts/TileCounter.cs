

using UnityEngine;

class TileCounter: MonoBehaviour
{
    public int tileIndex;
	public TilePlacer tilePlacer;

	public void ChooseThis() {
		tilePlacer.GetComponent<TilePlacer>().SetCurrentTile(tileIndex);
	}
}