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
    int grappleType;

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


    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        cam = Camera.main.transform;
    }

    private void Update()
    {
        if (Input.GetKey(grappleKey)) StartGrapple(); 

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

        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxGrappleDistance, whatIsGrappleable))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grappleable") && !hit.collider.gameObject.CompareTag("Enemy"))
            {
                grappleType = 1;
                grapplePoint = hit.point;
                Invoke(nameof(ExcuteGrapple), grappleDelayTime);
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grappleable") && hit.collider.gameObject.CompareTag("Enemy"))
            {
                grappleType = 2;
                grapplePoint = hit.point;
                Invoke(nameof(ExcuteGrapple), grappleDelayTime);
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Pullable"))
            {
                grapplePoint = hit.point;
                PullObject(hit.collider.gameObject);
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                grappleType = 2;
                grapplePoint = hit.point;
                Invoke(nameof(ExcuteGrapple), grappleDelayTime);
            }
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        gunLineRenderer.enabled = true;
        gunLineRenderer.SetPosition(1, grapplePoint);
    }

    private void PullObject(GameObject objectToPull)
    {
        Vector3 pullDirection = (objectToPull.transform.position - transform.position).normalized; 
        objectToPull.GetComponent<Rigidbody>().AddForce(pullDirection * pullForce, ForceMode.Impulse); 
        Invoke(nameof(StopGrapple), .25f);
    }


    private void ExcuteGrapple()
    {
        playerMovement.Freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRealtiveYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRealtiveYPos + overshootYAxis;

        if (grapplePointRealtiveYPos < 0) highestPointOnArc = overshootYAxis;

        playerMovement.JumpToPosition(grapplePoint, highestPointOnArc, grappleType);
        Invoke(nameof(StopGrapple), .25f);

    }

    public void StopGrapple()
    {
        playerMovement.Freeze = false; 
        isGrappling = false;
        grapplingCoolDownTimer = grapplingCoolDown; 
        gunLineRenderer.enabled = false;
    }

}
