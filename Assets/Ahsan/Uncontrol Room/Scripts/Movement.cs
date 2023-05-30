using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Tooltip("Speed at which the character is moving")][SerializeField] float moveSpeed = 15;
    [Tooltip("Camera move speed")][SerializeField] float camSpeed = 15;

    [SerializeField] InputAction Move;
    [SerializeField] InputAction Look;

    Vector2 m_Movement;
    Vector2 m_Look;

    float lookValue;
    private void OnEnable()
    {
        Move.Enable();
        Look.Enable();
    }

    private void OnDisable()
    {
        Move.Disable();
        Look.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        lookValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Read Input
        m_Movement = Move.ReadValue<Vector2>();
        m_Look = Look.ReadValue<Vector2>();

        // Move Character
        transform.Translate(new Vector3(m_Movement.x, 0, m_Movement.y) * Time.deltaTime * moveSpeed, Space.Self);

        // Move Camera
        transform.Rotate(transform.up, m_Look.x * camSpeed, Space.World);
        lookValue -= m_Look.y * camSpeed;
        lookValue = Mathf.Clamp(lookValue, -90.0f, 90.0f);
        var CamTransform = GetComponentInChildren<Camera>().transform;
        CamTransform.rotation = Quaternion.Euler(lookValue,CamTransform.eulerAngles.y,0);









    }
}
