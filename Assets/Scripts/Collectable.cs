using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectableType
{
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

    Interactable interactable;

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

    private void Interactable_OnActivation()
    {
        BugWatchSettings.PickUp(collectableType);
        Destroy(gameObject);
    }
}
