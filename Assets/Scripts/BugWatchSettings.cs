using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

public static class BugWatchSettings
{
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

    public static void ResetControls()
    {
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var fields = typeof(BugWatchSettings)
            .GetFields(flags)
            .Where(f => f.FieldType == typeof(string))
            .Select(f => f.Name)
            .ToList();
        foreach (string field in fields)
        {
            if (PlayerPrefs.HasKey(field))
            {
                Debug.Log(string.Format("Removing setting for {0}", field));
                PlayerPrefs.DeleteKey(field);
            }
        }
    }
}
