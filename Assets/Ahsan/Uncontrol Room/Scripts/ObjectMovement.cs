using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime *  moveSpeed);
    }

    public void MoveBackward()
    {
        transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);

    }

    public void MoveRight()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);

    }

    public void MoveLeft()
    {
        transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);

    }
}
