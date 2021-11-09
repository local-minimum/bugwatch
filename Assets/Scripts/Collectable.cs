using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectableType
{
    None,
    Sweets,
    Rat,
    Gloves,
    Helmet,
    Weapon
};

public class Collectable : MonoBehaviour
{
    [SerializeField]
    CollectableType collectableType;

    [SerializeField]
    CollectableType requiredCollectable = CollectableType.None;

    Interactable interactable;

    [SerializeField]
    StoryBit refuseStory;

    [SerializeField]
    StoryBit pickupStory;

    private void Awake()
    {
        if (BugWatchSettings.HasPickedUp(collectableType))
        {
            Destroy(gameObject);
        }
        interactable = GetComponent<Interactable>();
        if (interactable == null)
        {
            Debug.LogError(string.Format("{0} was collectable but not interactable!", name));
            Destroy(gameObject);
        } else if (collectableType == CollectableType.None)
        {
            Debug.LogError(string.Format("{0} was of collectable type None, this isn't allowed", name));
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        interactable.OnActivation += Interactable_OnActivation;
    }

    private void OnDisable()
    {
        if (interactable)
        {
            interactable.OnActivation -= Interactable_OnActivation;
        }
    }

    private void HandleRefuseAction()
    {
        refuseStory?.EmitStory();
    }

    private void HandlePickUp()
    {
        BugWatchSettings.PickUp(collectableType);
        pickupStory?.EmitStory();
        Destroy(gameObject);
    }

    private void Interactable_OnActivation()
    {
        if (requiredCollectable != CollectableType.None & !BugWatchSettings.HasPickedUp(requiredCollectable)) {
            HandleRefuseAction();
        } else
        {
            HandlePickUp();
        }
    }
}
