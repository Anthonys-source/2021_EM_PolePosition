using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    #region Controller Variables and Events

    [Header("Movement")] public List<AxleInfo> axleInfos;
    public float forwardMotorTorque = 100000;
    public float backwardMotorTorque = 50000;
    public float maxSteeringAngle = 15;
    public float engineBrake = 1e+12f;
    public float footBrake = 1e+24f;
    public float topSpeed = 200f;
    public float downForce = 100f;
    public float slipLimit = 0.2f;

    private float currentRotation = 0.0f;
    private float inputAcceleration = 0.0f;
    private float inputSteering = 0.0f;
    private float inputBrake = 0.0f;

    private float m_SteerHelper = 0.8f;

    //Se guarda como int ya que solo se va a usar para actualizar la interfaz
    [SyncVar(hook = nameof(HandleSpeedUpdate))] private int m_CurrentSpeed = 0;
    public event Action<int> OnSpeedChangeEvent = delegate { };

    #endregion

    private PlayerInfo m_PlayerInfo;
    private ICarInputProvider m_CarInput;
    private Rigidbody m_Rigidbody;

    #region Unity Callbacks

    public void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_PlayerInfo = GetComponent<PlayerInfo>();
        m_CarInput = GetComponent<ICarInputProvider>();
    }

    public void Update()
    {
        if (isLocalPlayer)
        {
            CmdAccelerate(m_CarInput.AccelerateValue);
            CmdSteer(m_CarInput.SteerValue);
            CmdHandbrake(m_CarInput.HandbrakeValue);
        }

        if (isServer)
        {
            if (Math.Abs(m_CurrentSpeed - m_Rigidbody.velocity.magnitude) > float.Epsilon)
            {
                m_CurrentSpeed = (int)m_Rigidbody.velocity.magnitude;
            }
        }
    }

    private void HandleSpeedUpdate(int oldValue, int newValue)
    {
        if (OnSpeedChangeEvent != null)
            OnSpeedChangeEvent(newValue);
    }

    #region Input Commands and Server Methods

    [Server]
    public void SetAccelerationInput(float value)
    {
        inputAcceleration = value;
    }

    [Command]
    public void CmdAccelerate(float value)
    {
        SetAccelerationInput(value);
    }

    [Server]
    public void SetSteerInput(float value)
    {
        inputSteering = value;
    }

    [Command]
    public void CmdSteer(float value)
    {
        SetSteerInput(value);
    }

    [Server]
    public void SetHandbrakeInput(float value)
    {
        inputBrake = value;
    }

    [Command]
    public void CmdHandbrake(float value)
    {
        SetHandbrakeInput(value);
    }

    #endregion

    public void FixedUpdate()
    {
        // If you are not the Server or the race hasn't started yet dont calculate
        // the movement physics
        if (!isServer) return;
        if (!PolePositionManager.instance.raceStarted) return;

        // Clamp input values, this might be redundant
        inputSteering = Mathf.Clamp(inputSteering, -1, 1);
        inputAcceleration = Mathf.Clamp(inputAcceleration, -1, 1);
        inputBrake = Mathf.Clamp(inputBrake, 0, 1);

        float steering = maxSteeringAngle * inputSteering;

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }

            if (axleInfo.motor)
            {
                if (inputAcceleration > float.Epsilon)
                {
                    axleInfo.leftWheel.motorTorque = forwardMotorTorque;
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.motorTorque = forwardMotorTorque;
                    axleInfo.rightWheel.brakeTorque = 0;
                }

                if (inputAcceleration < -float.Epsilon)
                {
                    axleInfo.leftWheel.motorTorque = -backwardMotorTorque;
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.motorTorque = -backwardMotorTorque;
                    axleInfo.rightWheel.brakeTorque = 0;
                }

                if (Math.Abs(inputAcceleration) < float.Epsilon)
                {
                    axleInfo.leftWheel.motorTorque = 0;
                    axleInfo.leftWheel.brakeTorque = engineBrake;
                    axleInfo.rightWheel.motorTorque = 0;
                    axleInfo.rightWheel.brakeTorque = engineBrake;
                }

                if (inputBrake > 0)
                {
                    axleInfo.leftWheel.brakeTorque = footBrake;
                    axleInfo.rightWheel.brakeTorque = footBrake;
                }
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        SteerHelper();
        SpeedLimiter();
        AddDownForce();
        TractionControl();
    }

    #endregion

    #region Methods

    // crude traction control that reduces the power to wheel if the car is wheel spinning too much
    [Server]
    private void TractionControl()
    {
        foreach (var axleInfo in axleInfos)
        {
            WheelHit wheelHitLeft;
            WheelHit wheelHitRight;
            axleInfo.leftWheel.GetGroundHit(out wheelHitLeft);
            axleInfo.rightWheel.GetGroundHit(out wheelHitRight);

            if (wheelHitLeft.forwardSlip >= slipLimit)
            {
                var howMuchSlip = (wheelHitLeft.forwardSlip - slipLimit) / (1 - slipLimit);
                axleInfo.leftWheel.motorTorque -= axleInfo.leftWheel.motorTorque * howMuchSlip * slipLimit;
            }

            if (wheelHitRight.forwardSlip >= slipLimit)
            {
                var howMuchSlip = (wheelHitRight.forwardSlip - slipLimit) / (1 - slipLimit);
                axleInfo.rightWheel.motorTorque -= axleInfo.rightWheel.motorTorque * howMuchSlip * slipLimit;
            }
        }
    }

    // this is used to add more grip in relation to speed
    [Server]
    private void AddDownForce()
    {
        foreach (var axleInfo in axleInfos)
        {
            axleInfo.leftWheel.attachedRigidbody.AddForce(
                -transform.up * (downForce * axleInfo.leftWheel.attachedRigidbody.velocity.magnitude));
        }
    }

    [Server]
    private void SpeedLimiter()
    {
        float speed = m_Rigidbody.velocity.magnitude;
        if (speed > topSpeed)
            m_Rigidbody.velocity = topSpeed * m_Rigidbody.velocity.normalized;
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    [Server]
    public void ApplyLocalPositionToVisuals(WheelCollider col)
    {
        if (col.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = col.transform.GetChild(0);
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);
        var myTransform = visualWheel.transform;
        myTransform.position = position;
        myTransform.rotation = rotation;
    }

    [Server]
    private void SteerHelper()
    {
        foreach (var axleInfo in axleInfos)
        {
            WheelHit[] wheelHit = new WheelHit[2];
            axleInfo.leftWheel.GetGroundHit(out wheelHit[0]);
            axleInfo.rightWheel.GetGroundHit(out wheelHit[1]);
            foreach (var wh in wheelHit)
            {
                if (wh.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(currentRotation - transform.eulerAngles.y) < 10f)
        {
            var turnAdjust = (transform.eulerAngles.y - currentRotation) * m_SteerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnAdjust, Vector3.up);
            m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
        }

        currentRotation = transform.eulerAngles.y;
    }

    #endregion
}