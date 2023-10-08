using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [Header("Refrences")]
    private ThirdPersonController player;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gunTip;
    [SerializeField] private LayerMask whatIsGrappleable;
    [SerializeField] private LineRenderer gunLineRenderer;

    [Header("Grappling")]
    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;

    [SerializeField] private Vector3 grapplePoint;


    [Header("Cooldown")]
    [SerializeField] private float grapplingCoolDown;
    [SerializeField] private float grapplingCoolDownTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool isGrappling;

    
    void Start()
    {
        player = GetComponent<ThirdPersonController>();
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

        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExcuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        gunLineRenderer.enabled = true;
        gunLineRenderer.SetPosition(1, grapplePoint);

    }

    private void ExcuteGrapple()
    {

    }

    private void StopGrapple()
    {
        isGrappling = false;
        grapplingCoolDownTimer = grapplingCoolDown;

        gunLineRenderer.enabled = false;
    }

}
