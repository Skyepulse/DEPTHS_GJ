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

    enum Direction
    {
        North,
        East,
        South,
        West,
        None
    }

    private Vector3Int getDoorPosition(Tilemap tilemap, Direction direction)
    {
        Vector3Int position = Vector3Int.zero;
        BoundsInt bounds = tilemap.cellBounds;

        switch (direction)
        {
            case Direction.North:
                for (position.y = bounds.yMax - 1; position.y >= bounds.y; position.y--)
                {
                    for (position.x = bounds.x; position.x < bounds.xMax; position.x++)
                    {
                        TileBase tile = tilemap.GetTile(position);
                        if (tile != null && tile.name.Contains(doorTileName))
                        {
                            return position;
                        }
                    }
                }
                break;

            case Direction.South:
                for (position.y = bounds.y; position.y < bounds.yMax; position.y++)
                {
                    for (position.x = bounds.x; position.x < bounds.xMax; position.x++)
                    {
                        TileBase tile = tilemap.GetTile(position);
                        if (tile != null && tile.name.Contains(doorTileName))
                        {
                            return position;
                        }
                    }
                }
                break;

            case Direction.East:
                for (position.x = bounds.xMax - 1; position.x >= bounds.x; position.x--)
                {
                    for (position.y = bounds.y; position.y < bounds.yMax; position.y++)
                    {
                        TileBase tile = tilemap.GetTile(position);
                        if (tile != null && tile.name.Contains(doorTileName))
                        {
                            return position;
                        }
                    }
                }
                break;

            case Direction.West:
                for (position.x = bounds.x; position.x < bounds.xMax; position.x++)
                {
                    for (position.y = bounds.y; position.y < bounds.yMax; position.y++)
                    {
                        TileBase tile = tilemap.GetTile(position);
                        if (tile != null && tile.name.Contains(doorTileName))
                        {
                            return position;
                        }
                    }
                }
                break;
        }

        Debug.LogWarning("No door tile found in the specified direction.");
        return Vector3Int.zero;
    }

    private Direction getOppositeDirection(Direction direction)
    {
        return (Direction)(((int)direction + 2) % 4);
    }

    private BoundsInt getRoomBounds(GameObject room)
    {
        Tilemap tilemap = room.GetComponentInChildren<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogWarning("No Tilemap found in the room prefab: " + room.name);
            return new BoundsInt();
        }

        BoundsInt globalBounds = tilemap.cellBounds;
        Vector2Int min = new(int.MaxValue, int.MaxValue);
        Vector2Int max = new(int.MinValue, int.MinValue);

        for (int x = globalBounds.x; x < globalBounds.xMax; x++)
        {
            for (int y = globalBounds.y; y < globalBounds.yMax; y++)
            {
                Vector3Int localPosition = new(x, y, 0);
                TileBase tile = tilemap.GetTile(localPosition);
                if (tile != null && tile.name.Contains(doorTileName))
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

    private bool Intersects(BoundsInt a, BoundsInt b)
    {
        return a.xMin < b.xMax && a.xMax > b.xMin && a.yMin < b.yMax && a.yMax > b.yMin;
    }

    public void GenerateMap()
    {
        ClearMap();
        Debug.Log("Generating map...");

        GameObject firstRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)], Vector3.zero, Quaternion.identity);
        firstRoom.transform.SetParent(transform);

        GameObject[] rooms = new GameObject[numRooms];
        rooms[0] = firstRoom; // Store the first room
        Direction[] roomDirections = new Direction[numRooms];
        roomDirections[0] = Direction.None; // First room has no direction

        GameObject lastRoom = firstRoom;
        Direction lastRoomDirection = Direction.None;

        string generationSteps = "";

        for (int i = 1; i < numRooms; i++)
        {
            bool roomFound = false;

            int attempts = 0;
            while (!roomFound && attempts < 30)
            {
                attempts++;
                // Randomly select a room prefab
                GameObject roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

                Tilemap tilemap = roomPrefab.GetComponentInChildren<Tilemap>();
                if (tilemap == null)
                {
                    Debug.LogWarning("No Tilemap found in the room prefab: " + roomPrefab.name);
                    return;
                }

                // Random room direction that isn't the same as the last room's direction
                Direction roomDirection;
                if (lastRoomDirection == Direction.None) roomDirection = (Direction)Random.Range(0, 4);
                else roomDirection = (Direction)(((int)lastRoomDirection + Random.Range(1, 4)) % 4);

                generationSteps += roomDirection.ToString() + " -> ";

                BoundsInt bounds = tilemap.cellBounds;

                // Find the last room's door position
                Tilemap lastRoomTilemap = lastRoom.GetComponentInChildren<Tilemap>();
                Vector3Int lastRoomDoorPosition = getDoorPosition(lastRoomTilemap, roomDirection);

                // Get the door position of the room prefab
                Vector3Int doorPosition = getDoorPosition(tilemap, getOppositeDirection(roomDirection));

                // The offset to spawn the new room
                Vector3 offset = new(lastRoomDoorPosition.x - doorPosition.x, lastRoomDoorPosition.y - doorPosition.y, 0);

                Vector3 spawnPosition = lastRoom.transform.position + offset; // Calculate the spawn position

                roomFound = true;
                // Check intersection with previously placed rooms
                for (int j = 0; j < i - 1; j++)
                {
                    BoundsInt roomBounds = getRoomBounds(rooms[j]);
                    BoundsInt newRoomBounds = getRoomBounds(roomPrefab);
                    newRoomBounds.position += Vector3Int.FloorToInt(spawnPosition);
                    roomBounds.position += Vector3Int.FloorToInt(rooms[j].transform.position);

                    if (Intersects(roomBounds, newRoomBounds))
                    {
                        roomFound = false;
                        break;
                    }
                }
                if (!roomFound) continue;

                // Instantiate the new room prefab at the calculated position
                GameObject room = Instantiate(roomPrefab, spawnPosition, Quaternion.identity);
                room.transform.SetParent(transform);

                lastRoom = room;
                lastRoomDirection = getOppositeDirection(roomDirection);
                rooms[i] = room; // Store the new room
                roomDirections[i] = roomDirection; // Store the direction of the new room
            }

            if (!roomFound) return;
        }

        // Remove all door tiles from the rooms

        for (int i = 0; i < numRooms; i++)
        {
            Tilemap tilemap = rooms[i].GetComponentInChildren<Tilemap>();
            if (tilemap == null)
            {
                Debug.LogWarning("No Tilemap found in the room prefab: " + rooms[i].name);
                continue;
            }

            Direction roomDirection = roomDirections[i];
            Vector3Int doorPosition = getDoorPosition(tilemap, getOppositeDirection(roomDirection));

            BoundsInt bounds = tilemap.cellBounds;
            for (int x = bounds.x; x < bounds.xMax; x++)
            {
                for (int y = bounds.y; y < bounds.yMax; y++)
                {
                    Vector3Int localPosition = new(x, y, 0);
                    TileBase tile = tilemap.GetTile(localPosition);
                    if (tile != null && tile.name.Contains(doorTileName) && localPosition != doorPosition)
                    {
                        tilemap.SetTile(localPosition, null); // Remove the door tile
                    }
                }
            }
        }

        Debug.Log("Map generation complete. Steps: " + generationSteps);
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
