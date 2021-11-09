using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Caption
{
    public string text;

    public Caption(string text)
    {
        this.text = text;
    }
}

public class StoryBit : MonoBehaviour
{
    [SerializeField]
    Caption[] captions;

    public void EmitStory()
    {
        if (captions.Length == 0)
        {
            Debug.LogWarning(string.Format("Attempting to emit story {0}, but no bits stored", name));
            return;
        }
        UICaption.Show(captions[0].text);
    }

    public void EmitStory(string context)
    {
        if (captions.Length == 0)
        {
            Debug.LogWarning(string.Format("Attempting to emit story {0} (context {1}), but no bits stored", name, context));
            return;
        }
        UICaption.Show(captions[0].text);
    }
}
