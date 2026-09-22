using Godot;

public partial class Main : Node3D
{
    private WorldMap _worldMap = null!;
    private WorldCamera _worldCamera = null!;
    private PlayerShip _playerShip = null!;

    public override void _Ready()
    {
        BuildEnvironment();

        // 월드맵 생성
        _worldMap = new WorldMap{ Name = "WorldMap" };
        AddChild(_worldMap);
        // 월드 카메라 생성
        _worldCamera = new WorldCamera { Name = "WorldCamera" };
        AddChild(_worldCamera);

        CreatePlayerShip();
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

    private void CreatePlayerShip()
    {
        PackedScene shipScene =
            GD.Load<PackedScene>(
                "res://assets/models/troop-medium-ship.glb"
            );


        Node3D shipModel =
            shipScene.Instantiate<Node3D>();


        _playerShip =
            new PlayerShip();   

        _playerShip.Name =
            "PlayerShip";

        _playerShip.SetWorldMap(
            _worldMap
        );                    

        // 처음 시작할 위치
        _playerShip.Position =
            new Vector3(
                15.0f,
                0.15f,
                5.0f
            );


        _playerShip.AddChild(
            shipModel
        );


        AddChild(
            _playerShip
        );
    }    
}