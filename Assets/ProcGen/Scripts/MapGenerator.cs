using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{

private static MapGenerator _instance;
    public static MapGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("No MapGenerator instance found in the scene.");
            }
            return _instance;
        }
    }

    public MapGenerator()
    {
        _instance = this;
    }

    [SerializeField]
    private GameObject[] roomPrefabs;

    [SerializeField]
    private int numRooms = 5;

    [SerializeField]
    private GameObject tunnelPrefab;

    [SerializeField]
    private GameObject endTriggerPrefab;

    private RoomNode[] roomNodes;

    public enum Direction
    {
        North,
        East,
        South,
        West,
        None
    }

    public struct RoomNode
    {
        public GameObject room;
        public Room.Door doorIn;
        public Room.Door doorOut;
    }

    private Direction GetOppositeDirection(Direction direction)
    {
        if (direction == Direction.None)
            return Direction.None;

        return (Direction)(((int)direction + 2) % 4);
    }


    private bool Intersects(BoundsInt a, BoundsInt b)
    {
        return a.xMin < b.xMax && a.xMax > b.xMin && a.yMin < b.yMax && a.yMax > b.yMin;
    }

    public void GenerateMap(int _numRooms = -1)
    {
        if(_numRooms < 0)
            _numRooms = numRooms;

        ClearMap();
        Debug.Log("Generating map...");

        GameObject firstRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)], Vector3.zero, Quaternion.identity);
        firstRoom.transform.SetParent(transform);
        firstRoom.GetComponent<Room>().RoomIndex = 0;

        roomNodes = new RoomNode[_numRooms];
        roomNodes[0].room = firstRoom;
        roomNodes[0].doorIn = new Room.Door
        {
            position = Vector3Int.zero,
            direction = Direction.None,
            room = firstRoom.GetComponent<Room>()
        };

        string generationSteps = "";

        for (int i = 1; i < _numRooms; i++)
        {
            bool roomFound = false;

            int attempts = 0;
            while (!roomFound && attempts < 30)
            {
                attempts++;

                List<Room.Door> newRoomDoorCandidates = roomNodes[i - 1].doorIn.room.GetComponent<Room>().GetDoorPositions(roomNodes[i - 1].doorIn.direction, true);

                roomNodes[i - 1].doorOut = newRoomDoorCandidates[Random.Range(0, newRoomDoorCandidates.Count)];

                Direction roomDirection = GetOppositeDirection(roomNodes[i - 1].doorOut.direction);

                generationSteps += roomDirection.ToString() + " -> ";

                List<Room.Door> candidateDoors = new();
                foreach (GameObject roomPrefab in roomPrefabs)
                {
                    List<Room.Door> roomDoors = roomPrefab.GetComponent<Room>().GetDoorPositions(roomDirection);
                    foreach (var door in roomDoors)
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
                    BoundsInt roomBounds = roomNodes[j].room.GetComponent<Room>().GetBounds();
                    BoundsInt newRoomBounds = newDoorIn.room.GetPrefab().GetComponent<Room>().GetBounds();
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
                room.GetComponent<Room>().RoomIndex = i;

                roomNodes[i] = new RoomNode
                {
                    room = room,
                    doorIn = newDoorIn
                };
            }

            if (!roomFound) return;
        }

        // Place tunnels between rooms
        for (int i = 0; i < _numRooms; i++)
        {
            List<Room.Door> doors = roomNodes[i].room.GetComponent<Room>().GetDoors();
            for (int j = 0; j < doors.Count; j++)
            {
                Room.Door door = doors[j];
                if (roomNodes[i].doorIn.position == door.position) continue; // Skip if the door is the same as the doorIn

                Vector3 localPosition = new Vector3(door.position.x, door.position.y, 0) + roomNodes[i].room.transform.position;
                Vector3Int spawnPosition = Vector3Int.FloorToInt(localPosition);
                GameObject tunnel = Instantiate(tunnelPrefab, spawnPosition, Quaternion.identity);
                tunnel.transform.SetParent(transform);
                
                if (tunnel.TryGetComponent<Tunnel>(out var tunnelComponent))
                {
                    tunnelComponent.Direction = door.direction;
                    tunnelComponent.SetState((roomNodes[i].doorOut?.position == door.position) ? Tunnel.State.Closed : Tunnel.State.Wall);
                    door.tunnel = tunnelComponent;
                }
                else
                {
                    Debug.LogWarning("Tunnel component not found on the tunnel prefab: " + tunnelPrefab.name);
                }
            }
        }

        // Set the doorOut tunnel of each room to open
        for (int i = 0; i < _numRooms; i++) 
        {
            roomNodes[i].doorIn?.tunnel?.SetState(Tunnel.State.Open);
            roomNodes[i].doorOut?.tunnel?.SetState(Tunnel.State.Open);
        }

        // Place end trigger in the last room
        GameObject endRoom = roomNodes[_numRooms - 1].room;
        if (endRoom != null)
        {
            BoundsInt endRoomBounds = endRoom.GetComponent<Room>().GetBounds();
            Vector3 endPosition = endRoom.transform.position + endRoomBounds.center;
            GameObject endTrigger = Instantiate(endTriggerPrefab, endPosition, Quaternion.identity);
            endTrigger.transform.SetParent(transform);
        }
        else
        {
            Debug.LogWarning("End room is null. Cannot place end trigger.");
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

    public RoomNode WhichRoom(Vector3 position)
    {
        foreach (var roomNode in roomNodes)
        {
            if (roomNode.room == null) continue; // Skip if the room is null
            BoundsInt roomBounds = roomNode.room.GetComponent<Room>().GetBounds();
            roomBounds.position += Vector3Int.FloorToInt(roomNode.room.transform.position);
            if (roomBounds.Contains(Vector3Int.FloorToInt(position)))
            {
                return roomNode;
            }
        }

        return default;
    }

    public Vector3 GetSpawnPoint()
    {
        RoomNode spawnRoom = roomNodes[0];
        // Get the center of the bounds of the spawn room
        BoundsInt spawnRoomBounds = spawnRoom.room.GetComponent<Room>().GetBounds();
        spawnRoomBounds.position += Vector3Int.FloorToInt(spawnRoom.room.transform.position);

        Vector3 spawnPoint = spawnRoomBounds.center;
        spawnPoint.z = 0;

        return spawnPoint;
    }

    public static void Destroy()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }

    public void CloseDoors(int roomIndex) {
        if (roomIndex < 0 || roomIndex >= roomNodes.Length)
        {
            Debug.LogError("Invalid room index: " + roomIndex);
            return;
        }

        RoomNode roomNode = roomNodes[roomIndex];
        if (roomNode.room != null)
        {
            roomNode.doorIn?.tunnel?.SetState(Tunnel.State.Closed);
        }
        if(roomIndex > 0) {
            roomNodes[roomIndex - 1].doorOut?.tunnel?.SetState(Tunnel.State.Closed);
        }
    }
}
