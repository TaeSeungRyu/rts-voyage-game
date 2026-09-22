using Godot;

public partial class PlayerShip : Node3D
{
    // --------------------------------------------------
    // 이동 설정
    // --------------------------------------------------

    // 최대 전진 속도
    private const float MaxForwardSpeed = 3.0f;

    // 최대 후진 속도
    private const float MaxBackwardSpeed = 1.5f;

    // 전진 가속도
    private const float ForwardAcceleration = 3.0f;

    // 후진 가속도
    private const float BackwardAcceleration = 2.0f;

    // 키를 놓았을 때 감속
    private const float Deceleration = 2.0f;

    // 회전 속도
    // 기존보다 크게 해서 회전 반경을 줄인다.
    private const float TurnSpeed = 2.8f;


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
    // 전진 / 후진
    // --------------------------------------------------

    private void HandleSpeed(
        float delta
    )
    {
        bool forward =
            Input.IsKeyPressed(Key.W)
            || Input.IsKeyPressed(Key.Up);

        bool backward =
            Input.IsKeyPressed(Key.S)
            || Input.IsKeyPressed(Key.Down);


        // 앞으로
        if (forward)
        {
            _currentSpeed =
                Mathf.MoveToward(
                    _currentSpeed,
                    MaxForwardSpeed,
                    ForwardAcceleration * delta
                );

            return;
        }


        // 뒤로
        if (backward)
        {
            _currentSpeed =
                Mathf.MoveToward(
                    _currentSpeed,
                    -MaxBackwardSpeed,
                    BackwardAcceleration * delta
                );

            return;
        }


        // 아무 키도 누르지 않으면 0으로 감속
        _currentSpeed =
            Mathf.MoveToward(
                _currentSpeed,
                0.0f,
                Deceleration * delta
            );
    }


    // --------------------------------------------------
    // 좌우 방향 전환
    // --------------------------------------------------

    private void HandleRotation(
        float delta
    )
    {
        float turnInput =
            0.0f;


        // 왼쪽
        if (Input.IsKeyPressed(Key.A)
            || Input.IsKeyPressed(Key.Left))
        {
            turnInput += 1.0f;
        }


        // 오른쪽
        if (Input.IsKeyPressed(Key.D)
            || Input.IsKeyPressed(Key.Right))
        {
            turnInput -= 1.0f;
        }


        if (turnInput == 0.0f)
        {
            return;
        }


        // 정지 상태에서도 회전 가능
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
        if (Mathf.Abs(_currentSpeed) <= 0.001f)
        {
            return;
        }


        // 배가 바라보는 방향
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


        // 전진 / 후진 방향
        float moveDirection =
            Mathf.Sign(_currentSpeed);


        // --------------------------------------------------
        // 다음 위치
        // --------------------------------------------------

        Vector3 nextPosition =
            Position
            + forward
            * _currentSpeed
            * delta;


        // --------------------------------------------------
        // 충돌 검사 방향
        //
        // 전진이면 앞쪽,
        // 후진이면 뒤쪽을 검사한다.
        // --------------------------------------------------

        Vector3 collisionDirection =
            forward
            * moveDirection;


        Vector3 checkCenter =
            nextPosition
            + collisionDirection * 0.45f;


        Vector3 checkLeft =
            checkCenter
            - right * 0.20f;


        Vector3 checkRight =
            checkCenter
            + right * 0.20f;


        // --------------------------------------------------
        // 육지 검사
        // --------------------------------------------------

        if (_worldMap != null)
        {
            bool blocked =
                !_worldMap.IsWater(checkCenter)
                || !_worldMap.IsWater(checkLeft)
                || !_worldMap.IsWater(checkRight);


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