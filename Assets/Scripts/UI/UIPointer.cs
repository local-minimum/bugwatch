using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

public enum UIPointerMode { None, Default, Interaction };

public class UIPointer : MonoBehaviour
{
    private static UIPointer _instance { get; set; }

    private static UIPointerMode _mode = UIPointerMode.None;
    public static UIPointerMode Mode {
        get {
            return _mode;
        }
        set {
            if (_mode == value) return;
            _instance.Set(value);
        }
    }

    public static string Verb
    {
        set
        {
            _instance.SetVerb(value);
        }
    }

    [SerializeField]
    Image pointer;

    [SerializeField]
    Image interactablePointer;

    [SerializeField]
    Color active;

    [SerializeField]
    Color inactive;

    [SerializeField]
    UIPointerMode startMode = UIPointerMode.Default;

    [SerializeField]
    Image verbKeyboardImage;

    [SerializeField]
    InputActionAsset inputActions;

    void Set(UIPointerMode mode)
    {
        pointer.color = mode == UIPointerMode.None ? inactive : active;
        interactablePointer.color = mode == UIPointerMode.Interaction ? active : inactive;
        _mode = mode;
    }

    string _verb = " ";
    void SetVerb(string verb)
    {
        if (_verb == verb) return;
        if (string.IsNullOrEmpty(verb))
        {
            verbKeyboardImage.enabled = false;
        }
        else
        {
            verbKeyboardImage.enabled = true;
            var action = inputActions.FindAction("Interact", true);
            var binding = action
                .bindings
                .Select(b => b.name)
                .FirstOrDefault();

            Debug.Log(binding);
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
        }
    }

    private void Start()
    {
        Mode = startMode;
        Verb = null;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
