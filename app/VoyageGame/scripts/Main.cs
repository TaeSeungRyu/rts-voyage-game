using Godot;

public partial class Main : Node3D
{
    private Camera3D _camera = null!;
    private float _yaw = -35.0f;
    private float _pitch = -48.0f;
    private float _distance = 38.0f;
    private Vector3 _focus = new(0, 0, 0);
    private bool _rotating;

    public override void _Ready()
    {
        BuildWorld();
        BuildCamera();
        BuildLight();
        BuildUi();
        UpdateCamera();
    }

    public override void _Process(double delta)
    {
        float speed = 14.0f * (float)delta;
        Vector3 move = Vector3.Zero;

        if (Input.IsKeyPressed(Key.W)) move.Z -= 1;
        if (Input.IsKeyPressed(Key.S)) move.Z += 1;
        if (Input.IsKeyPressed(Key.A)) move.X -= 1;
        if (Input.IsKeyPressed(Key.D)) move.X += 1;

        if (move != Vector3.Zero)
        {
            move = move.Normalized();
            _focus += new Vector3(move.X, 0, move.Z) * speed;
            UpdateCamera();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Right)
                _rotating = mb.Pressed;

            if (mb.Pressed && mb.ButtonIndex == MouseButton.WheelUp)
            {
                _distance = Mathf.Max(12, _distance - 3);
                UpdateCamera();
            }

            if (mb.Pressed && mb.ButtonIndex == MouseButton.WheelDown)
            {
                _distance = Mathf.Min(70, _distance + 3);
                UpdateCamera();
            }
        }

