using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ActivationEvent();

public class Interactable : MonoBehaviour
{
    public event ActivationEvent OnActivation;

    [SerializeField, Range(0, 100)]
    private float maxDistanceNotice = 3f;

    [SerializeField, Range(0, 100)]
    private float maxDistanceActivate = 1f;

    private bool live = true;

    [SerializeField]
    private string _activationVerb;

    public string activationVerb
    {
        get { return _activationVerb; }
    }

    private void Start()
    {
        if (maxDistanceActivate > maxDistanceNotice)
        {
            Debug.LogError(string.Format("{0} interactable can be activated further away than noticed!", name));
        }
    }
    public UIPointerMode InteractionMode(float distance)
    {
        return distance <= maxDistanceNotice && live ? UIPointerMode.Interaction : UIPointerMode.Default;
    }
    
    public bool CanTriggerInteraction(float distance)
    {
        return live && distance < maxDistanceActivate;
    }

    public void UpdateInteraction(float distance, string interactionKey)
    {
        UIPointer.Mode = InteractionMode(distance);
        if (CanTriggerInteraction(distance))
        {
            UIPointer.VerbKey = interactionKey;
            UIPointer.Verb = activationVerb;
            
        }
    }

    public void Activate()
    {
        OnActivation?.Invoke();
    }
}
