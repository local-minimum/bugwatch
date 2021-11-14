using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerProfileMood { Indecisive, Scared, Entusiast, Lawful, Parasitized };

[System.Serializable]
public class PlayerProfile {
    public int Indecisive;
    public int Scared;
    public int Enthusiast;
    public int Lawful;
    public int Parasitized;

    public PlayerProfile(int indecisive, int scared, int enthusiast, int lawful, int parasitized)
    {
        Indecisive = indecisive;
        Scared = scared;
        Enthusiast = enthusiast;
        Lawful = lawful;
        Parasitized = parasitized;
    }

    public PlayerProfile(System.Func<PlayerProfileMood, int> moodLoader)
    {
        Indecisive = moodLoader(PlayerProfileMood.Indecisive);
        Scared = moodLoader(PlayerProfileMood.Scared);
        Enthusiast = moodLoader(PlayerProfileMood.Entusiast);
        Lawful = moodLoader(PlayerProfileMood.Lawful);
        Parasitized = moodLoader(PlayerProfileMood.Parasitized);
    }

    public PlayerProfile Evolve(PlayerProfile other)
    {
        return new PlayerProfile(
            Mathf.Max(0, Indecisive + other.Indecisive),
            Mathf.Max(0, Scared + other.Scared),
            Mathf.Max(0, Enthusiast + other.Enthusiast),
            Mathf.Max(0, Lawful + other.Lawful),
            Mathf.Max(0, Parasitized + other.Parasitized)
        );
    }

    public bool MatchesRequirement(PlayerProfile requirement)
    {
        return requirement == null
            || Indecisive >= requirement.Indecisive
            && Scared >= requirement.Scared
            && Enthusiast >= requirement.Enthusiast
            && Lawful >= requirement.Lawful
            && Parasitized >= requirement.Parasitized;
    }

    public bool MatchesMaxRuirement(PlayerProfile maxRequirement)
    {
        return maxRequirement == null
            || (Indecisive <= maxRequirement.Indecisive || maxRequirement.Indecisive < 0)
            && (Scared <= maxRequirement.Scared || maxRequirement.Scared < 0)
            && (Enthusiast <= maxRequirement.Enthusiast || maxRequirement.Enthusiast < 0)
            && (Lawful <= maxRequirement.Lawful || maxRequirement.Lawful < 0)
            && (Parasitized <= maxRequirement.Parasitized || maxRequirement.Parasitized < 0);
    }

    public int GetMoodValue(PlayerProfileMood mood)
    {
        switch (mood)
        {
            case PlayerProfileMood.Entusiast:
                return Enthusiast;
            case PlayerProfileMood.Scared:
                return Scared;
            case PlayerProfileMood.Lawful:
                return Lawful;
            case PlayerProfileMood.Indecisive:
                return Indecisive;
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
