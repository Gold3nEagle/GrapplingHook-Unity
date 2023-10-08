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
    public Transform CombatLookAt;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }

    private void Update()
    {
        Vector3 viewDirection = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        Orientation.forward = viewDirection.normalized; 
        Vector3 dirToCombatLookAt = CombatLookAt.position - new Vector3(transform.position.x, CombatLookAt.position.y, transform.position.z);
        Orientation.forward = dirToCombatLookAt.normalized; 
        PlayerObject.forward = dirToCombatLookAt.normalized;


    }

}
