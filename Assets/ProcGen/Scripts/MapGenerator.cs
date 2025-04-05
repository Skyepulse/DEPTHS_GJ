using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject[] roomPrefabs;
    [SerializeField]
    private int numRooms = 5;

    [SerializeField]
    private string doorTileName = "tileset_4";

    private Vector3Int getDoorPosition(Tilemap tilemap, bool reverse = false)
    {
        Vector3Int position = Vector3Int.zero;
        BoundsInt bounds = tilemap.cellBounds;

        for (position.x = reverse ? bounds.xMax : bounds.x; reverse ? position.x >= bounds.x : position.x < bounds.xMax; position.x = reverse ? position.x - 1 : position.x + 1)
        {
            for (position.y = bounds.y; position.y < bounds.yMax; position.y++)
            {
                TileBase tile = tilemap.GetTile(position);
                if (tile != null && tile.name.Contains(doorTileName))
                {
                    return position; // Return the door position immediately
                }
            }
        }

        Debug.LogWarning("No door tile found in the specified direction.");
        return Vector3Int.zero;
    }

    public void GenerateMap()
    {
        ClearMap();
        Debug.Log("Generating map...");

        GameObject firstRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)], Vector3.zero, Quaternion.identity);
        firstRoom.transform.SetParent(transform);

        GameObject lastRoom = firstRoom;

        for (int i = 1; i < numRooms; i++)
        {
            // Randomly select a room prefab
            GameObject roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

            Tilemap tilemap = roomPrefab.GetComponentInChildren<Tilemap>();
            if (tilemap != null)
            {
                Vector3 spawnPosition = Vector3.zero;

                BoundsInt bounds = tilemap.cellBounds;

                // Get the door position of the room prefab
                Vector3Int doorPosition = getDoorPosition(tilemap);

                // Find the last room's door position
                Tilemap lastRoomTilemap = lastRoom.GetComponentInChildren<Tilemap>();
                Vector3Int lastRoomDoorPosition = getDoorPosition(lastRoomTilemap, true);

                // The offset to spawn the new room
                Vector3 offset = new(lastRoomDoorPosition.x - doorPosition.x, lastRoomDoorPosition.y - doorPosition.y, 0);

                spawnPosition = lastRoom.transform.position + offset; // Calculate the spawn position

                // Instantiate the new room prefab at the calculated position
                GameObject room = Instantiate(roomPrefab, spawnPosition, Quaternion.identity);
                room.transform.SetParent(transform); // Set the parent to this GameObject

                lastRoom = room; // Update the last room reference
            }
            else
            {
                Debug.LogWarning("No Tilemap found in the room prefab: " + roomPrefab.name);
            }
        }
    }

    public void ClearMap()
    {
        Debug.Log("Clearing map...");
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
