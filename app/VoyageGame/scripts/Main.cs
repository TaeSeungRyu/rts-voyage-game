using Godot;

public partial class Main : Node3D
{
    private Camera3D _camera = null!;

    // rts-slg의 실제 타일 크기에 따라 이 값만 조정하면 됨.
    private const float TileSize = 1.0f;

    private float _yaw = -35.0f;
    private float _pitch = -50.0f;
    private float _distance = 30.0f;

    private Vector3 _focus = Vector3.Zero;

    private bool _rotating = false;

    // 0 = 바다
    // 1 = 육지
    private readonly int[,] _map =
    {
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0 },
        { 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0 },
        { 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
        { 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
        { 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0 },
        { 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
    };


    public override void _Ready()
    {
        BuildEnvironment();

        BuildMap();

        BuildCamera();

        UpdateCamera();
    }


    public override void _Process(double delta)
    {
        float speed = 10.0f * (float)delta;

        Vector3 move = Vector3.Zero;

        if (Input.IsKeyPressed(Key.W))
            move.Z -= 1;

        if (Input.IsKeyPressed(Key.S))
            move.Z += 1;

        if (Input.IsKeyPressed(Key.A))
            move.X -= 1;

        if (Input.IsKeyPressed(Key.D))
            move.X += 1;


        if (move != Vector3.Zero)
        {
            move = move.Normalized();

            _focus += move * speed;

            UpdateCamera();
        }
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            // 우클릭
            if (mouseButton.ButtonIndex == MouseButton.Right)
            {
                _rotating = mouseButton.Pressed;
            }


            // 줌 인
            if (
                mouseButton.Pressed &&
                mouseButton.ButtonIndex == MouseButton.WheelUp
            )
            {
                _distance = Mathf.Max(
                    10.0f,
                    _distance - 2.0f
                );

                UpdateCamera();
            }


            // 줌 아웃
            if (
                mouseButton.Pressed &&
                mouseButton.ButtonIndex == MouseButton.WheelDown
            )
            {
                _distance = Mathf.Min(
                    60.0f,
                    _distance + 2.0f
                );

                UpdateCamera();
            }
        }


        // 우클릭 드래그
        if (
            @event is InputEventMouseMotion motion &&
            _rotating
        )
        {
            _yaw -= motion.Relative.X * 0.25f;

            _pitch = Mathf.Clamp(
                _pitch - motion.Relative.Y * 0.20f,
                -75.0f,
                -20.0f
            );

            UpdateCamera();
        }
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
                new Color(0.55f, 0.75f, 0.86f),

            AmbientLightSource =
                Godot.Environment.AmbientSource.Color,

            AmbientLightColor =
                new Color(0.85f, 0.88f, 0.82f),

            AmbientLightEnergy = 0.8f
        };

        worldEnvironment.Environment = environment;

        AddChild(worldEnvironment);


        // 태양
        var sun = new DirectionalLight3D
        {
            RotationDegrees =
                new Vector3(-55, -35, 0),

            ShadowEnabled = true,

            LightEnergy = 1.2f
        };

        AddChild(sun);
    }


    // --------------------------------------------------
    // 맵 생성
    // --------------------------------------------------

    private void BuildMap()
    {
        int rows = _map.GetLength(0);
        int columns = _map.GetLength(1);


        for (int z = 0; z < rows; z++)
        {
            for (int x = 0; x < columns; x++)
            {
                int tileType = _map[z, x];
                Vector3 position = new Vector3(
                    x * TileSize,
                    0,
                    z * TileSize
                );

                if (tileType == 0)
                {
                    CreateTile(
                        "res://assets/models/water.glb",
                        position
                    );
                }
                else
                {
                    CreateTile(
                        "res://assets/models/grass.glb",
                        position
                    );
                }
            }
        }
    }


    // --------------------------------------------------
    // GLB 타일 생성
    // --------------------------------------------------

    private void CreateTile(
        string path,
        Vector3 position
    )
    {
        PackedScene scene =
            GD.Load<PackedScene>(path);


        if (scene == null)
        {
            GD.PrintErr(
                $"타일을 불러올 수 없습니다: {path}"
            );

            return;
        }


        Node3D tile =
            scene.Instantiate<Node3D>();


        tile.Position = position;


        AddChild(tile);
    }


    // --------------------------------------------------
    // 카메라
    // --------------------------------------------------

    private void BuildCamera()
    {
        _camera = new Camera3D
        {
            Current = true,

            Fov = 50.0f
        };


        AddChild(_camera);


        // 맵 중앙을 바라보게 함
        int rows = _map.GetLength(0);
        int columns = _map.GetLength(1);


        _focus = new Vector3(
            (columns - 1) * TileSize / 2.0f,
            0,
            (rows - 1) * TileSize / 2.0f
        );
    }


    private void UpdateCamera()
    {
        float yaw =
            Mathf.DegToRad(_yaw);

        float pitch =
            Mathf.DegToRad(_pitch);


        Vector3 direction = new Vector3(
            Mathf.Cos(pitch) * Mathf.Sin(yaw),

            -Mathf.Sin(pitch),

            Mathf.Cos(pitch) * Mathf.Cos(yaw)
        );


        _camera.Position =
            _focus + direction * _distance;


        _camera.LookAt(
            _focus,
            Vector3.Up
        );
    }
}