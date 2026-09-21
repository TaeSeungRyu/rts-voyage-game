using Godot;

public partial class WorldCamera : Node3D
{
    private Camera3D _camera = null!;
    private WorldMap _worldMap = null!;
    private Vector3 _focus = Vector3.Zero;
    private float _yaw = -35.0f;
    private float _pitch = -50.0f;
    private float _distance = 30.0f;    
    private bool _rotating = false;


   public override void _Ready()
   {
        _worldMap = GetParent().GetNode<WorldMap>("WorldMap");    
        _camera = new Camera3D
        {
            Current = true,
            Fov = 50.0f
        };
        AddChild(_camera);
        // 맵 중앙을 바라보게 함
        _focus = _worldMap.GetCenter();
        // ★ 이 부분이 중요
        _camera.Position = _focus + new Vector3(
            0,
            10,
            10
        );

        // ★ 맵 중앙을 바라봄
        _camera.LookAt(_focus);

        GD.Print($"Camera Position: {_camera.GlobalPosition}");
        GD.Print($"Map Focus: {_focus}");        
   }

    public void UpdateCamera()
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

}