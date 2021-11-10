using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Story
{
    RatPickup,
    RatRefuse,
}

[System.Serializable]
public class Caption
{
    [Tooltip("No ID means it is repeatable")]
    public string id;
    public string text;
}

public class StoryBit : MonoBehaviour
{
    [SerializeField]
    Story story;

    [SerializeField]
    Caption[] captions;

    private IEnumerable<Caption> UnusedCaptions()
    {
        var used = BugWatchSettings.UsedStoryBits(story);
        return captions
            .Where(c => !used.Contains(c.id));
    }

    public void EmitStory()
    {
        if (captions.Length == 0)
        {
            Debug.LogWarning(string.Format("Attempting to emit story {0}, but no bits stored", name));
            return;
        }
        Caption caption = UnusedCaptions().FirstOrDefault();
        if (caption != null)
        {
            UICaption.Show(caption.text);
            BugWatchSettings.UseStoryBit(story, caption.id);
        }

    }

    public void EmitStory(string context)
    {
        if (captions.Length == 0)
        {
            Debug.LogWarning(string.Format("Attempting to emit story {0} (context {1}), but no bits stored", name, context));
            return;
        }
        Caption caption = UnusedCaptions().FirstOrDefault();
        if (caption != null)
        {
            UICaption.Show(caption.text);
            BugWatchSettings.UseStoryBit(story, caption.id);
        }        
    }
}
