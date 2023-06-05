using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PROTO_DogLikeMech : MonoBehaviour
{
    

    private PlayerInput playerInput;


    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    #region
    public void LeftLegF(InputAction.CallbackContext ctx)
    {

    }
    public void RightLegF(InputAction.CallbackContext ctx)
    {

    }
    public void LeftLegB(InputAction.CallbackContext ctx)
    {

    }
    public void RightLegB(InputAction.CallbackContext ctx)
    {

    }
    public void Reverse(InputAction.CallbackContext ctx)
    {

    }
    public void Turn(InputAction.CallbackContext ctx)
    {

    }

    #endregion //Inputs

    private void LateUpdate()
    {

    }

}
