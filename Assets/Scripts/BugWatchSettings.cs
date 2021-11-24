using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

public enum GameSetting { MouseInvertY, WordsPerMinute };
public delegate void SettingsChange<T>(GameSetting setting, T value);


public static class BugWatchSettings
{
    public static event SettingsChange<float> OnChangeFloatSetting;
    public static event SettingsChange<bool> OnChangeBoolSetting;

    #region GENERIC
    static bool _verbose = true;
    private static void _ResetSection(string section)
    {
        var start = string.Format("{0}.", section);
        var flags = BindingFlags.Static | BindingFlags.NonPublic;

        var fields = typeof(BugWatchSettings)
            .GetFields(flags)
            .Where(f => f.FieldType == typeof(string))
            .Select(f => f.GetValue(null).ToString())
            .Where(fName => fName.StartsWith(start))
            .ToList();
        foreach (string field in fields)
        {
            if (PlayerPrefs.HasKey(field))
            {
                if (_verbose) Debug.Log(string.Format("Removing setting for {0}", field));
                PlayerPrefs.DeleteKey(field);
            }
        }
    }
    private static void _ResetEnum<T>(System.Func<T, string> keyMaker, T e) where T : System.Enum
    {
        foreach (T collectable in System.Enum.GetValues(e.GetType()))
        {
            var key = keyMaker(collectable);
            if (PlayerPrefs.HasKey(key))
            {
                if (_verbose) Debug.Log(string.Format("Removing setting for {0}", key));
                PlayerPrefs.DeleteKey(key);
            }
        }
    }
    #endregion

    #region CONTROLS
    private static readonly string _CONTROLS = "Controls";
    private static readonly string _CONTROLS_MOUSE_Y_DIRECTION = string.Format("{0}.Mouse.Y", _CONTROLS);
    private static readonly string _CONTROLS_WORDS_PER_MINUTE = string.Format("{0}.Text.WordsPerMinute", _CONTROLS);

    public static bool MouseYDirectionInverted
    {
        get
        {
            return PlayerPrefs.GetInt(_CONTROLS_MOUSE_Y_DIRECTION, 1) == -1;
        }
        set
        {
            PlayerPrefs.SetInt(_CONTROLS_MOUSE_Y_DIRECTION, value ? -1 : 1);
            OnChangeBoolSetting?.Invoke(GameSetting.MouseInvertY, value);
        }
    }

    public static float WordsPerMinute
    {
        get
        {
            return PlayerPrefs.GetFloat(_CONTROLS_WORDS_PER_MINUTE, 220);
        }

        set
        {
            PlayerPrefs.SetFloat(_CONTROLS_WORDS_PER_MINUTE, value);
            OnChangeFloatSetting?.Invoke(GameSetting.WordsPerMinute, value);
        }
    }

    public static void ResetControls() => _ResetSection(_CONTROLS);
    #endregion

    #region INVENTORY
    private static readonly string _INVENTORY = "Inventory";

    private static string CollectableKey(CollectableType collectable) => string.Format("{0}.{1}", _INVENTORY, collectable.ToString());
    public static bool HasPickedUp(CollectableType collectable) => PlayerPrefs.HasKey(CollectableKey(collectable));

    public static void PickUp(CollectableType collectable)
    {
        PlayerPrefs.SetInt(CollectableKey(collectable), 42);
    }

    public static void ResetInventory()
    {
        _ResetSection(_INVENTORY);
        // Can be any of the enum values
        _ResetEnum(CollectableKey, CollectableType.None);
    }
    #endregion

    #region PROGRESS
    private static readonly string _PROGRESS = "Progress";
    private static readonly string _PROGRESS_HAS_GAME = string.Format("{0}.HasGame", _PROGRESS);
    private static readonly string _PROGRESS_LEVEL = string.Format("{0}.Level", _PROGRESS);
    private static readonly string _PROGRESS_DISTRICT = string.Format("{0}.District", _PROGRESS);

    public static string District
    {
        get
        {
            return PlayerPrefs.GetString(_PROGRESS_DISTRICT, "Start");
        }

        set
        {
            if (string.IsNullOrEmpty(value))
            {
                PlayerPrefs.DeleteKey(_PROGRESS_DISTRICT);
            } else
            {
                PlayerPrefs.SetString(_PROGRESS_DISTRICT, value);
            }
        }
    }
    public static bool HasProgress
    {
        get
        {
            return PlayerPrefs.HasKey(_PROGRESS_HAS_GAME);
        }
    }

    public static string Level
    {
        get
        {
            return PlayerPrefs.GetString(_PROGRESS_LEVEL, "Scenes/Level");
        }
    }

