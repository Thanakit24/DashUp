using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    [SerializeField] private CinemachineVirtualCamera cinemcam;
    [SerializeField] private float shakeIntensity = 1f;
    [SerializeField] private float shakeTime = 0.2f;
    private float shakeTimer;
    private CinemachineBasicMultiChannelPerlin cinemcamPerlin;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
        cinemcam = GetComponent<CinemachineVirtualCamera>();
        cinemcamPerlin = cinemcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    private void Start()
    {
        StopShake();
    }
    public void ShakeCamera()
    {
        cinemcamPerlin.m_AmplitudeGain = shakeIntensity;
        shakeTimer = shakeTime;
    }

    public void StopShake()
    {
        cinemcamPerlin.m_AmplitudeGain = 0f;
        shakeTimer = 0f;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    ShakeCamera();
        //}

        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                StopShake();
            }
        }
    }
}
