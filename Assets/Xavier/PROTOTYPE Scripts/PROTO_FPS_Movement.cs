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
    [SerializeField] float SurfSpeed = 12.0f;
    [SerializeField] bool SurfWithMovement = true;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] float gravity = -30f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask webs;

    private PlayerInput playerInput;
    private bool isSurfing;
    private Vector3 surfDirection;

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

            float length = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));

            boxCol.size = new Vector3(lineRenderer.startWidth, lineRenderer.startWidth, length);
            boxCol.center = Vector3.zero;
            boxCol.isTrigger = true;

            Vector3 dir = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
            lineRenderer.transform.rotation = Quaternion.LookRotation(dir);
            lineRenderer.gameObject.layer = 7;
        }
    }

    public void Movement(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isGrounded)
        {
            groundCheck.GetComponent<SphereCollider>().enabled = false;
            StartCoroutine(SurfGrabDelay());
            transform.position = new Vector3(transform.position.x, transform.position.y + (surfDirection.y * 1.25f), transform.position.z);
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (isSurfing) { velocityY *= 1.25f; }

            isSurfing = false;
        }
    }
    #endregion

    void Update()
    {
        if (isSurfing) { Surf(); return; }

        UpdateMove();
    }

    void Surf()
    {
        if (Physics.CheckSphere(groundCheck.position, 0.25f, webs) == false)
        {
            isSurfing = false;
            isGrounded = false;
            return;
        }

        transform.position += surfDirection * Time.deltaTime * SurfSpeed;
    }

    void UpdateMove()
    {
        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * Speed + Vector3.up * velocityY;

        if (Physics.CheckSphere(groundCheck.position, 0.25f, webs) && groundCheck.GetComponent<SphereCollider>().enabled)
        {
            RaycastHit[] hits = Physics.SphereCastAll(groundCheck.position, 0.25f, groundCheck.forward, 0.5f);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent<LineRenderer>(out LineRenderer lineRenderer))
                {
                    surfDirection = (lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0)).normalized;
                    float dotProduct = 0;

                    if (SurfWithMovement)
                        dotProduct = Vector3.Dot(surfDirection, velocity.normalized);
                    else
                        dotProduct = Vector3.Dot(surfDirection, playerCamera.forward);

                    if (dotProduct < 0) { surfDirection *= -1; }

                    Debug.Log($"SURF DIRECTION = {surfDirection}    MOVE DIRECTION = {velocity.normalized}    DOT PRODUCT = {dotProduct}");
                }
            }

            isSurfing = true;
            isGrounded = true;
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

        moveInput.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, moveInput, ref currentDirVelocity, moveSmoothTime);

        velocityY += gravity * 2f * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (controller.velocity.y < 0 && controller.velocity.y > -0.1f && !isSurfing)
        {
            groundCheck.GetComponent<SphereCollider>().enabled = false;
            groundCheck.GetComponent<SphereCollider>().enabled = true;
        }

        if (isGrounded! && controller.velocity.y < -1f)
        {
            velocityY = -8f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isSurfing && other.gameObject.layer == 7)
        {
            GetComponent<CharacterController>().enabled = false;
            transform.position = new Vector3(0, 1.5f, 0);
            GetComponent<CharacterController>().enabled = true;
            Debug.Log("KILLEDED");
        }
    }

    IEnumerator SurfGrabDelay()
    {
        yield return new WaitForSeconds(1);

        groundCheck.GetComponent<SphereCollider>().enabled = true;
    }
}
