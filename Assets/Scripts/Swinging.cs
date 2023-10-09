using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("Refernces")]
    public LineRenderer lineRenderer;
    public Transform GunTip, Cam, Player;
    public LayerMask WhatIsGrappleable;
    public PlayerMovement PlayerMovement;

    [Header("Swinging")]
    [SerializeField] private float maxSwingDistance = 25;
    private Vector3 swingPoint;
    private SpringJoint joint;

    [Header("Swinging")]
    public Transform Orientation;
    public Rigidbody RB;
    public float HorizontalThrustForce;
    public float ForwardThrustForce;
    public float ExtendCableSpeed;

    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;
    private Vector3 currentGrapplePosition;

    private void Start()
    {
        Cam = Camera.main.transform;
        Player = transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(swingKey)) StartSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();

        if (joint != null) OdmGearMovement();

    }

    private void OdmGearMovement()
    {
        // right
        if (Input.GetKey(KeyCode.D)) RB.AddForce(Orientation.right * HorizontalThrustForce * Time.deltaTime);
        // left
        if (Input.GetKey(KeyCode.A)) RB.AddForce(-Orientation.right * HorizontalThrustForce * Time.deltaTime);

        // forward
        if (Input.GetKey(KeyCode.W)) RB.AddForce(Orientation.forward * HorizontalThrustForce * Time.deltaTime);

        // shorten cable
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            RB.AddForce(directionToPoint.normalized * ForwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }
        // extend cable
        if (Input.GetKey(KeyCode.S))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + ExtendCableSpeed;

            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartSwing()
    {
        PlayerMovement.Swinging = true;

        GetComponent<GrappleHook>().StopGrapple();
        PlayerMovement.ResetRestrictions();


        RaycastHit hit;
        if (Physics.Raycast(Cam.position, Cam.forward, out hit, maxSwingDistance, WhatIsGrappleable))
        {
            swingPoint = hit.point;
            joint = Player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distancFromPoint = Vector3.Distance(Player.position, swingPoint);

            // the distance grapple will try to keep from grapple point
            joint.maxDistance = distancFromPoint * 0.8f;
            joint.minDistance = distancFromPoint * 0.25f;

            // customize values as you like
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lineRenderer.positionCount = 2;
            currentGrapplePosition = GunTip.position;
        }

    }

    public void StopSwing()
    {
        PlayerMovement.Swinging = false;

        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

    private void DrawRope()
    {
        if (!joint) return;
        lineRenderer.SetPosition(0, GunTip.position);
        lineRenderer.SetPosition(1, swingPoint);
    }

}
