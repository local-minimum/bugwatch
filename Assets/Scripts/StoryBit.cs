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

enum Prioritized { Caption, Dialogue, None };

[System.Serializable]
public class Caption
{
    [Tooltip("No ID means it is repeatable.")]
    public string id;
    public string text;
    [Tooltip("If dialogue is useful only in particular context such as lacking a certain collectable.")]
    public string context;
    public int priority;

    public PlayerProfile requirement;
    public PlayerProfile maxRequirement;
    public PlayerProfile moodEffect;

    public AudioClip narration;
}

[System.Serializable]
public class Dialogue
{
    [Tooltip("No ID means it is repeatable. Note ID of contained captions irrelevant.")]
    public string id;
    public string intro;
    public int priority;
    public string context;
    public Caption[] options;
    public PlayerProfile requirement;
    public PlayerProfile maxRequirement;
}

public class StoryBit : MonoBehaviour
{
    [SerializeField]
    Story story;

    [SerializeField]
    List<Caption> captions = new List<Caption>();

    [SerializeField]
    List<Dialogue> dialogues = new List<Dialogue>();

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
            .Where(c => !used.Contains(c.id) 
                && playerProfile.MatchesRequirement(c.requirement)
                && playerProfile.MatchesMaxRuirement(c.maxRequirement))
            .OrderBy(c =>c.priority);
    }

    private IEnumerable<Dialogue> UnusedDialogues()
    {
        var used = BugWatchSettings.UsedStoryBits(story);
        var playerProfile = BugWatchSettings.PlayerProfile;
        return dialogues
            .Where(d => !used.Contains(d.id)
                && playerProfile.MatchesRequirement(d.requirement)
                && playerProfile.MatchesMaxRuirement(d.maxRequirement))
            .OrderBy(d => d.priority);
    }

    private bool StoryLess
    {
        get
        {
            return captions.Count == 0 && dialogues.Count == 0;
        }
    }

    private Prioritized Priority(Caption caption, Dialogue dialogue)
    {
        if (caption != null && dialogue == null || caption.priority < dialogue.priority)
        {
            return Prioritized.Caption;
        } else if (dialogue != null)
        {
            return Prioritized.Dialogue;
        }
        return Prioritized.None;
    }

    public void EmitStory()
    {
        if (StoryLess)
        {
            Debug.LogWarning(string.Format("Attempting to emit story {0}, but no bits stored", name));
            return;
        }
        var caption = UnusedCaptions().FirstOrDefault();
        var dialogue = UnusedDialogues().FirstOrDefault();
        switch (Priority(caption, dialogue))
        {
            case Prioritized.Caption:
                if (!EmitCaption(caption))
                {
                    Debug.LogWarning(string.Format("{0} attempted to emit a caption but non left", name));
                }
                break;
            case Prioritized.Dialogue:
                if (!EmitDialogue(dialogue))
                {
                    Debug.LogWarning(string.Format("{0} attempt to emit a dialogue but non left", name));
                }

                break;
            case Prioritized.None:
                Debug.LogWarning(string.Format("{0} exhausted story bit", name));
                break;
        }
    }

    public void EmitStory(string context)
    {
        if (StoryLess)
        {
            Debug.LogWarning(string.Format("Attempting to emit story {0} (context {1}), but no bits stored", name, context));
            return;
        }
        var caption = UnusedCaptions().Where(c => c.context == context).FirstOrDefault();
        var dialogue = UnusedDialogues().Where(d => d.context == context).FirstOrDefault();
        switch (Priority(caption, dialogue))
        {
            case Prioritized.Caption:
                if (!EmitCaption(caption))
                {
                    Debug.LogWarning(string.Format("{0} attempted to emit a caption but non left matching context {1}", name, context));
                }
                break;
            case Prioritized.Dialogue:
                if (!EmitDialogue(dialogue))
                {
                    Debug.LogWarning(string.Format("{0} attempt to emit a dialogue but non left matching context {1}", name, context));
                }
                break;
            case Prioritized.None:
                Debug.LogWarning(string.Format("{0} exhausted story bit", name));
                break;
        }
    }

    bool EmitCaption(Caption caption)
    {
        if (caption != null)
        {
            var duration = UICaption.Show(caption.text, caption.narration == null ? 0f : caption.narration.length);
            if (caption.narration)
            {
                PlayerInternalSpeaker.Speaker.PlayOneShot(caption.narration);
            } else
            {
                PlayerInternalSpeaker.Mumble(duration);
            }

            BugWatchSettings.UseStoryBit(story, caption.id);
            if (caption.moodEffect != null)
            {
                PlayerProfile profile = BugWatchSettings.PlayerProfile.Evolve(caption.moodEffect);
                BugWatchSettings.PlayerProfile = profile;
            }
            return true;
        } else
        {
            return false;           
        }
    }

    bool EmitDialogue(Dialogue dialogue)
    {
        if (dialogue != null)
        {
            PlayerController.Pause = true;
            return true;
        } else
        {
            return false;
        }
    }
}
