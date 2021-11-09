using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField, Tooltip("If requiring None, can't be opened.")]
    CollectableType[] requiredCollectables;

    [SerializeField]
    StoryBit entryStory;

    [SerializeField]
    StoryBit refusedEntryStory;

    private void HandleRefusedEntry(CollectableType collectableType)
    {
        refusedEntryStory?.EmitStory(collectableType.ToString());
    }

    private void HandleFirstEntry()
    {
        entryStory?.EmitStory("first");
    }

    private void HandleEntry()
    {
        entryStory?.EmitStory("consecutive");
    }

    private bool DisableColliders()
    {
        bool anyDisabled = false;
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            if (!col.isTrigger && col.enabled)
            {
                col.enabled = false;
                anyDisabled = true;
            }
        }
        return anyDisabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)        
        {
            for (int i=0; i<requiredCollectables.Length; i++)
            {
                if (!BugWatchSettings.HasPickedUp(requiredCollectables[i]))
                {
                    HandleRefusedEntry(requiredCollectables[i]);
                    return;
                }
            }
            if (DisableColliders())
            {
                HandleFirstEntry();
            } else
            {
                HandleEntry();
            }            
        } else
        {
            Debug.Log("Other " + other.name);
        }
    }
}
