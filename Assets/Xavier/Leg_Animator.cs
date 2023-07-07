using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Leg_Animator : MonoBehaviour
{
    public Transform targetPoint, groundSnap, footBone, ankleBone, kneeBone, hipBone;
    public LayerMask groundLayer;
    public float rotationMultiplier, groundCheckDistance = 1;
    public AnimationCurve ankleCurve;
    public Animator legAnimations;
    public bool isSkidding, showDebug;
    [SerializeField] private EventReference legActiveEvent, legInactiveEvent;
    [SerializeField] private TextureSound[] textureSounds;

    public Mech_Controller controllerRef;
    public bool isHeld, canMove, grounded = true;
    private EventReference currentTerrainSfx;
    [HideInInspector] public float legSpeed, legHeight;
    [HideInInspector] public int forwardMultiplier = 1;

    private Vector3 prevTargetPos = Vector3.zero, initialDir;
    private float maxLegDistance;
    private Texture terrainType = null;

    void Start()
    {
        legHeight = targetPoint.position.y;

        prevTargetPos = targetPoint.position;

        Vector2 hip = new Vector2(hipBone.position.y, hipBone.position.z);
        Vector2 knee = new Vector2(kneeBone.position.y, kneeBone.position.z);
        Vector2 ankle = new Vector2(ankleBone.position.y, ankleBone.position.z);

        maxLegDistance = Vector2.Distance(hip, knee) + Vector2.Distance(knee, ankle);
    }

    void LateUpdate()
    {
        if (!isHeld) { CheckForGround(); return; }
        grounded = false;
        //RotateAnkleBone();
    }

    private void ApplyGravity()
    {
        targetPoint.position = new Vector3(targetPoint.position.x, targetPoint.position.y - Time.deltaTime, targetPoint.position.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundSnap.position, groundCheckDistance * 3);
    }

    public void CheckForGround()
    {
        Debug.DrawRay(hipBone.position, Vector3.down * Vector3.Distance(hipBone.position, groundSnap.position));

        initialDir.Normalize();
        targetPoint.gameObject.GetComponent<Target_Follow>().follow = true;
        
        RaycastHit hit;
        if (Physics.Raycast(groundSnap.position, Vector3.down * groundCheckDistance, out hit, groundCheckDistance, groundLayer)) //Initial check to see if we need to apply leg gravity
        {
            legHeight = targetPoint.position.y;
            SetTargetFollowState(false);

            controllerRef.CheckLegIdleStatus();

            if (!grounded && Physics.Raycast(groundSnap.position, Vector3.down * groundCheckDistance, out hit, groundCheckDistance, groundLayer))
            {
                if(hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
                {
                    Vector3 terrainPos = hit.point - terrain.transform.position;
                    Vector3 splatMapPos = new Vector3(terrainPos.x / terrain.terrainData.size.x, 0, terrainPos.z / terrain.terrainData.size.z);

                    int x = Mathf.FloorToInt(splatMapPos.x * terrain.terrainData.alphamapWidth);
                    int z = Mathf.FloorToInt(splatMapPos.z * terrain.terrainData.alphamapHeight);

                    float[,,] alphaMap = terrain.terrainData.GetAlphamaps(x, z, 1, 1);

                    int primaryTexture = 0;
                    for(int i = 0; i < alphaMap.Length; i++)
                    {
                        if(alphaMap[0, 0, i] > alphaMap[0, 0, primaryTexture]) { primaryTexture = i; }
                    }

                    for(int i = 0; i < textureSounds.Length; i++) 
                    { 
                        if(textureSounds[i].texture == terrain.terrainData.terrainLayers[primaryTexture].diffuseTexture) { currentTerrainSfx = textureSounds[i].textureSound; }

                        Audio_Manager.instance.PlayOneShot(currentTerrainSfx, transform.position);
                    }
                }
            }

            grounded = true;
            canMove = false;
        }
        else if(Physics.Raycast(hipBone.position, Vector3.down, out hit, Vector3.Distance(hipBone.position, groundSnap.position), groundLayer) == false) //Check to ensure the leg didn't clip through the ground
        {
            grounded = false;
            ApplyGravity();
        }
        else //If we did clip through the ground, ground the leg back on top
        {
            SetTargetFollowState(false);
            Debug.Log("CLIPPING STOPPED");
            grounded = true;
            canMove = false;
        }

        prevTargetPos = targetPoint.position;
    }

    private void RotateAnkleBone()
    {
        if (grounded) { Vector3 rot = new Vector3(0, 0, 0); targetPoint.rotation = Quaternion.Euler(rot); return; }

        Vector2 hip = new Vector2(hipBone.position.y / 2, hipBone.position.z);
        Vector2 ankle = new Vector2(ankleBone.position.y / 2, ankleBone.position.z);

        float currentLegLength = Vector2.Distance(hip, ankle);
        float displacement = currentLegLength / maxLegDistance;

        float rotAmnt = Vector2.Angle(ankle, hip);

        if (ankleBone.position.z - hipBone.position.z > 0) { rotAmnt *= -1; }

        rotAmnt *= rotationMultiplier * ankleCurve.Evaluate(displacement);

        Vector3 ankleRot = new Vector3(0, 90, rotAmnt - 90);

        ankleBone.eulerAngles = ankleRot;
    }

    public void SetTargetFollowState(bool newState) 
    { 
        if (isSkidding) { return; } 
        targetPoint.gameObject.GetComponent<Target_Follow>().follow = newState; 
    }

    public void LegActiveStatus(bool active)
    {
        if (active) { Audio_Manager.instance.PlayOneShot(legActiveEvent, transform.position); return; }
        Audio_Manager.instance.PlayOneShot(legInactiveEvent, transform.position);
    }

    [System.Serializable]
    private class TextureSound
    {
        public Texture texture;
        public EventReference textureSound;
    }
}
