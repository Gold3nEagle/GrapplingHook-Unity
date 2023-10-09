using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("Refernces")]
    public LineRenderer lineRenderer;
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappleable;
    public PlayerMovement playerMovement;

    [Header("Swinging")]
    [SerializeField] private float maxSwingDistance = 25;
    private Vector3 swingPoint;
    private SpringJoint joint;

    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;
    private Vector3 currentGrapplePosition;

    private void Start()
    {
        cam = Camera.main.transform;
        player = transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(swingKey)) StartSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();

    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartSwing()
    {
        playerMovement.Swinging = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
        {
            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distancFromPoint = Vector3.Distance(player.position, swingPoint);

            // the distance grapple will try to keep from grapple point
            joint.maxDistance = distancFromPoint * 0.8f;
            joint.minDistance = distancFromPoint * 0.25f;

            // customize values as you like
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lineRenderer.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }

    }

    private void StopSwing()
    {
        playerMovement.Swinging = false;

        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

    private void DrawRope()
    {
        if (!joint) return;
        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, swingPoint);
    }

}
