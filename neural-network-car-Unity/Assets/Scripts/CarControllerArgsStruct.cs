// De här parametrarna (orkar inte ändra på namnet av structen) behövde passeras flera gånger till samma ställe så jag satte
// ihop dem till en struct så de inte skulle behöva skrivas ut så många gånger

[System.Serializable] public struct CarControllerArgs{
    public float acceleration;
    public float maxSpeed;
    public float rotationSpeed;
    public float deceleration;
    public float maxRotationRadius;
}