    public static void ResetProgress() => _ResetSection(_PROGRESS);
    public static void NewGame()
    {
        ResetProgress();
        ResetProfile();
        ResetInventory();
        ResetSightings();
        ResetStories();
        PlayerPrefs.SetInt(_PROGRESS_HAS_GAME, 42);
    }
    #endregion

    #region SIGHTINGS
    private static readonly string _SIGHTING = "Sighting";
    private static string SightingKey(Sighting sighting) => string.Format("{0}.{1}", _SIGHTING, sighting.ToString());
    private static string SightingIdsKey(Sighting sighting) => string.Format("{0}.{1}.ConsumedIds", _SIGHTING, sighting.ToString());

    private static SightingType GetSightingType(Sighting sighting)
    {
        var key = SightingKey(sighting);
        var value = PlayerPrefs.GetString(key, "None");
        SightingType sightingType;
        if (System.Enum.TryParse(value, out sightingType))
        {
            return sightingType;
        }
        return SightingType.None;
    }

    public static SightingType GetNextSightingType(Sighting sighting)
    {
        switch (GetSightingType(sighting))
        {
            case SightingType.None:
                return SightingType.Spot;
            case SightingType.Spot:
                return SightingType.Observe;
            case SightingType.Observe:
                return SightingType.Capture;
            default:
                return SightingType.None;
        }
    }

    static List<string> ConsumedIdsString(Sighting sighting)
    {
        var key = SightingIdsKey(sighting);
        return PlayerPrefs.GetString(key, "").Split(',').ToList();
    }

    public static bool HasConsumedSighting(Sighting sighting, string sightingId)
    {
        return ConsumedIdsString(sighting).Contains(sightingId);
    }

    public static void SetSightingType(Sighting sighting, SightingType sightingType, string sightingId)
    {
        var key = SightingKey(sighting);
        PlayerPrefs.SetString(key, sightingType.ToString());

        var ids = ConsumedIdsString(sighting);
        if (ids.Contains(sightingId))
        {
            Debug.LogError(string.Format("Attempting setting id {0} for sighting {1}/{2} though already consumed.", sightingId, sighting, sightingType));
        } else
        {
            ids.Add(sightingId);
            PlayerPrefs.SetString(SightingIdsKey(sighting), string.Join(",", ids));
        }
    }

    public static void ResetSightings()
    {
        _ResetSection(_SIGHTING);
        // Can be any of the enum values
        _ResetEnum(SightingKey, Sighting.Crawler);
    }
    #endregion
    
    #region STORY
    private static readonly string _STORY = "Story";

    private static string StoryKey(Story story) => string.Format("{0}.{1}", _STORY, story.ToString());

    public static string[] UsedStoryBits(Story story)
    {
        string key = StoryKey(story);
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetString(key).Split(',');
        }
        return new string[0];
    }

    public static void UseStoryBit(Story story, string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        if (id.Contains(","))
        {
            Debug.LogError(string.Format("Id may not contain comma: '{0}'", id));
        } else
        {
            var bits = UsedStoryBits(story);
            if (bits.Contains(id))
            {
                Debug.LogWarning(string.Format("Reused a story bit '{0}', this should not happen.", id));
            } else
            {
                PlayerPrefs.SetString(StoryKey(story), string.Join(",", bits.Concat(new string[1] { id })));
            }

        }
    }

    public static void ResetStoryBit(Story story)
    {
        PlayerPrefs.DeleteKey(StoryKey(story));
    }

    public static void ResetStories()
    {
        _ResetSection(_STORY);
        _ResetEnum(StoryKey, Story.RatPickup);
    }
    #endregion

    #region PROFILE
    private static readonly string _PROFILE = "Profile";
    private static readonly string _PROFILE_MOOD = string.Format("{0}.Mood", _PROFILE);

    private static string ProfileMoodKey(PlayerProfileMood mood) => string.Format("{0}.{1}", _PROFILE_MOOD, mood.ToString());

    public static void SetProfileMood(PlayerProfileMood mood, int value)
    {
        PlayerPrefs.SetInt(ProfileMoodKey(mood), value);
    }

    public static int GetProfileMood(PlayerProfileMood mood)
    {
        return PlayerPrefs.GetInt(ProfileMoodKey(mood), 0);
    }

    public static PlayerProfile PlayerProfile {
        get {
            return new PlayerProfile(GetProfileMood);
        }

        set
        {
            foreach(var (mood, moodValue) in value.AsSequence())
            { 
                SetProfileMood(mood, moodValue);
            }
        }
    }

    public static void ResetProfile()
    {
        _ResetSection(_PROFILE);
        _ResetEnum(ProfileMoodKey, PlayerProfileMood.Entusiast);
    }
    #endregion
}
