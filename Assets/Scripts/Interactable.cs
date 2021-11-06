using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    private float maxDistanceNotice = 1f;

    [SerializeField, Range(0, 100)]
    private float maxDistanceActivate = 0.5f;

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
}
