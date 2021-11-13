using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{    
    StoryBit story;

    void Start()
    {
        story = GetComponent<StoryBit>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            story?.EmitStory();
        }
    }
}
