using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerController : MonoBehaviour
{
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

    bool ready = false;
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        playerControls = new MovementControl();
    }

    private void Start()
    {
        Cursor.visible = false;
        StartCoroutine(WaitForLoaded(SceneManager.LoadSceneAsync("GameUI", LoadSceneMode.Additive)));
        
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    IEnumerator<WaitForEndOfFrame> WaitForLoaded(AsyncOperation operation)
    {
        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        ready = true;
        UIPointer.Mode = UIPointerMode.Default;
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

    public void OnMove(Vector2 input)
    {
        var move = Vector3.ClampMagnitude(transform.forward * input.y + transform.right * input.x, 1);
        _controller.Move(move * speed * Time.deltaTime);
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
            _upDownRotation - y,
            lookMinRotation,
            lookMaxRotation
        );
        _cameraTransform.localRotation = Quaternion.Euler(_upDownRotation, 0, 0);
    }

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
            } else
            {
                UIPointer.Mode = UIPointerMode.Default;
                UIPointer.Verb = null;
                UIPointer.VerbKey = null;
            }
        } else
        {
            UIPointer.Mode = UIPointerMode.Default;
            UIPointer.Verb = null;
            UIPointer.VerbKey = null;
        }
    }

    private void OnDestroy()
    {
        SceneManager.UnloadSceneAsync("GameUI");
    }
}
