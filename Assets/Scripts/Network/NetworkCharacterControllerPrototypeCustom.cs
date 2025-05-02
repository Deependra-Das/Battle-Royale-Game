using System;    
using Fusion;    
using UnityEngine;    


[RequireComponent(typeof(CharacterController))]
[DisallowMultipleComponent] 
public class NetworkCharacterControllerPrototypeCustom : NetworkBehaviour
{
    [Header("Character Controller Settings")]
    public float gravity = -20.0f;
    public float jumpImpulse = 8.0f;
    public float acceleration = 10.0f;
    public float braking = 10.0f;
    public float maxSpeed = 2.0f;
    public float rotationSpeed = 15.0f;
    public float verticalRotationSpeed = 50.0f;


    [Networked]
    [HideInInspector]
    public bool IsGrounded { get; set; }


    [Networked]
    [HideInInspector]
    public Vector3 Velocity { get; set; }


    public CharacterController Controller { get; private set; }


    void Awake()
    {
        CacheController();
    }


    public override void Spawned()
    {
        base.Spawned();
        CacheController();
    }


    private void CacheController()
    {
        if (Controller == null)
        {
            Controller = GetComponent<CharacterController>();
        }
    }

  
    public virtual void Jump(bool ignoreGrounded = false, float? overrideImpulse = null)
    {
        if (IsGrounded || ignoreGrounded)
        {
            var newVel = Velocity;
            newVel.y += overrideImpulse ?? jumpImpulse;
            Velocity = newVel;
        }
    }


    public void Rotate(float rotationValue)
    {
        transform.Rotate(0, rotationValue * Runner.DeltaTime * rotationSpeed, 0);
    }

  
    public virtual void Move(Vector3 direction)
    {
        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Velocity;


        direction = direction.normalized;


        if (IsGrounded && moveVelocity.y < 0)
        {
            moveVelocity.y = 0f;
        }


        moveVelocity.y += gravity * Runner.DeltaTime;


        var horizontalVel = default(Vector3);
        horizontalVel.x = moveVelocity.x;
        horizontalVel.z = moveVelocity.z;


        if (direction == default)
        {
            Debug.Log("direction == Default");
            horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
        }
        else
        {
            Debug.Log("direction else");
            Debug.Log($"HorizontalVel {horizontalVel} + direcction {direction} * deltaTime {deltaTime}, maxSpeed {maxSpeed}");
            horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * acceleration * deltaTime, maxSpeed);
            // LEFT RIGHT Movements will affect the rotation so commenting out is desirable    
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Runner.DeltaTime);    
        }


        moveVelocity.x = horizontalVel.x;
        moveVelocity.z = horizontalVel.z;
        Controller.Move(moveVelocity * deltaTime);


        //Debug.Log($"current pos {transform.position} - previous pos {previousPos} * runner tickrate {Runner.Simulation.Config.TickRate}");
        //Debug.Log($"current pos {transform.position} - previous pos {previousPos} = {transform.position - previousPos}");


        Vector3 simulatedVelocity = (transform.position - previousPos); // * Runner.Simulation.Config.TickRate;
        //simulatedVelocity.y = simulatedVelocity.y * 0.5f;    
        Velocity = simulatedVelocity;
        Debug.Log("Velocity:: " + Velocity);
        IsGrounded = Controller.isGrounded;
    }
}
