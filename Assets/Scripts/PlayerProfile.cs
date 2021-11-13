using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerProfileMood { Taciturn, Quirky, Entusiast, Sad, Parasitized };

[System.Serializable]
public class PlayerProfile {
    public int Taciturn;
    public int Quirky;
    public int Enthusiast;
    public int Sad;
    public int Parasitized;

    public PlayerProfile(int taciturn, int quirky, int enthusiast, int sad, int parasitized)
    {
        Taciturn = taciturn;
        Quirky = quirky;
        Enthusiast = enthusiast;
        Sad = sad;
        Parasitized = parasitized;
    }

    public PlayerProfile(System.Func<PlayerProfileMood, int> moodLoader)
    {
        Taciturn = moodLoader(PlayerProfileMood.Taciturn);
        Quirky = moodLoader(PlayerProfileMood.Quirky);
        Enthusiast = moodLoader(PlayerProfileMood.Entusiast);
        Sad = moodLoader(PlayerProfileMood.Sad);
        Parasitized = moodLoader(PlayerProfileMood.Parasitized);
    }

    public PlayerProfile Evolve(PlayerProfile other)
    {
        return new PlayerProfile(
            Mathf.Max(0, Taciturn + other.Taciturn),
            Mathf.Max(0, Quirky + other.Quirky),
            Mathf.Max(0, Enthusiast + other.Enthusiast),
            Mathf.Max(0, Sad + other.Sad),
            Mathf.Max(0, Parasitized + other.Parasitized)
        );
    }

    public bool MatchesRequirement(PlayerProfile requirement)
    {
        return requirement == null
            || Taciturn >= requirement.Taciturn
            && Quirky >= requirement.Quirky
            && Enthusiast >= requirement.Enthusiast
            && Sad >= requirement.Sad
            && Parasitized >= requirement.Parasitized;
    }

    public bool MatchesMaxRuirement(PlayerProfile maxRequirement)
    {
        return maxRequirement == null
            || (Taciturn <= maxRequirement.Taciturn || maxRequirement.Taciturn < 0)
            && (Quirky <= maxRequirement.Quirky || maxRequirement.Quirky < 0)
            && (Enthusiast <= maxRequirement.Enthusiast || maxRequirement.Enthusiast < 0)
            && (Sad <= maxRequirement.Sad || maxRequirement.Sad < 0)
            && (Parasitized <= maxRequirement.Parasitized || maxRequirement.Parasitized < 0);
    }

    public int GetMoodValue(PlayerProfileMood mood)
    {
        switch (mood)
        {
            case PlayerProfileMood.Entusiast:
                return Enthusiast;
            case PlayerProfileMood.Quirky:
                return Quirky;
            case PlayerProfileMood.Sad:
                return Sad;
            case PlayerProfileMood.Taciturn:
                return Taciturn;
            case PlayerProfileMood.Parasitized:
                return Parasitized;
            default:
                Debug.LogError(string.Format("{0} not implemented as value", mood));
                return 0;
        }
    }

    public IEnumerable<(PlayerProfileMood, int)> AsSequence()
    {
        IList list = System.Enum.GetValues(typeof(PlayerProfileMood));
        for (int i = 0, l = list.Count; i<l;  i++)
        {
            PlayerProfileMood mood = (PlayerProfileMood)list[i];
            yield return (mood, GetMoodValue(mood));
        }
    }
}
