using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

public static class BugWatchSettings
{
    static bool _verbose = false;
    private static void _ResetSection(string section)
    {
        var start = string.Format("{0}.", section);
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var fields = typeof(BugWatchSettings)
            .GetFields(flags)
            .Where(f => f.FieldType == typeof(string))
            .Select(f => f.Name)
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

    #region CONTROLS
    private static readonly string _CONTROLS = "Controls";
    private static readonly string _MOUSE_Y_DIRECTION = string.Format("{0}.Mouse.Y", _CONTROLS);
    public static float MouseYDirection
    {
        get
        {
            return PlayerPrefs.GetInt(_MOUSE_Y_DIRECTION, 1) == 1 ? 1f : -1f;
        }
    }

    public static bool MouseYDirectionInverted
    {
        get
        {
            return PlayerPrefs.GetInt(_MOUSE_Y_DIRECTION, 1) == -1;
        }
        set
        {
            PlayerPrefs.SetInt(_MOUSE_Y_DIRECTION, value ? -1 : 1);
        }
    }

    public static void ResetControls() => _ResetSection(_CONTROLS);
    #endregion

    #region Inventory
    private static readonly string _INVENTORY = "Inventory";

    private static string CollectableKey(CollectableType collectable) => string.Format("{0}.{1}", _INVENTORY, collectable.ToString());    
    public static bool HasPickedUp(CollectableType collectable) => PlayerPrefs.HasKey(CollectableKey(collectable));

    public static void PickUp(CollectableType collectable)
    {
        PlayerPrefs.SetInt(CollectableKey(collectable), 42);
    }

    public static void ResetInventory() => _ResetSection(_INVENTORY);
    #endregion
}
