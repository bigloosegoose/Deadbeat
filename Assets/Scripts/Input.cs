using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Input : MonoBehaviour
{

    [Header("Input")]
    [SerializeField] InputActionReference moveRef;
    [SerializeField] InputActionReference lookRef;
    [SerializeField] InputActionReference jumpRef;
    [SerializeField] InputActionReference sprintRef;
    

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5.5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;

    [Header("Look")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float lookSensitivity = 0.2f;
    [SerializeField] private float maxLookAngle = 85f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    static readonly int speedHash = Animator.StringToHash("speed");

    private CharacterController cc;
    float pitch;
    float verticalVelocity;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        moveRef.action.Enable();
        lookRef.action.Enable();
        jumpRef.action.Enable();
        sprintRef.action.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        moveRef.action.Disable();
        lookRef.action.Disable();
        jumpRef.action.Disable();
        sprintRef.action.Disable();
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        Vector2 look = lookRef.action.ReadValue<Vector2>() * lookSensitivity;

        transform.Rotate(0f, look.x, 0f); //yaw say no ahah
        pitch = Mathf.Clamp(pitch - look.y, -30f, maxLookAngle);
        cameraHolder.localRotation = Quaternion.Euler(pitch, 0f, 0f); //currently camera only

    }

    void Move()
    {
        Vector2 input = moveRef.action.ReadValue<Vector2>();
        float speed = sprintRef.action.IsPressed() ? sprintSpeed : walkSpeed;
        Vector3 horizontal = (transform.right * input.x + transform.forward * input.y) * speed;

        if (cc.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f; // try zero
        if (cc.isGrounded && jumpRef.action.WasPressedThisFrame()) verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);


        verticalVelocity += gravity * Time.deltaTime;
        cc.Move((horizontal + Vector3.up * verticalVelocity) * Time.deltaTime);

        float planarSpeed = new Vector3(horizontal.x, 0f, horizontal.z).magnitude;
        animator.SetFloat("Speed", planarSpeed, 0.1f, Time.deltaTime);
    }

}