using UnityEngine;

public enum DoorDirection { Up, Down, Left, Right }

public class Door : MonoBehaviour
{
    // Reference to the room this door belongs to
    public Room parentRoom;

    public DoorDirection direction;

    // Event triggered when the player enters through the door
    public delegate void PlayerEnteredDoor(Door door);
    public event PlayerEnteredDoor OnPlayerEnteredDoor;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered room: {parentRoom.name} through door at position: {transform.position}");

            // Trigger any subscribed events
            OnPlayerEnteredDoor?.Invoke(this);

            // Optionally, activate the room or handle other logic
            parentRoom.gameObject.SetActive(true);
        }
    }
}
