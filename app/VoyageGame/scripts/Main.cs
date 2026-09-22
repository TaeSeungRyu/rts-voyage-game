using Godot;

public partial class Main : Node3D
{
    // --------------------------------------------------
    // World
    // --------------------------------------------------

    private WorldMap _worldMap = null!;


    // --------------------------------------------------
    // Camera
    // --------------------------------------------------

    private WorldCamera _worldCamera = null!;


    // --------------------------------------------------
    // Player
    // --------------------------------------------------

    private PlayerShip _playerShip = null!;


    // --------------------------------------------------
    // Ready
    // --------------------------------------------------

    public override void _Ready()
    {
        CreateEnvironment();

        CreateWorldMap();

        CreateWorldCamera();

        CreatePlayerShip();
    }


    // --------------------------------------------------
    // Environment
    // --------------------------------------------------

    private void CreateEnvironment()
    {
        WorldEnvironment worldEnvironment =
            new WorldEnvironment();


        Environment environment =
            new Environment();


        environment.BackgroundMode =
            Environment.BGMode.Color;

        environment.BackgroundColor =
            new Color(
                0.8f,
                0.9f,
                1.0f
            );


        environment.AmbientLightSource =
            Environment.AmbientSource.Color;

        environment.AmbientLightColor =
            Colors.White;

        environment.AmbientLightEnergy =
            0.8f;


        worldEnvironment.Environment =
            environment;


        AddChild(
            worldEnvironment
        );


        // --------------------------------------------------
        // Directional Light
        // --------------------------------------------------

        DirectionalLight3D light =
            new DirectionalLight3D();


        light.RotationDegrees =
            new Vector3(
                -60.0f,
                -30.0f,
                0.0f
            );


        light.LightEnergy =
            0.1f;


        AddChild(
            light
        );
    }


    // --------------------------------------------------
    // World Map
    // --------------------------------------------------

    private void CreateWorldMap()
    {
        _worldMap =
            new WorldMap();


        _worldMap.Name =
            "WorldMap";


        AddChild(
            _worldMap
        );
    }


    // --------------------------------------------------
    // Camera
    // --------------------------------------------------

    private void CreateWorldCamera()
    {
        _worldCamera =
            new WorldCamera();


        _worldCamera.Name =
            "WorldCamera";

        AddChild(
            _worldCamera
        );
    }


    // --------------------------------------------------
    // Player Ship
    // --------------------------------------------------

    private void CreatePlayerShip()
    {
        _playerShip =
            new PlayerShip();


        _playerShip.Name =
            "PlayerShip";


        // WorldMap 전달
        // 육지 충돌 검사에 사용
        _playerShip.SetWorldMap(
            _worldMap
        );


        // 사용할 배 선택
        //
        // ships.json의 ID만 전달한다.
        _playerShip.LoadShip(
            "merchant_ship"
        );


        // 시작 위치
        _playerShip.Position =
            new Vector3(
                15.0f,
                0.15f,
                5.0f
            );


        AddChild(
            _playerShip
        );
    }
}