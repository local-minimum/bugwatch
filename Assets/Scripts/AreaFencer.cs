using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaFencer : MonoBehaviour
{
    const float FenceHeight = 10;
    const float FenceTriggerWidth = 0.75f;
    const float FenceWidth = 0.1f;

    [SerializeField]
    Transform nextPoint;

    private void Start()
    {
        transform.LookAt(nextPoint);
        Vector3 center = transform.InverseTransformPoint(Vector3.Lerp(transform.position, nextPoint.position, 0.5f));
        var z = Vector3.Distance(transform.position, nextPoint.position);
        var trigger = gameObject.AddComponent<BoxCollider>();
        var collider = gameObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.center = center;
        collider.center = center;
        trigger.size = new Vector3(FenceTriggerWidth, FenceHeight, z);
        collider.size = new Vector3(FenceWidth, FenceHeight, z);
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var gate = GetComponentInParent<Gate>();
        if (gate)
        {
            gate.HandleTriggerEnter(other);
        } else
        {
            Debug.LogWarning(string.Format("{0} does not have a partent Gate, will never open", name));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var district = GetComponentInParent<WorldDistrict>();
        if (district)
        {
            var fence = new Vector2(nextPoint.position.x - transform.position.x, nextPoint.position.z - transform.position.z);
            var player = new Vector2(other.transform.position.x - transform.position.x, other.transform.position.z - transform.position.z);
            Debug.Log(fence);
            Debug.Log(player);
            var angle = Mathf.Atan2(fence.y, fence.x) - Mathf.Atan2(player.y, player.x);
            Debug.Log(angle);
            if (angle > 0)
            {
                district.HandleTrigger(other);
            }
        }        
    }
}
