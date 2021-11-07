using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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

    bool ready = false;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        StartCoroutine(WaitForLoaded(SceneManager.LoadSceneAsync("GameUI", LoadSceneMode.Additive)));
        
    }

    private void OnEnable()
    {
        GetComponent<PlayerInput>().onActionTriggered += PlayerController_onActionTriggered;
    }

    private void OnDisable()
    {
        GetComponent<PlayerInput>().onActionTriggered -= PlayerController_onActionTriggered;
    }

    private void PlayerController_onActionTriggered(InputAction.CallbackContext context)
    {
        if (!ready) return;
        var action = context.action;
        switch (action.name)
        {
            case "Move":
                OnMove(context.ReadValue<Vector2>());
                break;
            case "Interact":
                break;
            case "Look":
                break;
        }
        
    }

    IEnumerator<WaitForEndOfFrame> WaitForLoaded(AsyncOperation operation)
    {
        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        ready = true;
        UIPointer.Mode = UIPointerMode.Default;
    }

    private void Update()
    {        
        if (!ready) return;        
        UpdateLookAt();
    }

    public void OnMove(Vector2 moveVector)
    {        
        _controller.Move(moveVector.normalized * speed * Time.deltaTime);
    }

    public void OnLook(InputValue input)
    {
        Vector2 lookVector = input.Get<Vector2>();
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
            _upDownRotation - y * mouseAngleSpeed * Time.deltaTime,
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
                interactable.UpdateInteraction(hit.distance);
            } else
            {
                UIPointer.Mode = UIPointerMode.Default;
                UIPointer.Verb = null;
            }
        } else
        {
            UIPointer.Mode = UIPointerMode.Default;
            UIPointer.Verb = null;
        }
    }

    private void OnDestroy()
    {
        SceneManager.UnloadSceneAsync("GameUI");
    }
}
