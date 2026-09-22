using Godot;

public partial class PlayerShip : Node3D
{
    // --------------------------------------------------
    // 이동 설정
    // --------------------------------------------------

    // 최대 속도
    private const float MaxSpeed = 3.0f;

    // 가속도
    private const float Acceleration = 1.8f;

    // 자연 감속
    private const float Deceleration = 0.6f;

    // S 키 감속
    private const float BrakePower = 2.5f;

    // 좌우 선회 속도
    private const float TurnSpeed = 1.5f;


    // --------------------------------------------------
    // 상태
    // --------------------------------------------------

    private float _currentSpeed = 0.0f;

    private WorldMap _worldMap = null!;


    // --------------------------------------------------
    // WorldMap 연결
    // --------------------------------------------------

    public void SetWorldMap(
        WorldMap worldMap
    )
    {
        _worldMap = worldMap;
    }


    // --------------------------------------------------
    // Process
    // --------------------------------------------------

    public override void _Process(double delta)
    {
        float dt =
            (float)delta;

        HandleSpeed(dt);

        HandleRotation(dt);

        MoveShip(dt);
    }


    // --------------------------------------------------
    // 속도 처리
    // --------------------------------------------------

    private void HandleSpeed(
        float delta
    )
    {
        bool forward =
            Input.IsKeyPressed(Key.W)
            || Input.IsKeyPressed(Key.Up);

        bool brake =
            Input.IsKeyPressed(Key.S)
            || Input.IsKeyPressed(Key.Down);


        // W : 가속
        if (forward)
        {
            _currentSpeed +=
                Acceleration
                * delta;
        }

        // S : 감속
        else if (brake)
        {
            _currentSpeed -=
                BrakePower
                * delta;
        }

        // 아무것도 누르지 않으면 자연 감속
        else
        {
            _currentSpeed -=
                Deceleration
                * delta;
        }


        _currentSpeed =
            Mathf.Clamp(
                _currentSpeed,
                0.0f,
                MaxSpeed
            );
    }


    // --------------------------------------------------
    // 선회
    // --------------------------------------------------

    private void HandleRotation(
        float delta
    )
    {
        // 완전히 정지해 있으면 선회하지 않는다.
        if (_currentSpeed <= 0.01f)
        {
            return;
        }


        float turnInput =
            0.0f;


        // A : 좌회전
        if (Input.IsKeyPressed(Key.A)
            || Input.IsKeyPressed(Key.Left))
        {
            turnInput += 1.0f;
        }


        // D : 우회전
        if (Input.IsKeyPressed(Key.D)
            || Input.IsKeyPressed(Key.Right))
        {
            turnInput -= 1.0f;
        }


        if (turnInput == 0.0f)
        {
            return;
        }


        RotateY(
            turnInput
            * TurnSpeed
            * delta
        );
    }


    // --------------------------------------------------
    // 이동
    // --------------------------------------------------
    private void MoveShip(
        float delta
    )
    {
        if (_currentSpeed <= 0.0f)
        {
            return;
        }


        // --------------------------------------------------
        // 배 방향
        // --------------------------------------------------

        Vector3 forward =
            -GlobalTransform.Basis.Z;

        forward.Y =
            0.0f;

        forward =
            forward.Normalized();


        Vector3 right =
            GlobalTransform.Basis.X;

        right.Y =
            0.0f;

        right =
            right.Normalized();


        // --------------------------------------------------
        // 다음 위치
        // --------------------------------------------------

        Vector3 nextPosition =
            Position
            + forward
            * _currentSpeed
            * delta;


        // --------------------------------------------------
        // 충돌 검사 위치
        // --------------------------------------------------

        Vector3 front =
            nextPosition
            + forward * 0.35f;


        Vector3 frontLeft =
            front
            - right * 0.20f;


        Vector3 frontRight =
            front
            + right * 0.20f;


        // --------------------------------------------------
        // 육지 검사
        // --------------------------------------------------

        if (_worldMap != null)
        {
            bool blocked =
                !_worldMap.IsWater(front)
                || !_worldMap.IsWater(frontLeft)
                || !_worldMap.IsWater(frontRight);


            if (blocked)
            {
                _currentSpeed =
                    0.0f;

                return;
            }
        }


        Position =
            nextPosition;
    }
}