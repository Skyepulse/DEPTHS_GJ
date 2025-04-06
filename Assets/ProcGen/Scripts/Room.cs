using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [System.Serializable]
    public class Door
    {
        public Vector3Int position;
        public MapGenerator.Direction direction;
        public Room room;
        public Tunnel tunnel;
    }

    public Door[] doors;

    public GameObject GetPrefab()
    {
        return gameObject;
    }

    public List<Door> GetDoorPositions(MapGenerator.Direction direction, bool invert = false)
    {
        List<Door> doorsCandidates = new();

        foreach (var door in doors)
        {
            if (invert ? door.direction != direction : door.direction == direction)
            {
                doorsCandidates.Add(door);
            }
        }

        return doorsCandidates;
    }

    public List<Door> GetDoors()
    {
        List<Door> doorsCandidates = new();

        foreach (var door in doors)
        {
            doorsCandidates.Add(door);
        }

        return doorsCandidates;
    }

    private BoundsInt GetBounds(Tilemap tilemap)
    {

        BoundsInt globalBounds = tilemap.cellBounds;
        Vector2Int min = new(int.MaxValue, int.MaxValue);
        Vector2Int max = new(int.MinValue, int.MinValue);

        for (int x = globalBounds.x; x < globalBounds.xMax; x++)
        {
            for (int y = globalBounds.y; y < globalBounds.yMax; y++)
            {
                Vector3Int localPosition = new(x, y, 0);
                TileBase tile = tilemap.GetTile(localPosition);
                if (tile != null)
                {
                    min.x = Mathf.Min(min.x, x);
                    min.y = Mathf.Min(min.y, y);
                    max.x = Mathf.Max(max.x, x);
                    max.y = Mathf.Max(max.y, y);
                }
            }
        }

        return new BoundsInt(new Vector3Int(min.x, min.y, 0), new Vector3Int(max.x - min.x + 1, max.y - min.y + 1, 1));
    }

    private BoundsInt mergeBounds(BoundsInt bounds1, BoundsInt bounds2)
    {
        Vector3Int min = new(Mathf.Min(bounds1.x, bounds2.x), Mathf.Min(bounds1.y, bounds2.y), 0);
        Vector3Int max = new(Mathf.Max(bounds1.xMax, bounds2.xMax), Mathf.Max(bounds1.yMax, bounds2.yMax), 1);
        return new BoundsInt(min, max - min);
    }

    // Tilemaps are children of the room prefab
    public BoundsInt GetBounds()
    {
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
        if (tilemaps.Length == 0)
        {
            Debug.LogWarning("No Tilemap found in the room prefab: " + gameObject.name);
            return new BoundsInt(Vector3Int.zero, Vector3Int.zero);
        }

        BoundsInt bounds = GetBounds(tilemaps[0]);
        for (int i = 1; i < tilemaps.Length; i++)
        {
            BoundsInt tilemapBounds = GetBounds(tilemaps[i]);
            bounds = mergeBounds(bounds, tilemapBounds);
        }

        return bounds;
    }

    public void OnDrawGizmosSelected()
    {
        foreach (Door door in doors)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + (Vector3)door.position + Vector3.one * 0.5f, 0.1f);

            Gizmos.color = Color.green;
            Vector3 direction = door.direction switch
            {
                MapGenerator.Direction.North => Vector3.up,
                MapGenerator.Direction.East => Vector3.right,
                MapGenerator.Direction.South => Vector3.down,
                MapGenerator.Direction.West => Vector3.left,
                _ => Vector3.zero,
            };
            Gizmos.DrawLine(transform.position + (Vector3)door.position + Vector3.one * 0.5f, transform.position + (Vector3)door.position + Vector3.one * 0.5f + direction);
        }
        // Show bounds
        Gizmos.color = Color.blue;
        BoundsInt bounds = GetBounds();
        Gizmos.DrawWireCube(transform.position + (Vector3)bounds.position + new Vector3(bounds.size.x, bounds.size.y) * 0.5f, new Vector3(bounds.size.x, bounds.size.y, 1));
    }

}
