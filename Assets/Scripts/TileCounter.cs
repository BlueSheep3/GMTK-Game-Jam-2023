

using UnityEngine;

class TileCounter: MonoBehaviour
{
    public int tileIndex;
	public TilePlacer tilePlacer;

	public void ChooseThis() {
		if(tileIndex == -1)
			return;
		tilePlacer.GetComponent<TilePlacer>().SetCurrentTile(tileIndex);
	}
}