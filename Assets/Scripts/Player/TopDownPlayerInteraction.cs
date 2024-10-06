using UnityEngine;

public class TopDownPlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("The maximum range within which the player can interact with objects.")]
    public float interactRange = 1f;

    [Tooltip("The layer that defines which objects are interactable.")]
    public LayerMask interactableLayer;

    [Tooltip("Reference to the player's movement controller to access the direction they are facing.")]
    private TopDownCharacterController playerController; // Reference to the movement script

    private void Start()
    {
        // Get the player's movement script to access direction
        playerController = GetComponent<TopDownCharacterController>();
    }

    private void Update()
    {
        HandleInteract();
    }

    private void HandleInteract()
    {
        // Check if the Interact button is pressed
        if (Input.GetButtonDown("Interact"))
        {
            // Get the player's last direction from the movement script
            Vector2 facingDirection = playerController.GetFacingDirection();

            // Cast a ray in the direction the player is facing to detect interactables
            RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDirection, interactRange, interactableLayer);

            // If an interactable object is hit, call its Interact method
            if (hit.collider != null)
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}

public interface IInteractable
{
    void Interact();
}
