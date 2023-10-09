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

    // Camera shake parameters
    [SerializeField] private float shakeAmplitude = 1f; // Shake intensity
    [SerializeField] private float shakeFrequency = 1f; // Shake speed

    private void Start()
    {
        particleSystem.SetActive(false);
        defaultFOV = vcam.m_Lens.FieldOfView;

        // Initialize noise parameters
        vcam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        vcam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        vcam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }

    private void Update()
    {
        float speedRatio = Mathf.InverseLerp(minFastSpeedThreshold, maxFastSpeedThreshold, playerMovement.Velocity);
        float targetFOV = Mathf.Lerp(minFastFOV, maxFastFOV, speedRatio);

        Debug.Log($"Target FOV: {targetFOV}, Speed Ratio: {speedRatio}");

        if (playerMovement.Velocity > minFastSpeedThreshold)
        {
            particleSystem.SetActive(true);
            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, targetFOV, Time.deltaTime * interpolationSpeed);

            // Add camera shake
            vcam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakeAmplitude * speedRatio;
            vcam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakeAmplitude * speedRatio;
            vcam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakeAmplitude * speedRatio;

            vcam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = shakeFrequency;
            vcam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = shakeFrequency;
            vcam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = shakeFrequency;
        }
        else
        {
            particleSystem.SetActive(false);
            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, defaultFOV, Time.deltaTime * interpolationSpeed);

            // Remove camera shake
            vcam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
            vcam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
            vcam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        }
    }

}
