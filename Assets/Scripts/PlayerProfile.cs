using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerProfileMood { Taciturn, Quirky, Entusiast, Sad };

[System.Serializable]
public class PlayerProfile {
    public int Taciturn;
    public int Quirky;
    public int Enthusiast;
    public int Sad;

    public PlayerProfile(int taciturn, int quirky, int enthusiast, int sad)
    {
        Taciturn = taciturn;
        Quirky = quirky;
        Enthusiast = enthusiast;
        Sad = sad;
    }

    public PlayerProfile(System.Func<PlayerProfileMood, int> moodLoader)
    {
        Taciturn = moodLoader(PlayerProfileMood.Taciturn);
        Quirky = moodLoader(PlayerProfileMood.Quirky);
        Enthusiast = moodLoader(PlayerProfileMood.Entusiast);
        Sad = moodLoader(PlayerProfileMood.Sad);
    }

    public PlayerProfile Evolve(PlayerProfile other)
    {
        return new PlayerProfile(
            Mathf.Max(0, Taciturn + other.Taciturn),
            Mathf.Max(0, Quirky + other.Quirky),
            Mathf.Max(0, Enthusiast + other.Enthusiast),
            Mathf.Max(0, Sad + other.Sad)
        );
    }

    public bool MatchesRequirement(PlayerProfile requirement)
    {
        return requirement == null
            || Taciturn >= requirement.Taciturn
            && Quirky >= requirement.Quirky
            && Enthusiast >= requirement.Enthusiast
            && Sad >= requirement.Sad;
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
