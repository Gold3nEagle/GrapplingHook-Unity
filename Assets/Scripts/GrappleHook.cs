using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [Header("Refrences")]
    private PlayerMovement playerMovement;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform gunTip;
    [SerializeField] private LayerMask whatIsGrappleable;
    [SerializeField] private LineRenderer gunLineRenderer;

    [Header("Grappling")]
    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;
    [SerializeField] private float overshootYAxis;

    [SerializeField] private Vector3 grapplePoint;


    [Header("Cooldown")]
    [SerializeField] private float grapplingCoolDown;
    [SerializeField] private float grapplingCoolDownTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    [SerializeField] private float pullForce = 10f; 
    private bool isGrappling;

    public Transform pullToPosition;


    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        cam = Camera.main.transform;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) StartGrapple();

        if (grapplingCoolDownTimer > 0)
            grapplingCoolDownTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (isGrappling)
        {
            gunLineRenderer.SetPosition(0, gunTip.position);
        }
    }

    private void StartGrapple()
    {
        if (grapplingCoolDownTimer > 0) return;

        isGrappling = true;
        playerMovement.Freeze = true;

        // Check for objects that can be grappled or pulled
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxGrappleDistance, whatIsGrappleable))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grappleable"))
            {
                grapplePoint = hit.point;
                Invoke(nameof(ExcuteGrapple), grappleDelayTime);
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Pullable"))
            {
                // Implement pulling logic here
                grapplePoint = hit.point;
                PullObject(hit.collider.gameObject);
            }
        }
        // if there is no hit
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        // enable the visuals
        gunLineRenderer.enabled = true;
        gunLineRenderer.SetPosition(1, grapplePoint);
    }

    private void PullObject(GameObject objectToPull)
    {
        // Calculate the direction from the player to the object
        Vector3 pullDirection = (objectToPull.transform.position - transform.position).normalized;

        // Apply force to pull the object towards the player
        objectToPull.GetComponent<Rigidbody>().AddForce(pullDirection * pullForce, ForceMode.Impulse); 

        // Optionally, you can disable any physics interactions with the pulled object
        // objectToPull.GetComponent<Rigidbody>().isKinematic = true;
        Invoke(nameof(StopGrapple), 1f);
    }


    private void ExcuteGrapple()
    {
        playerMovement.Freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRealtiveYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRealtiveYPos + overshootYAxis;

        if (grapplePointRealtiveYPos < 0) highestPointOnArc = overshootYAxis;

        playerMovement.JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f);

    }

    public void StopGrapple()
    {
        playerMovement.Freeze = false;

        isGrappling = false;
        grapplingCoolDownTimer = grapplingCoolDown;

        gunLineRenderer.enabled = false;
    }

}
