using UnityEngine;
using UnityEngine.Tilemaps;

class Platform : MonoBehaviour
{
	public EdgeCollider2D col = null!;
	public Direction direction = 0; // (direction of normal)

	public enum Direction : byte {
		Up, Left, Down, Right
	}

	void Start() {
		Tilemap map = GetComponentInParent<Tilemap>();
		Grid grid = map.layoutGrid;

		Vector3Int cell = grid.WorldToCell(transform.position);
		TileBase tileThis = map.GetTile(cell);
		ExpandCollider(tileThis, cell, map);

		// script is no longer required on the object
		Destroy(this);
	}


	void ExpandCollider(TileBase self, Vector3Int cell, Tilemap map) {
		Vector2Int offsetVec = (int)direction % 2 == 0 ? new(1,0) : new(0,1);

		int length = 1;
		while(self == map.GetTile(cell + length * (Vector3Int)offsetVec))
			length++;

		col.points = new Vector2[2] { col.points[0], col.points[1] + (length - 1.001f) * (Vector2)offsetVec };
	}
}
