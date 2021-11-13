using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDistrict : MonoBehaviour
{
    [SerializeField]
    private Transform spawnPosition;

    [SerializeField]
    private string districtId;

    [SerializeField]
    private string districtName;

    static Dictionary<string, WorldDistrict> districts = new Dictionary<string, WorldDistrict>();

    private void Awake()
    {
        if (string.IsNullOrEmpty(districtId))
        {
            Debug.LogError(string.Format("{0} lacks a valid location id", name));
            Destroy(gameObject);
        } else if (!districts.ContainsKey(districtId))
        {
            districts.Add(districtId, this);
        } else if (districts[districtId] != this)
        {
            Debug.LogError(string.Format("{0} is duplicating id '{1}' from {2}", name, districtId, districts[districtId].name));
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (BugWatchSettings.District == districtId)
        {
            var player = Instantiate(Resources.Load<GameObject>("Player"));
            var rey = new Ray(spawnPosition.position, spawnPosition.up * -1);
            player.transform.position = spawnPosition.position;
            RaycastHit hit;
            if (Physics.Raycast(rey, out hit))
            {
                var offset = player.GetComponent<CharacterController>().bounds.ClosestPoint(hit.point) - hit.point;
                player.transform.position -= offset;
            } else
            {
                Debug.LogWarning("Failed to detect ground where spawning player");

            }            
            player.transform.rotation = spawnPosition.rotation;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            BugWatchSettings.District = districtId;
            if (!string.IsNullOrEmpty(districtName))
            {
                if (UICaption.Ready)
                {
                    UICaption.Show(districtName);
                } else
                {
                    StartCoroutine(DelayShowText(districtName));
                }
            }
        }
    }

    private IEnumerator<WaitForSeconds> DelayShowText(string text)
    {
        while (!UICaption.Ready)
        {
            yield return new WaitForSeconds(0.5f);
        }
        UICaption.Show(text);
    }

    private void OnDestroy()
    {
        if (districts.ContainsKey(districtId) && districts[districtId] == this)
        {
            districts.Remove(districtId);
        }
    }
}
