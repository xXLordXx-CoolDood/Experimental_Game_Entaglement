using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PROTO_DogLikeMech : MonoBehaviour
{
    public List<Transform> legTargets = new List<Transform>(4);
    public float maxLegHeight = 3f;
    public float legSpeed;

    private PlayerInput playerInput;
    private List<bool> legActive = new List<bool>(4);

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        for(int i = 0; i < legTargets.Count; i++) { legActive.Add(false); }
    }

    #region
    public void LeftLegF(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { legActive[0] = true; }
        if(ctx.canceled) { legActive[0] = false; }
    }
    public void RightLegF(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { legActive[1] = true; }
        if (ctx.canceled) { legActive[1] = false; }
    }
    public void LeftLegB(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { legActive[2] = true; }
        if (ctx.canceled) { legActive[2] = false; }
    }

    public void RightLegB(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) { legActive[3] = true; }
        if (ctx.canceled) { legActive[3] = false; }
    }

    #endregion

    private void Update()
    {
        int i = 0;
        foreach (bool val in legActive)
        {
            if (val) { legTargets[i].transform.localPosition += new Vector3(0, 0.03f * Time.deltaTime * legSpeed, 0); }
            if (!val) { legTargets[i].transform.localPosition -= new Vector3(0, 0.03f * Time.deltaTime * legSpeed, 0); }

            legTargets[i].position = new Vector3(legTargets[i].position.x, Mathf.Clamp(legTargets[i].position.y, -0.051f, maxLegHeight), legTargets[i].position.z);

            i++;
        }
    }
}
