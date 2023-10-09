using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedFeelBehaviour : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject particleSystem;
    [SerializeField] private CinemachineFreeLook vcam;

    private float defaultFOV;

    [Header("Params")]
    [SerializeField] private float minFastFOV = 60;
    [SerializeField] private float maxFastFOV = 120;
    [SerializeField] private float minFastSpeedThreshold = 8;
    [SerializeField] private float maxFastSpeedThreshold = 12;
    [SerializeField] private float interpolationSpeed = 5;

    private void Start()
    {
        particleSystem.SetActive(false);
        defaultFOV = vcam.m_Lens.FieldOfView;
    }

    private void Update()
    {
        float speedRatio = Mathf.InverseLerp(minFastSpeedThreshold, maxFastSpeedThreshold, playerMovement.MoveSpeed);
        float targetFOV = Mathf.Lerp(minFastFOV, maxFastFOV, speedRatio);

        Debug.Log($"Target FOV: {targetFOV}, Speed Ratio: {speedRatio}");

        if (playerMovement.MoveSpeed > minFastSpeedThreshold)
        {
            particleSystem.SetActive(true);
            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, targetFOV, Time.deltaTime * interpolationSpeed);
        }
        else
        {
            particleSystem.SetActive(false);
            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, defaultFOV, Time.deltaTime * interpolationSpeed);
        }
    }

}
