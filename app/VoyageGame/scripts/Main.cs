using Godot;

public partial class Main : Node3D
{
    private WorldMap _worldMap = null!;
    private WorldCamera _worldCamera = null!;

    public override void _Ready()
    {
        BuildEnvironment();

        // 월드맵 생성
        _worldMap = new WorldMap
        {
            Name = "WorldMap"
        };
        AddChild(_worldMap);
        // 월드 카메라 생성
        _worldCamera = new WorldCamera
        {
            Name = "WorldCamera"
        };
        AddChild(_worldCamera);
    }


    // --------------------------------------------------
    // 환경
    // --------------------------------------------------

    private void BuildEnvironment()
    {
        var worldEnvironment = new WorldEnvironment();
        var environment = new Godot.Environment
        {
            BackgroundMode =
                Godot.Environment.BGMode.Color,
            BackgroundColor =
                Colors.White,
            AmbientLightSource =
                Godot.Environment.AmbientSource.Color,
            AmbientLightColor =
                new Color(0.78f, 0.78f, 0.82f),
            AmbientLightEnergy =
                0.9f
        };
        worldEnvironment.Environment =
            environment;
        AddChild(worldEnvironment);
        var sun = new DirectionalLight3D
        {
            RotationDegrees = new Vector3(-55, -35, 0),
            LightEnergy = 0.1f,
            ShadowEnabled = true
        };
        AddChild(sun);
    }
}