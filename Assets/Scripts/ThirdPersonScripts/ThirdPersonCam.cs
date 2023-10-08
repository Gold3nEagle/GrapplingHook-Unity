using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{

    [Header("Refrences")]
    public Transform Orientation;
    public Transform Player;
    public Transform PlayerObject;
    public Rigidbody rb;

    public float RotationSpeed;

    public CameraStyle CurrentStyle;

    public Transform CombatLookAt;

    public enum CameraStyle
    {
        Basic,
        Combat,
        Topdown
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // rotate orientation
        Vector3 viewDirection = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        Orientation.forward = viewDirection.normalized;

        // rotate player object
        if (CurrentStyle == CameraStyle.Basic)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 inputDirection = Orientation.forward * verticalInput + Orientation.right * horizontalInput;

            if (inputDirection != Vector3.zero)
                PlayerObject.forward = Vector3.Slerp(PlayerObject.forward, inputDirection.normalized, Time.deltaTime * RotationSpeed);
        }
        else if (CurrentStyle == CameraStyle.Basic)
        {
            Vector3 dirToCombatLookAt = CombatLookAt.position - new Vector3(transform.position.x, CombatLookAt.position.y, transform.position.z);
            Orientation.forward = dirToCombatLookAt.normalized;

            PlayerObject.forward = dirToCombatLookAt.normalized;
        }
        

    }

}
