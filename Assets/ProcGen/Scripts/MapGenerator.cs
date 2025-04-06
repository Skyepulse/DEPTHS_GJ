using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject[] roomPrefabs;

    [SerializeField]
    private int numRooms = 5;

    public enum Direction
    {
        North,
        East,
        South,
        West,
        None
    }

    struct RoomNode
    {
        public GameObject room;
        public Room.Door doorIn;
        public Room.Door doorOut;
    }

    private List<Room.Door> GetDoorPositions(Room room, Direction direction)
    {
        List<Room.Door> doors = new();

        foreach (var door in room.doors)
        {
            if (door.direction == direction)
            {
                doors.Add(door);
            }
        }

        return doors;
    }

    private Direction GetOppositeDirection(Direction direction)
    {
        if (direction == Direction.None)
            return Direction.None;

        return (Direction)(((int)direction + 2) % 4);
    }

    private BoundsInt GetRoomBounds(GameObject room)
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

        RoomNode[] roomNodes = new RoomNode[numRooms];
        roomNodes[0].room = firstRoom;
        roomNodes[0].doorIn = new Room.Door
        {
            position = Vector3Int.zero,
            direction = Direction.None,
            room = firstRoom.GetComponent<Room>()
        };

        string generationSteps = "";

        for (int i = 1; i < numRooms; i++)
        {
            bool roomFound = false;

            int attempts = 0;
            while (!roomFound && attempts < 30)
            {
                attempts++;

                List<Room.Door> newRoomDoorCandidates = new();
                foreach (var door in roomNodes[i - 1].room.GetComponent<Room>().doors)
                {
                    if (door.direction != roomNodes[i - 1].doorOut.direction)
                    {
                        Room.Door modifiedDoor = door;
                        modifiedDoor.room = roomNodes[i - 1].room.GetComponent<Room>();
                        newRoomDoorCandidates.Add(modifiedDoor);
                    }
                }
                roomNodes[i - 1].doorOut = newRoomDoorCandidates[Random.Range(0, newRoomDoorCandidates.Count)];

                Direction roomDirection = GetOppositeDirection(roomNodes[i - 1].doorOut.direction);

                generationSteps += roomDirection.ToString() + " -> ";

                List<Room.Door> candidateDoors = new();
                foreach (GameObject roomPrefab in roomPrefabs)
                {
                    List<Room.Door> roomDoors = GetDoorPositions(roomPrefab.GetComponent<Room>(), roomDirection);

                    foreach (var door in roomDoors)
                        if (door.direction == roomDirection)
                        {
                            Room.Door modifiedDoor = door;
                            modifiedDoor.room = roomPrefab.GetComponent<Room>();
                            candidateDoors.Add(modifiedDoor);
                        }
                }
                if (candidateDoors.Count == 0)
                {
                    Debug.LogWarning("No candidate doors found for direction: " + roomDirection);
                    break;
                }

                Room.Door newDoorIn = candidateDoors[Random.Range(0, candidateDoors.Count)];

                // The offset to spawn the new room
                Vector3 offset = new(roomNodes[i - 1].doorOut.position.x - newDoorIn.position.x, roomNodes[i - 1].doorOut.position.y - newDoorIn.position.y, 0);

                Vector3 spawnPosition = roomNodes[i - 1].room.transform.position + offset; // Calculate the spawn position

                roomFound = true;
                // Check intersection with previously placed rooms
                for (int j = 0; j < i - 1; j++)
                {
                    BoundsInt roomBounds = GetRoomBounds(roomNodes[j].room);
                    BoundsInt newRoomBounds = GetRoomBounds(newDoorIn.room.GetPrefab());
                    newRoomBounds.position += Vector3Int.FloorToInt(spawnPosition);
                    roomBounds.position += Vector3Int.FloorToInt(roomNodes[j].room.transform.position);

                    if (Intersects(roomBounds, newRoomBounds))
                    {
                        roomFound = false;
                        break;
                    }
                }
                if (!roomFound) continue;

                // Instantiate the new room prefab at the calculated position
                GameObject room = Instantiate(newDoorIn.room.GetPrefab(), spawnPosition, Quaternion.identity);
                room.transform.SetParent(transform);

                roomNodes[i] = new RoomNode
                {
                    room = room,
                    doorIn = newDoorIn
                };
            }

            if (!roomFound) return;
        }

        // Remove all door tiles from the rooms

        for (int i = 0; i < numRooms; i++)
        {
            Tilemap tilemap = roomNodes[i].room.GetComponentInChildren<Tilemap>();
            if (tilemap == null)
            {
                Debug.LogWarning("No Tilemap found in the room prefab: " + roomNodes[i].room.name);
                continue;
            }

            foreach (var door in roomNodes[i].room.GetComponent<Room>().doors)
            {
                if (door.position != roomNodes[i].doorOut.position)
                {
                    Vector3Int localPosition = new(door.position.x, door.position.y, 0);
                    TileBase tile = tilemap.GetTile(localPosition);
                    tilemap.SetTile(localPosition, null); // Remove the door tile

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
