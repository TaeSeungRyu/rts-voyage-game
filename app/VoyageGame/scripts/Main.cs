using Godot;

public partial class Main : Node3D
{
    private WorldCamera _worldCamera = null!;

    private WorldMap _worldMap = null!;



    public override void _Ready()
    {
        BuildEnvironment();

        _worldMap = new WorldMap();
                AddChild(_worldMap);
        _worldCamera = new WorldCamera();
    }



    // --------------------------------------------------
    // 환경
    // --------------------------------------------------

    private void BuildEnvironment()
    {
        var worldEnvironment = new WorldEnvironment();

        var environment = new Godot.Environment
        {
            BackgroundMode = Godot.Environment.BGMode.Color,
            BackgroundColor = Colors.White,
            AmbientLightSource = Godot.Environment.AmbientSource.Color,
            AmbientLightColor = new Color(0.78f, 0.78f, 0.82f),
            AmbientLightEnergy = 0.9f
        };
        worldEnvironment.Environment = environment;
        AddChild(worldEnvironment);

        var sun = new DirectionalLight3D
        {
            RotationDegrees = new Vector3(-55, -35, 0),
            // 기존 1.2f → 낮춰보기
            LightEnergy = 0.1f,
            ShadowEnabled = true
        };
        AddChild(sun);
    }
}