using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeOfDay
{
    Midday,
    Evening,
    Night
}

public class Sun : MonoBehaviour
{
    private static Sun instance { get; set; }

    public static TimeOfDay TimeOfDay
    {
        get
        {
            return instance._timeOfDay;
        }

        set
        {
            instance.timeOfDay = value;
        }
    }

    private TimeOfDay _timeOfDay = TimeOfDay.Night;
    private TimeOfDay timeOfDay
    {
        set {
            StartCoroutine(EaseLight(value));
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    Color midDayColor;

    [SerializeField]
    Color eveningColor;

    [SerializeField]
    Color nightColor;

    [SerializeField, Range(1, 10)]
    float easingTime = 4;

    Light sun;

    private void Start()
    {
        timeOfDay = TimeOfDay.Midday;
        sun = GetComponent<Light>();
    }

    private Color GetColor(TimeOfDay timeOfDay)
    {
        switch (timeOfDay)
        {
            case TimeOfDay.Midday:
                return midDayColor;
            case TimeOfDay.Evening:
                return eveningColor;
            case TimeOfDay.Night:
                return nightColor;
            default:
                return midDayColor;
        }
    }

    private IEnumerator<WaitForSeconds> EaseLight(TimeOfDay timeOfDay)
    {
        var startTime = Time.timeSinceLevelLoad;
        var progress = 0f;
        var startColor = GetColor(_timeOfDay);
        var endColor = GetColor(timeOfDay);
        while (progress < 1)
        {
            yield return new WaitForSeconds(0.02f);
            progress = Mathf.Clamp01((Time.timeSinceLevelLoad - startTime) / easingTime);
            sun.color = Color.Lerp(startColor, endColor, progress);
        }
        _timeOfDay = timeOfDay;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
