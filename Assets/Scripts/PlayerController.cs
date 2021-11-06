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

        // Move
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var moveVector = transform.forward * vertical + transform.right * horizontal;
        moveVector = Vector3.ClampMagnitude(moveVector, 1);
        _controller.Move(moveVector * speed * Time.deltaTime);

        // Rotation
        var a = Input.GetAxis("Mouse X") * mouseAngleSpeed * Time.deltaTime;
        var rotation = transform.rotation;
        rotation *= Quaternion.AngleAxis(a, Vector3.up);
        transform.rotation = rotation;

        // Updown look
        _upDownRotation = Mathf.Clamp(
            _upDownRotation - Input.GetAxis("Mouse Y") * mouseAngleSpeed * Time.deltaTime,
            lookMinRotation,
            lookMaxRotation
        );
        _cameraTransform.localRotation = Quaternion.Euler(_upDownRotation, 0, 0);

    }

    private void OnDestroy()
    {
        SceneManager.UnloadSceneAsync("GameUI");
    }
}
