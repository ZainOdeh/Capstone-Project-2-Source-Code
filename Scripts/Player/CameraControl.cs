using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    [Header("Camera")]
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 90.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -90.0f;

    [Header("Player")]
    [Tooltip("Rotation speed of the character")]
    public float RotationSpeed = 10.0f;

    [Header("Look Back")]
    [Tooltip("Key to hold for looking back")]
    public KeyCode LookBackKey = KeyCode.R;
    [Tooltip("Speed of the look back rotation")]
    public float LookBackSpeed = 30.0f;

    private bool isLookingBack = false;

    //Other
    private PlayerInput _input;

    //Camera
    private float xRotation;
    private float yRotation;

    public CinemachineVirtualCamera virtualCamera; // Reference to the Cinemachine Virtual Camera
    private CinemachineBasicMultiChannelPerlin noiseComponent; // The noise component of the virtual camera

    public float transitionSpeed = 0.5f; // Speed at which the noise effect transitions

    private float currentAmplitude = 0.0f; // Current amplitude of the noise
    private float currentFrequency = 0.0f; // Current frequency of the noise




    private void Start() {
        _input = GetComponent<PlayerInput>();

        // Get the noise component of the virtual camera
        noiseComponent = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    }

    private void Update() {
        HandleLookBack();
        CameraRotation();
        UpdateWobbleTransition();
    }

    private void HandleLookBack() {
        if (Input.GetKey(LookBackKey)) {
            isLookingBack = true;
        } else {
            isLookingBack = false;
        }
    }

    private void CameraRotation() {
        float deltaTimeMultiplier = Time.deltaTime * 10;
        xRotation += -_input.look.y * RotationSpeed * deltaTimeMultiplier;
        yRotation = _input.look.x * RotationSpeed * deltaTimeMultiplier;

        xRotation = ClampAngle(xRotation, BottomClamp, TopClamp);


        //New - Faris
        Quaternion targetRotation = isLookingBack ? Quaternion.Euler(xRotation, 180.0f, 0.0f) : Quaternion.Euler(xRotation, 0.0f, 0.0f);

        virtualCamera.transform.localRotation = Quaternion.Slerp(virtualCamera.transform.localRotation, targetRotation, Time.deltaTime * LookBackSpeed);

        //Old, i fixed it - Faris
        //virtualCamera.transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);

        //if (isLookingBack) {
        //    // Rotate the camera to look back
        //    virtualCamera.transform.localRotation = Quaternion.Slerp(virtualCamera.transform.localRotation, Quaternion.Euler(xRotation, 180.0f, 0.0f), Time.deltaTime * LookBackSpeed);
        //} else {
        //    // Normal rotation
        //    virtualCamera.transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
        //}

        transform.Rotate(Vector3.up * yRotation);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }


    public void CameraWobble(bool status, float value) {
        if (status == true) {
            currentAmplitude = value;
            currentFrequency = value;
        }

    }

    private void UpdateWobbleTransition() {
        // Smoothly transition the amplitude and frequency
        currentAmplitude = Mathf.Lerp(currentAmplitude, 1, Time.deltaTime * transitionSpeed);
        currentFrequency = Mathf.Lerp(currentFrequency, 1, Time.deltaTime * transitionSpeed);
        // Apply the interpolated values to the noise component
        noiseComponent.m_AmplitudeGain = currentAmplitude;
        noiseComponent.m_FrequencyGain = currentFrequency;
    }



}
