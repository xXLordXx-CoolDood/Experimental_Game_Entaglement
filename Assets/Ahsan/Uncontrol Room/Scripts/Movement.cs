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

    }

    // Update is called once per frame
    void Update()
    {

        m_Movement = Move.ReadValue<Vector2>();
        m_Look = Look.ReadValue<Vector2>();

        transform.Translate(new Vector3(m_Movement.x, 0, m_Movement.y) * Time.deltaTime * moveSpeed, Space.Self);

        transform.Rotate(transform.up, m_Look.x, Space.World);
    }
}
