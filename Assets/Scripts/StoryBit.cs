using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Story
{   
    _General,
    RatPickup,
    RatRefuse,
}

[System.Serializable]
public class Caption
{
    [Tooltip("No ID means it is repeatable")]
    public string id;
    public string text;
    [Tooltip("If dialogue is useful only in particular context such as lacking a certain collectable.")]
    public string context;

    public PlayerProfile requirement;
    public PlayerProfile moodEffect;
}

public class StoryBit : MonoBehaviour
{
    [SerializeField]
    Story story;

    [SerializeField]
    Caption[] captions;

    private static Dictionary<Story, StoryBit> storyBits = new Dictionary<Story, StoryBit>();

    private void Awake()
    {
        if (story != Story._General) {
            if (!storyBits.ContainsKey(story)) {
                storyBits.Add(story, this);
            } else if (storyBits[story] != this)
            {
                Debug.LogError(string.Format(
                    "Duplicate story on two different story bits {0} and {1}. Not intended!",
                    storyBits[story].name,
                    name
                ));
            }
        }
    }

    private void OnDestroy()
    {
        if (storyBits.ContainsKey(story) && storyBits[story] == this)
        {
            storyBits.Remove(story);
        }
    }

    private IEnumerable<Caption> UnusedCaptions()
    {
        var used = BugWatchSettings.UsedStoryBits(story);
        var playerProfile = BugWatchSettings.PlayerProfile;
        return captions
            .Where(c => !used.Contains(c.id) && playerProfile.MatchesRequirement(c.requirement));
    }

    public void EmitStory()
    {
        if (captions.Length == 0)
        {
            Debug.LogWarning(string.Format("Attempting to emit story {0}, but no bits stored", name));
            return;
        }
        EmitCaption(UnusedCaptions().FirstOrDefault());

    }

    public void EmitStory(string context)
    {
        if (captions.Length == 0)
        {
            Debug.LogWarning(string.Format("Attempting to emit story {0} (context {1}), but no bits stored", name, context));
            return;
        }
        EmitCaption(UnusedCaptions().Where(c => c.context == context).FirstOrDefault());        
    }

    void EmitCaption(Caption caption)
    {
        if (caption != null)
        {
            UICaption.Show(caption.text);
            BugWatchSettings.UseStoryBit(story, caption.id);
            if (caption.moodEffect != null)
            {
                PlayerProfile profile = BugWatchSettings.PlayerProfile.Evolve(caption.moodEffect);
                BugWatchSettings.PlayerProfile = profile;
            }
        } else
        {
            Debug.LogWarning(string.Format("{0} attempted to emit a caption but non left matching requirements", name));
        }
    }
}
