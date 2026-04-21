using UnityEngine;
using UnityEngine.Tilemaps;

public class GridClickHandler : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap highlightTileMap;
    public TileBase highlightTile;
    private Vector3Int previousPos;
    private bool hasPrevious = false;

    void HighlightCell(Vector3Int pos)
    {
        if (hasPrevious)
        {
            highlightTileMap.SetTile(previousPos, null);
        }
        if (tilemap.HasTile(pos))
        highlightTileMap.SetTile(pos, highlightTile);

        previousPos = pos;
        hasPrevious = true;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

            Debug.Log("Clicked Cell: " + cellPos);

            // Kiểm tra có tile không
            if (tilemap.HasTile(cellPos))
            {
                Debug.Log("Có tile tại: " + cellPos);
            }
            else
            {
                Debug.Log("Không có tile!");
            }
            HighlightCell(cellPos);
        }
    }
}

