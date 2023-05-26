using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PROTO_FPS_Movement : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform shotSpawn;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float Speed = 6.0f;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] float gravity = -30f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;

    private PlayerInput playerInput;

    public float jumpHeight = 6f;
    float velocityY;
    bool isGrounded;

    float cameraCap;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;
    Vector2 moveInput;

    CharacterController controller;
    Vector2 currentDir;
    Vector2 currentDirVelocity;
    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        FPSControls playerInputActions = new FPSControls();
        playerInputActions.FPS.Walking.performed += Movement;

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }

    #region input
    public void UpdateMouse(InputAction.CallbackContext ctx)
    {
        Vector2 targetMouseDelta = ctx.ReadValue<Vector2>() / 20;

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraCap -= currentMouseDelta.y * mouseSensitivity;

        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraCap;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    public void Shoot(InputAction.CallbackContext ctx)
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        //Spawn web
        if (ctx.performed && Physics.Raycast(ray, out hit))
        {
            LineRenderer lineRenderer = new GameObject().AddComponent<LineRenderer>();

            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.75f;
            lineRenderer.endWidth = 0.75f;
            lineRenderer.SetPosition(0, shotSpawn.position);
            lineRenderer.SetPosition(1, hit.point);

            lineRenderer.transform.position = (lineRenderer.GetPosition(0) + lineRenderer.GetPosition(1)) / 2;

            BoxCollider boxCol = lineRenderer.gameObject.AddComponent<BoxCollider>();

            boxCol.size = new Vector3(lineRenderer.startWidth, lineRenderer.startWidth, (lineRenderer.GetPosition(0) + lineRenderer.GetPosition(1)).magnitude);
            boxCol.center = Vector3.zero;

            Vector3 dir = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
            lineRenderer.transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    public void Movement(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isGrounded)
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
    #endregion

    void Update()
    {
        UpdateMove();
    }

    void UpdateMove()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

        moveInput.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, moveInput, ref currentDirVelocity, moveSmoothTime);

        velocityY += gravity * 2f * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * Speed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (isGrounded! && controller.velocity.y < -1f)
        {
            velocityY = -8f;
        }
    }
}
