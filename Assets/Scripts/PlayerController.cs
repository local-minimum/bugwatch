using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance { get; set; }

    public static bool Pause
    {
        set { instance.Paused = value; }
    }

    [SerializeField, Range(1, 15)]
    float speed = 3.5f;
    private CharacterController _controller;

    [SerializeField, Range(50, 900)]
    float mouseAngleSpeed = 150f;
    [SerializeField, Range(-90, 0)]
    float lookMinRotation = -45f;
    [SerializeField, Range(0, 90)]
    float lookMaxRotation = 30f;
    float _upDownRotation;
    [SerializeField]
    Transform _cameraTransform;

    [SerializeField, Range(10, 1000)]
    float viewDistance = 200;

    MovementControl playerControls;

    bool useGravity = true;
    bool ready = false;
    float mouseYDirection = 1;

    public bool Paused
    {
        set
        {
            ready = !value;
            if (value)
            {
                CleanUIPointer(UIPointerMode.None);
            }            
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (instance != this)
        {
            Destroy(gameObject);
        } else
        {
            _controller = GetComponent<CharacterController>();
            playerControls = new MovementControl();
            mouseYDirection = BugWatchSettings.MouseYDirectionInverted ? -1 : 1;
        }
    }

    private void Start()
    {
        Cursor.visible = false;
        StartCoroutine(WaitForLoaded(SceneManager.LoadSceneAsync("GameUI", LoadSceneMode.Additive)));
        BugWatchSettings.OnChangeBoolSetting += BugWatchSettings_OnChangeBoolSetting;
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.Interact.started += InteractStart;
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerControls.Player.Interact.started -= InteractStart;
    }

    private void BugWatchSettings_OnChangeBoolSetting(GameSetting setting, bool value)
    {
        switch (setting)
        {
            case GameSetting.MouseInvertY:
                mouseYDirection = value ? -1 : 1;
                break;
        }
    }

    private void InteractStart(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (interactable)
        {
            interactable.Activate();
        }
    }

    IEnumerator<WaitForEndOfFrame> WaitForLoaded(AsyncOperation operation)
    {
        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        ready = true;
        CleanUIPointer(UIPointerMode.Default);
    }

    void CleanUIPointer(UIPointerMode mode)
    {
        UIPointer.Mode = mode;
        UIPointer.Verb = null;
        UIPointer.VerbKey = null;
    }

    private void Update()
    {        
        if (!ready) return;        
        var move = playerControls.Player.Move.ReadValue<Vector2>();
        var look = playerControls.Player.Look.ReadValue<Vector2>();
        OnMove(move);
        OnLook(look);
        UpdateLookAt();
    }
    const float _GRAVITY = 9.81f;
    public void OnMove(Vector2 input)
    {
        var move = Vector3.ClampMagnitude(transform.forward * input.y + transform.right * input.x, 1) * speed;
        if (!_controller.isGrounded && useGravity) move.y -= _GRAVITY;
        _controller.Move(move * Time.deltaTime);
    }

    public void OnLook(Vector2 lookVector)
    {
        lookVector *= Time.deltaTime * mouseAngleSpeed;
        UpdateRotation(lookVector.x);
        UpdateUpDownLook(lookVector.y);

    }

    private void UpdateRotation(float x)
    {                
        var rotation = transform.rotation;
        rotation *= Quaternion.AngleAxis(x, Vector3.up);
        transform.rotation = rotation;
    }

    private void UpdateUpDownLook(float y)
    {
        _upDownRotation = Mathf.Clamp(
            _upDownRotation - y * mouseYDirection,
            lookMinRotation,
            lookMaxRotation
        );
        _cameraTransform.localRotation = Quaternion.Euler(_upDownRotation, 0, 0);
    }

    Interactable interactable;

    private void UpdateLookAt()
    {
        RaycastHit hit;
        Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
        if (Physics.Raycast(ray, out hit, viewDistance))
        {
            var interactable = hit.collider.GetComponent<Interactable>();
            if (interactable)
            {
                var bindingKey = playerControls
                    .Player
                    .Interact
                    .bindings
                    .Select(b => b.path.Split('/').LastOrDefault())
                    .FirstOrDefault();                
                interactable.UpdateInteraction(hit.distance, bindingKey);

                if (interactable.CanTriggerInteraction(hit.distance))
                {
                    this.interactable = interactable;
                } else
                {
                    this.interactable = null;
                }
            } else
            {
                CleanUIPointer(UIPointerMode.Default);
                this.interactable = null;
            }
        } else
        {
            CleanUIPointer(UIPointerMode.Default);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
        SceneManager.UnloadSceneAsync("GameUI");
        BugWatchSettings.OnChangeBoolSetting -= BugWatchSettings_OnChangeBoolSetting;
    }
}
