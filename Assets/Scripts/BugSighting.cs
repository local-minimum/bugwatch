using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum SightingType { None, Spot, Observe, Capture };
public enum Sighting { Crawler, Skidder, Flyer };

public class BugSighting : MonoBehaviour
{
    [SerializeField]
    string sightingId;

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

    bool SightingConsumed
    {
        get
        {
            return BugWatchSettings.HasConsumedSighting(sighting, sightingId);
        }
    }

    private void Awake()
    {
        if (string.IsNullOrEmpty(sightingId))
        {
            Debug.LogError(string.Format("{0} does not have a sighting id!", name));
        }
        interactable = GetComponent<Interactable>();
        if (interactable == null)
        {
            Debug.LogError(string.Format("{0} was a bug sighting without interactable!", name));
            Destroy(gameObject);
        } else if (SightingConsumed)
        {
            interactable.Consume();
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

    void HandleSighting(SightingType sightingType)
    {
        sightingStory?.EmitStory();
        interactable.Consume();
        BugWatchSettings.SetSightingType(sighting, sightingType, sightingId);
        UIPointer.Mode = UIPointerMode.Default;
        UIPointer.Verb = null;
        UIPointer.VerbKey = null;
    }

    private void Interactable_OnActivation()
    {
        var currentType = BugWatchSettings.GetNextSightingType(sighting);               
        if (!sightingTypes.Any(s => s == currentType))
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

        HandleSighting(currentType);
    }
}
