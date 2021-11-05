using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            _instance.Set(value);
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
    UIPointerMode startMode;

    void Set(UIPointerMode mode)
    {
        pointer.color = mode == UIPointerMode.None ? inactive : active;
        interactablePointer.color = mode == UIPointerMode.Interaction ? active : inactive;
        _mode = mode;
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
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
