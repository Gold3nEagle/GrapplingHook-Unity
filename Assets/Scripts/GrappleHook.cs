using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [Header("Refrences")]
    private PlayerMovement player;
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

    private bool isGrappling;

    
    void Start()
    {
        player = GetComponent<PlayerMovement>();
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
        player.Freeze = true;

        // if there is a hit
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExcuteGrapple), grappleDelayTime);
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

    private void ExcuteGrapple()
    {
        player.Freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRealtiveYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRealtiveYPos + overshootYAxis;

        if (grapplePointRealtiveYPos < 0) highestPointOnArc = overshootYAxis;

        player.JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f);

    }

    public void StopGrapple()
    {
        player.Freeze = false;

        isGrappling = false;
        grapplingCoolDownTimer = grapplingCoolDown;

        gunLineRenderer.enabled = false;
    }

}
