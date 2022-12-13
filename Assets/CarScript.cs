using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CarScript : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentBreakForce;
    private bool isBreaking;
    private GameObject mainCamera;

    private Rigidbody _rb;
    private float currentSpeed;
    private float currentPitch;

    private float minPitch = 0.75f;
    private float maxPitch = 2f;
    private float minSpeed = 0.3f;
    private float maxSpeed = 40f;

    [Space(30)]
    [Header("Konfigurace kamery")]
    public Vector3 cameraOffset;
    public float cameraTranslateSpeed;
    public float cameraRotationSpeed;

    [Space(30)]
    [Header("Nastavení auta")]
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [Space(30)]
    [Header("Levé pøední kolo")]
    [SerializeField] private WheelCollider leftFrontCollider;
    [SerializeField] private Transform leftFrontTransform;

    [Space(30)]
    [Header("Pravé pøední kolo")]
    [SerializeField] private WheelCollider rightFrontCollider;
    [SerializeField] private Transform rightFrontTransform;

    [Space(30)]
    [Header("Levé zadní kolo")]
    [SerializeField] private WheelCollider leftRearCollider;
    [SerializeField] private Transform leftRearTransform;

    [Space(30)]
    [Header("Pravé zadní kolo")]
    [SerializeField] private WheelCollider rightRearCollider;
    [SerializeField] private Transform rightRearTransform;

    [Space(30)]
    [Header("Sound settings")]
    [SerializeField] private AudioSource engineSound;

    [SerializeField] private AudioSource dramaticSound;

    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        _rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleCameraTranslation();
        HandleCameraRotation();
        GetInput();
        Handlemotor();
        HandleSteering();
        UpdateWheels();
    }

    private void Update()
    {
        EngineSound();
    }

    private void EngineSound()
    {
        currentSpeed = Mathf.Abs(_rb.velocity.magnitude);
        currentPitch = Mathf.Abs(currentSpeed / 50f * verticalInput);
        if (currentSpeed < minSpeed)
        {
            engineSound.pitch = minPitch;
        }
        if (currentSpeed > minSpeed && currentSpeed < maxSpeed)
        {
            engineSound.pitch = minPitch + currentPitch;
        }
        if (currentSpeed > maxSpeed)
        {
            engineSound.pitch = maxPitch;
        }

    }
    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }
    private void Handlemotor()
    {
        leftFrontCollider.motorTorque = verticalInput * motorForce;
        rightFrontCollider.motorTorque = verticalInput * motorForce;
        currentBreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }
    private void ApplyBreaking()
    {
        leftFrontCollider.brakeTorque = currentBreakForce;
        leftRearCollider.brakeTorque = currentBreakForce;
        rightFrontCollider.brakeTorque = currentBreakForce;
        rightRearCollider.brakeTorque = currentBreakForce;
    }
    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        leftFrontCollider.steerAngle = currentSteerAngle;
        rightFrontCollider.steerAngle = currentSteerAngle;
    }
    private void UpdateWheels()
    {
        UpdateSingleWheel(leftFrontCollider, leftFrontTransform);
        UpdateSingleWheel(leftRearCollider, leftRearTransform);
        UpdateSingleWheel(rightFrontCollider, rightFrontTransform);
        UpdateSingleWheel(rightRearCollider, rightRearTransform);
    }
    private void UpdateSingleWheel(WheelCollider wheelColl, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelColl.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
    private void HandleCameraTranslation()
    {
        var targetPosition = this.transform.TransformPoint(cameraOffset);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraTranslateSpeed * Time.deltaTime);
    }
    private void HandleCameraRotation()
    {
        var direction = this.transform.position - mainCamera.transform.position;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, rotation, cameraRotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish")
        {
            Destroy(other.gameObject);
            int x = UnityEngine.Random.Range(-20, 20);
            int z = UnityEngine.Random.Range(-20, 20);
            Vector3 pos = new Vector3(x, 0.5f, z);
            GameObject obstacle = Instantiate(other.gameObject, pos, Quaternion.Euler(0, 0, 0));
            obstacle.name = "Obstacle";


        }

    }





}