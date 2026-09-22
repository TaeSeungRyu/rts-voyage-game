public class ShipStats
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Model { get; set; } = "";
    public float MaxForwardSpeed { get; set; }
    public float MaxBackwardSpeed { get; set; }
    public float ForwardAcceleration { get; set; }
    public float BackwardAcceleration { get; set; }
    public float Deceleration { get; set; }
    public float TurnSpeed { get; set; }
    public float CollisionLength { get; set; }
    public float CollisionWidth { get; set; }
}