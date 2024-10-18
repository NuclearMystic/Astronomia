using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    public Vector2Int roomPosition;
    public Vector2Int roomSize;

    // List to hold Door references
    public List<Door> doors = new List<Door>();

    void Awake()
    {
        // Find and process the Doors GameObject
        Transform doorsTransform = FindChildByName(transform, "Doors");
        if (doorsTransform == null)
        {
            Debug.LogError("Doors GameObject not found in room.");
        }
        else
        {
            RetrieveDoors(doorsTransform);
        }

        // Find and process the RoomSize GameObject
        Transform roomSizeTransform = FindChildByName(transform, "RoomSize");
        if (roomSizeTransform == null)
        {
            Debug.LogError("RoomSize GameObject not found in room.");
        }
        else
        {
            CalculateRoomSize(roomSizeTransform);
        }
    }

    // Helper method to find a child by name
    private Transform FindChildByName(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;
        }
        return null;
    }

    void RetrieveDoors(Transform doorsTransform)
    {
        // Get all Door components in the Doors GameObject's children
        Door[] doorComponents = doorsTransform.GetComponentsInChildren<Door>();

        foreach (Door door in doorComponents)
        {
            // Assign the parent room to the door
            door.parentRoom = this;

            // Optionally, set the door's name for easier identification
            door.gameObject.name = $"Door_{door.transform.localPosition.x}_{door.transform.localPosition.y}";

            // Add the door to the doors list
            doors.Add(door);
        }
    }

    void CalculateRoomSize(Transform roomSizeTransform)
    {
        // Get the SpriteRenderer component
        SpriteRenderer sr = roomSizeTransform.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("RoomSize GameObject does not have a SpriteRenderer component.");
            return;
        }

        // Get the bounds of the sprite in world units
        Bounds bounds = sr.bounds;

        // Calculate width and height
        float width = bounds.size.x;
        float height = bounds.size.y;

        // Optionally, round to nearest integer if you prefer integer sizes
        roomSize = new Vector2Int(Mathf.RoundToInt(width), Mathf.RoundToInt(height));

        // Debug log to verify
        Debug.Log($"Room Size - Width: {roomSize.x}, Height: {roomSize.y}");
    }
}
