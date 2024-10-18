using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> roomPrefabs;
    public Tilemap hallwayTilemap;
    public TileBase hallwayTile;

    [Header("Generation Settings")]
    public int numberOfRooms = 10;
    public int maxAttemptsPerRoom = 20;

    private List<Room> generatedRooms = new List<Room>();
    private List<DoorConnection> openDoors = new List<DoorConnection>();

    void Start()
    {
        GenerateRooms();
    }

    void GenerateRooms()
    {
        if (roomPrefabs.Count == 0)
        {
            Debug.LogError("No room prefabs assigned to the RoomGenerator.");
            return;
        }

        // Instantiate the first room at the origin
        GameObject firstRoomObj = Instantiate(roomPrefabs[0], Vector3.zero, Quaternion.identity);
        Room firstRoom = firstRoomObj.GetComponent<Room>();
        if (firstRoom == null)
        {
            Debug.LogError("Room prefab does not have a Room script attached.");
            return;
        }
        firstRoom.roomPosition = Vector2Int.zero;
        generatedRooms.Add(firstRoom);

        // Add all doors of the first room to openDoors
        foreach (var door in firstRoom.doors)
        {
            openDoors.Add(new DoorConnection
            {
                room = firstRoom,
                door = door
            });
        }

        // Generate remaining rooms
        for (int i = 1; i < numberOfRooms; i++)
        {
            bool roomPlaced = false;
            int attempts = 0;

            while (!roomPlaced && attempts < maxAttemptsPerRoom)
            {
                attempts++;

                if (openDoors.Count == 0)
                {
                    Debug.LogWarning("No available open doors to connect new rooms.");
                    break;
                }

                // Select a random open door
                int openDoorIndex = Random.Range(0, openDoors.Count);
                DoorConnection openDoorConnection = openDoors[openDoorIndex];
                Room existingRoom = openDoorConnection.room;
                Door existingDoor = openDoorConnection.door;

                // Select a random room prefab
                int prefabIndex = Random.Range(0, roomPrefabs.Count);
                GameObject roomPrefab = roomPrefabs[prefabIndex];
                Room newRoom = Instantiate(roomPrefab).GetComponent<Room>();

                if (newRoom == null)
                {
                    Debug.LogError("Room prefab does not have a Room script attached.");
                    Destroy(newRoom.gameObject);
                    continue;
                }

                // Select a random door from the new room to connect
                int newRoomDoorIndex = Random.Range(0, newRoom.doors.Count);
                Door newRoomDoor = newRoom.doors[newRoomDoorIndex];

                // Calculate the rotation needed to align the doors
                Quaternion rotation = GetRotationToAlignDoors(existingDoor.direction, newRoomDoor.direction);
                newRoom.transform.rotation = rotation;

                // Position the new room so that the doors align
                Vector3 existingDoorWorldPos = existingDoor.transform.position;
                Vector3 newRoomDoorLocalPos = newRoomDoor.transform.localPosition;
                Vector3 rotatedNewRoomDoorPos = rotation * newRoomDoorLocalPos;
                Vector3 newRoomPosition = existingDoorWorldPos - rotatedNewRoomDoorPos;
                newRoom.transform.position = newRoomPosition;

                // Check for overlaps
                if (IsOverlapping(newRoom))
                {
                    // Overlap detected, try another position
                    Destroy(newRoom.gameObject);
                    continue;
                }

                // No overlap, finalize placement
                newRoom.roomPosition = new Vector2Int(
                    Mathf.RoundToInt(newRoom.transform.position.x),
                    Mathf.RoundToInt(newRoom.transform.position.y)
                );
                generatedRooms.Add(newRoom);

                // Connect doors by creating a hallway
                CreateHallway(existingDoor, newRoomDoor);

                // Remove the connected door from openDoors
                openDoors.RemoveAt(openDoorIndex);

                // Add new room's doors to openDoors, excluding the connected door
                foreach (var door in newRoom.doors)
                {
                    if (door != newRoomDoor)
                    {
                        openDoors.Add(new DoorConnection
                        {
                            room = newRoom,
                            door = door
                        });
                    }
                }

                roomPlaced = true;
            }

            if (!roomPlaced)
            {
                Debug.LogWarning($"Failed to place room {i + 1} after {maxAttemptsPerRoom} attempts.");
            }
        }

        // Optionally, connect remaining open doors or handle them as needed
    }

    // Structure to keep track of open doors
    private struct DoorConnection
    {
        public Room room;
        public Door door;
    }

    // Calculate the rotation needed to align two doors based on their directions
    Quaternion GetRotationToAlignDoors(DoorDirection existingDoorDir, DoorDirection newRoomDoorDir)
    {
        // Assuming that doors should face each other
        // e.g., If existing door is Up, new room door should be Down, and vice versa
        float rotationAngle = 0f;

        switch (existingDoorDir)
        {
            case DoorDirection.Up:
                rotationAngle = GetRotationAngle(DoorDirection.Down, newRoomDoorDir);
                break;
            case DoorDirection.Down:
                rotationAngle = GetRotationAngle(DoorDirection.Up, newRoomDoorDir);
                break;
            case DoorDirection.Left:
                rotationAngle = GetRotationAngle(DoorDirection.Right, newRoomDoorDir);
                break;
            case DoorDirection.Right:
                rotationAngle = GetRotationAngle(DoorDirection.Left, newRoomDoorDir);
                break;
        }

        return Quaternion.Euler(0, 0, rotationAngle);
    }

    // Helper to determine rotation angle based on door directions
    float GetRotationAngle(DoorDirection targetDir, DoorDirection currentDir)
    {
        // Define rotation angles to align currentDir to targetDir
        if (currentDir == targetDir)
            return 0f;
        else
        {
            switch (currentDir)
            {
                case DoorDirection.Up:
                    if (targetDir == DoorDirection.Right) return -90f;
                    if (targetDir == DoorDirection.Left) return 90f;
                    if (targetDir == DoorDirection.Down) return 180f;
                    break;
                case DoorDirection.Right:
                    if (targetDir == DoorDirection.Up) return 90f;
                    if (targetDir == DoorDirection.Down) return -90f;
                    if (targetDir == DoorDirection.Left) return 180f;
                    break;
                case DoorDirection.Down:
                    if (targetDir == DoorDirection.Right) return 90f;
                    if (targetDir == DoorDirection.Left) return -90f;
                    if (targetDir == DoorDirection.Up) return 180f;
                    break;
                case DoorDirection.Left:
                    if (targetDir == DoorDirection.Up) return -90f;
                    if (targetDir == DoorDirection.Down) return 90f;
                    if (targetDir == DoorDirection.Right) return 180f;
                    break;
            }
        }

        return 0f;
    }

    // Check if the new room overlaps with any existing rooms
    bool IsOverlapping(Room newRoom)
    {
        foreach (var room in generatedRooms)
        {
            if (AreRoomsOverlapping(room, newRoom))
                return true;
        }
        return false;
    }

    // Determine if two rooms overlap based on their positions and sizes
    // Determine if two rooms overlap based on their positions and sizes
    bool AreRoomsOverlapping(Room roomA, Room roomB)
    {
        // Assuming roomPosition is the center of the room
        Vector2 posA = roomA.transform.position;
        Vector2 posB = roomB.transform.position;

        // Convert Vector2Int to Vector2 before multiplication
        Vector2 sizeA = new Vector2(roomA.roomSize.x, roomA.roomSize.y) * 0.5f;
        Vector2 sizeB = new Vector2(roomB.roomSize.x, roomB.roomSize.y) * 0.5f;

        // Check for overlap on x and y axes
        bool overlapX = Mathf.Abs(posA.x - posB.x) < (sizeA.x + sizeB.x);
        bool overlapY = Mathf.Abs(posA.y - posB.y) < (sizeA.y + sizeB.y);

        return overlapX && overlapY;
    }


    void CreateHallway(Door doorA, Door doorB)
    {
        Vector3Int startCell = hallwayTilemap.WorldToCell(doorA.transform.position);
        Vector3Int endCell = hallwayTilemap.WorldToCell(doorB.transform.position);

        List<Vector3Int> hallwayCells = GetLine(startCell, endCell);

        foreach (Vector3Int cell in hallwayCells)
        {
            hallwayTilemap.SetTile(cell, hallwayTile);
        }
    }

    List<Vector3Int> GetLine(Vector3Int from, Vector3Int to)
    {
        List<Vector3Int> line = new List<Vector3Int>();

        int x0 = from.x;
        int y0 = from.y;
        int x1 = to.x;
        int y1 = to.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            line.Add(new Vector3Int(x0, y0, from.z));

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return line;
    }
}
