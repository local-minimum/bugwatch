using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        UpdateMove();
        UpdateRotation();
        UpdateUpDownLook();
        UpdateLookAt();
    }

    private void UpdateMove()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var moveVector = transform.forward * vertical + transform.right * horizontal;
        moveVector = Vector3.ClampMagnitude(moveVector, 1);
        _controller.Move(moveVector * speed * Time.deltaTime);
    }

    private void UpdateRotation()
    {
        var a = Input.GetAxis("Mouse X") * mouseAngleSpeed * Time.deltaTime;
        var rotation = transform.rotation;
        rotation *= Quaternion.AngleAxis(a, Vector3.up);
        transform.rotation = rotation;
    }

    private void UpdateUpDownLook()
    {
        _upDownRotation = Mathf.Clamp(
            _upDownRotation - Input.GetAxis("Mouse Y") * mouseAngleSpeed * Time.deltaTime,
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
                UIPointer.Mode = interactable.InteractionMode(hit.distance);
                if (interactable.CanTriggerInteraction(hit.distance))
                {
                    // TODO: trigger ui with verb
                    var verb = interactable.activationVerb;
                }
            } else
            {
                UIPointer.Mode = UIPointerMode.Default;
            }
        } else
        {
            UIPointer.Mode = UIPointerMode.Default;
        }
    }

    private void OnDestroy()
    {
        SceneManager.UnloadSceneAsync("GameUI");
    }
}