        if (@event is InputEventMouseMotion motion && _rotating)
        {
            _yaw -= motion.Relative.X * 0.25f;
            _pitch = Mathf.Clamp(_pitch - motion.Relative.Y * 0.20f, -75, -20);
            UpdateCamera();
        }
    }

    private void BuildWorld()
    {
        var envNode = new WorldEnvironment();
        var environment = new Godot.Environment
        {
            BackgroundMode = Godot.Environment.BGMode.Color,
            BackgroundColor = new Color(0.55f, 0.75f, 0.86f),
            AmbientLightSource = Godot.Environment.AmbientSource.Color,
            AmbientLightColor = new Color(0.85f, 0.88f, 0.82f),
            AmbientLightEnergy = 0.85f
        };
        envNode.Environment = environment;
        AddChild(envNode);

        // Sea
        AddBox("Sea", new Vector3(70, 0.5f, 55), new Vector3(0, -0.5f, 0),
            new Color(0.10f, 0.43f, 0.58f));

        // Main land masses
        AddLand(new Vector3(-15, 0.25f, -4), new Vector3(25, 1.0f, 31), new Color(0.38f, 0.62f, 0.30f));
        AddLand(new Vector3(15, 0.25f, 8), new Vector3(21, 1.0f, 22), new Color(0.44f, 0.67f, 0.32f));
        AddLand(new Vector3(23, 0.25f, -16), new Vector3(8, 1.0f, 9), new Color(0.47f, 0.69f, 0.34f));

        // Sandy coast strips / islands
        AddBox("Beach", new Vector3(5, 0.22f, 12), new Vector3(-2, 0.12f, 7), new Color(0.78f, 0.70f, 0.43f));
        AddBox("Island", new Vector3(5, 0.65f, 4), new Vector3(4, 0.05f, -13), new Color(0.43f, 0.65f, 0.31f));

        // Mountains
        AddMountain(new Vector3(-20, 1.0f, -9), 3.3f, 5.5f);
        AddMountain(new Vector3(-16, 1.0f, -5), 2.8f, 4.7f);
        AddMountain(new Vector3(-12, 1.0f, -10), 2.4f, 4.1f);
        AddMountain(new Vector3(17, 1.0f, 7), 3.0f, 5.0f);
        AddMountain(new Vector3(21, 1.0f, 11), 2.4f, 4.0f);

        // Forests
        for (int i = 0; i < 12; i++)
        {
            float x = -23 + (i % 4) * 3.1f;
            float z = 3 + (i / 4) * 3.2f;
            AddTree(new Vector3(x, 0.8f, z));
        }

        for (int i = 0; i < 7; i++)
            AddTree(new Vector3(10 + (i % 3) * 3.0f, 0.8f, 12 + (i / 3) * 3.0f));

        // Port
        Vector3 port = new(-3.8f, 0.7f, 4.0f);
        AddBox("PortTown", new Vector3(3.2f, 1.4f, 2.5f), port, new Color(0.72f, 0.43f, 0.23f));
        AddBox("Roof", new Vector3(3.6f, 0.5f, 2.9f), port + new Vector3(0, 0.95f, 0), new Color(0.52f, 0.18f, 0.12f));
        AddBox("Pier", new Vector3(1.2f, 0.22f, 5.0f), new Vector3(-1.0f, 0.18f, 6.7f), new Color(0.35f, 0.20f, 0.10f));

        // Ship
        AddShip(new Vector3(3.0f, 0.25f, 10.5f));
    }

    private void AddLand(Vector3 pos, Vector3 size, Color color)
    {
        AddBox("Land", size, pos, color);
    }

    private void AddMountain(Vector3 pos, float radius, float height)
    {
        var mesh = new CylinderMesh
        {
            TopRadius = 0.0f,
            BottomRadius = radius,
            Height = height,
            RadialSegments = 6
        };
        var node = NewMeshNode("Mountain", mesh, new Color(0.33f, 0.31f, 0.25f));
        node.Position = pos + new Vector3(0, height / 2.0f, 0);
        AddChild(node);
    }

    private void AddTree(Vector3 pos)
    {
        AddBox("Trunk", new Vector3(0.35f, 1.1f, 0.35f), pos, new Color(0.28f, 0.16f, 0.07f));

        var mesh = new CylinderMesh
        {
            TopRadius = 0,
            BottomRadius = 1.0f,
            Height = 2.4f,
            RadialSegments = 6
        };
        var crown = NewMeshNode("Tree", mesh, new Color(0.15f, 0.43f, 0.17f));
        crown.Position = pos + new Vector3(0, 1.6f, 0);
        AddChild(crown);
    }

    private void AddShip(Vector3 pos)
    {
        var ship = new Node3D { Name = "PlayerShip", Position = pos };
        AddChild(ship);

        var hullMesh = new BoxMesh { Size = new Vector3(1.5f, 0.55f, 4.0f) };
        var hull = NewMeshNode("Hull", hullMesh, new Color(0.30f, 0.14f, 0.06f));
        hull.Position = new Vector3(0, 0.45f, 0);
        ship.AddChild(hull);

        var mastMesh = new CylinderMesh
        {
            TopRadius = 0.08f,
            BottomRadius = 0.10f,
            Height = 4.2f,
            RadialSegments = 6
        };
        var mast = NewMeshNode("Mast", mastMesh, new Color(0.25f, 0.14f, 0.07f));
        mast.Position = new Vector3(0, 2.5f, 0);
        ship.AddChild(mast);

        var sailMesh = new QuadMesh { Size = new Vector2(2.8f, 2.8f) };
        var sail = NewMeshNode("Sail", sailMesh, new Color(0.92f, 0.88f, 0.70f));
        sail.Position = new Vector3(0, 2.6f, 0.05f);
        sail.RotationDegrees = new Vector3(0, 0, 0);
        ship.AddChild(sail);
    }

    private MeshInstance3D NewMeshNode(string name, Mesh mesh, Color color)
    {
        var material = new StandardMaterial3D
        {
            AlbedoColor = color,
            Roughness = 0.9f
        };

        return new MeshInstance3D
        {
            Name = name,
            Mesh = mesh,
            MaterialOverride = material
        };
    }

    private void AddBox(string name, Vector3 size, Vector3 pos, Color color)
    {
        var mesh = new BoxMesh { Size = size };
        var node = NewMeshNode(name, mesh, color);
        node.Position = pos;
        AddChild(node);
    }

    private void BuildCamera()
    {
        _camera = new Camera3D
        {
            Name = "Camera3D",
            Current = true,
            Fov = 50
        };
        AddChild(_camera);
    }

    private void BuildLight()
    {
        var light = new DirectionalLight3D
        {
            Name = "Sun",
            RotationDegrees = new Vector3(-55, -35, 0),
            ShadowEnabled = true,
            LightEnergy = 1.25f
        };
        AddChild(light);
    }

    private void BuildUi()
    {
        var canvas = new CanvasLayer();
        AddChild(canvas);

        var panel = new ColorRect
        {
            Position = new Vector2(18, 18),
            Size = new Vector2(390, 88),
            Color = new Color(0.05f, 0.07f, 0.08f, 0.78f)
        };
        canvas.AddChild(panel);

        var title = new Label
        {
            Text = "RTS VOYAGE - LOW POLY WORLD",
            Position = new Vector2(18, 12),
            Size = new Vector2(350, 30)
        };
        title.AddThemeFontSizeOverride("font_size", 20);
        panel.AddChild(title);

        var help = new Label
        {
            Text = "WASD: Move camera   Mouse Wheel: Zoom   RMB Drag: Rotate",
            Position = new Vector2(18, 49),
            Size = new Vector2(360, 25)
        };
        help.AddThemeFontSizeOverride("font_size", 13);
        panel.AddChild(help);
    }

    private void UpdateCamera()
    {
        float yawRad = Mathf.DegToRad(_yaw);
        float pitchRad = Mathf.DegToRad(_pitch);

        Vector3 offset = new(
            Mathf.Cos(pitchRad) * Mathf.Sin(yawRad),
            -Mathf.Sin(pitchRad),
            Mathf.Cos(pitchRad) * Mathf.Cos(yawRad)
        );

        _camera.Position = _focus + offset * _distance;
        _camera.LookAt(_focus, Vector3.Up);
    }
}
