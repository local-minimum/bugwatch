using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum SightingType { None, Spot, Observe, Capture };
public enum Sighting { Crawler, Skidder, Flyer };

public class BugSighting : MonoBehaviour
{
    [SerializeField]
    SightingType[] sightingTypes;

    [SerializeField]
    Sighting sighting;

    [SerializeField]
    CollectableType[] requiredCollectables;

    Interactable interactable;

    [SerializeField]
    StoryBit sightingStory;

    [SerializeField]
    StoryBit refuseStory;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        if (interactable == null)
        {
            Debug.LogError(string.Format("{0} wsa a bug sighting without interactable!", name));
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

    void HandleRefuseSighting(CollectableType collectableType)
    {
        refuseStory?.EmitStory(collectableType.ToString());
    }

    void HandleRefuseSighting(SightingType sightingType)
    {
        refuseStory?.EmitStory(sightingType.ToString());
    }

    void HandleSighting()
    {
        sightingStory?.EmitStory();
    }

    private void Interactable_OnActivation()
    {
        var currentType = BugWatchSettings.GetNextSightingType(sighting);
        if (sightingTypes.Any(s => s == currentType))
        {
            HandleRefuseSighting(currentType);
            return;
        }

        for(int i=0; i<requiredCollectables.Length; i++)
        {
            if (BugWatchSettings.HasPickedUp(requiredCollectables[i]))
            {
                HandleRefuseSighting(requiredCollectables[i]);
                return;
            }
        }

        HandleSighting();
    }
}
