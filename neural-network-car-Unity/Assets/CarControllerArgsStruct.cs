using System;

[Serializable] public struct CarControllerArgs{
    public float acceleration;
    public float maxSpeed;
    public float rotationSpeed;
    public float deceleration;
    public float maxRotationRadius;
}