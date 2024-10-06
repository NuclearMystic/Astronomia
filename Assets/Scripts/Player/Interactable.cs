using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [Tooltip("Select which type of interaction you'd like this object to trigger.")]
    public InteractionType interactionType;

    public void Interact()
    {
        switch (interactionType)
        {
            case InteractionType.Inspect:
                Debug.Log("Inpsecting...");
                break;
            case InteractionType.Activate:
                Debug.Log("Activated!");
                break;
            case InteractionType.Toggle:
                Debug.Log("Toggled.");
                break;
            case InteractionType.Talk:
                Debug.Log("Howdy!");
                break;
            default:
                Debug.Log("It's nothing...");
                break;
        }
    }
}

public enum InteractionType
{
    Toggle,
    Talk,
    Inspect,
    Activate
}
