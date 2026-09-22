using Godot;

public partial class PlayerShip : Node3D
{
    // --------------------------------------------------
    // 데이터
    // --------------------------------------------------

    private const string ShipDataPath =
        "res://assets/data/ships.json";


    private ShipStats _stats = null!;

    private WorldMap _worldMap = null!;

    private Node3D _shipModel = null!;


    // --------------------------------------------------
    // 상태
    // --------------------------------------------------

    private float _currentSpeed =
        0.0f;


    // --------------------------------------------------
    // WorldMap 연결
    // --------------------------------------------------

    public void SetWorldMap(
        WorldMap worldMap
    )
    {
        _worldMap =
            worldMap;
    }


    // --------------------------------------------------
    // Ship 로딩
    // --------------------------------------------------

    public void LoadShip(
        string shipId
    )
    {
        using FileAccess file =
            FileAccess.Open(
                ShipDataPath,
                FileAccess.ModeFlags.Read
            );


        if (file == null)
        {
            GD.PrintErr(
                $"선박 데이터 파일을 불러올 수 없습니다: {ShipDataPath}"
            );

            return;
        }


        string jsonText =
            file.GetAsText();


        Json json =
            new Json();


        Error error =
            json.Parse(
                jsonText
            );


        if (error != Error.Ok)
        {
            GD.PrintErr(
                $"선박 JSON 파싱 실패: {json.GetErrorMessage()}"
            );

            return;
        }


        Godot.Collections.Dictionary ships =
            json.Data
                .AsGodotDictionary();


        if (!ships.ContainsKey(shipId))
        {
            GD.PrintErr(
                $"선박을 찾을 수 없습니다: {shipId}"
            );

            return;
        }


        Godot.Collections.Dictionary data =
            ships[shipId]
                .AsGodotDictionary();


        // --------------------------------------------------
        // Stats 생성
        // --------------------------------------------------

        _stats =
            new ShipStats
            {
                Id =
                    shipId,

                Name =
                    data["name"]
                        .AsString(),

                Model =
                    data["model"]
                        .AsString(),

                MaxForwardSpeed =
                    (float)data["maxForwardSpeed"],

                MaxBackwardSpeed =
                    (float)data["maxBackwardSpeed"],

                ForwardAcceleration =
                    (float)data["forwardAcceleration"],

                BackwardAcceleration =
                    (float)data["backwardAcceleration"],

                Deceleration =
                    (float)data["deceleration"],

                TurnSpeed =
                    (float)data["turnSpeed"],

                CollisionLength =
                    (float)data["collisionLength"],

                CollisionWidth =
                    (float)data["collisionWidth"]
            };


        // --------------------------------------------------
        // 모델 로딩
        // --------------------------------------------------

        LoadShipModel();


        // 배를 변경했다면 기존 속도 초기화
        _currentSpeed =
            0.0f;


        GD.Print(
            $"Ship Loaded: {_stats.Name}"
        );
    }


    // --------------------------------------------------
    // Ship Model
    // --------------------------------------------------

    private void LoadShipModel()
    {
        if (_stats == null)
        {
            return;
        }


        // 기존 모델 제거
        if (_shipModel != null)
        {
            _shipModel.QueueFree();

            _shipModel =
                null;
        }


        PackedScene scene =
            GD.Load<PackedScene>(
                _stats.Model
            );


        if (scene == null)
        {
            GD.PrintErr(
                $"선박 모델을 불러올 수 없습니다: {_stats.Model}"
            );

            return;
        }


        _shipModel =
            scene.Instantiate<Node3D>();


        _shipModel.Name =
            "ShipModel";


        AddChild(
            _shipModel
        );
    }


    // --------------------------------------------------
    // Process
    // --------------------------------------------------

    public override void _Process(
        double delta
    )
    {
        // 배 데이터가 로딩되지 않았다면
        // 아무것도 하지 않는다.
        if (_stats == null)
        {
            return;
        }


        float dt =
            (float)delta;


        HandleSpeed(
            dt
        );


        HandleRotation(
            dt
        );


        MoveShip(
            dt
        );
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


        // --------------------------------------------------
        // 전진
        // --------------------------------------------------

        if (forward)
        {
            _currentSpeed =
                Mathf.MoveToward(
                    _currentSpeed,
                    _stats.MaxForwardSpeed,
                    _stats.ForwardAcceleration
                    * delta
                );

            return;
        }


        // --------------------------------------------------
        // 후진
        // --------------------------------------------------

        if (backward)
        {
            _currentSpeed =
                Mathf.MoveToward(
                    _currentSpeed,
                    -_stats.MaxBackwardSpeed,
                    _stats.BackwardAcceleration
                    * delta
                );

            return;
        }


        // --------------------------------------------------
        // 자연 감속
        // --------------------------------------------------

        _currentSpeed =
            Mathf.MoveToward(
                _currentSpeed,
                0.0f,
                _stats.Deceleration
                * delta
            );
    }


    // --------------------------------------------------
    // 방향 전환
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
            turnInput +=
                1.0f;
        }


        // 오른쪽
        if (Input.IsKeyPressed(Key.D)
            || Input.IsKeyPressed(Key.Right))
        {
            turnInput -=
                1.0f;
        }


        if (turnInput == 0.0f)
        {
            return;
        }


        // 정지 상태에서도 방향 전환 가능
        RotateY(
            turnInput
            * _stats.TurnSpeed
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
        if (Mathf.Abs(_currentSpeed)
            <= 0.001f)
        {
            return;
        }


        // --------------------------------------------------
        // 배 전방 방향
        // --------------------------------------------------

        Vector3 forward =
            -GlobalTransform
                .Basis
                .Z;


        forward.Y =
            0.0f;


        forward =
            forward.Normalized();


        // --------------------------------------------------
        // 배 우측 방향
        // --------------------------------------------------

        Vector3 right =
            GlobalTransform
                .Basis
                .X;


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
        // 전진 / 후진 방향
        // --------------------------------------------------

        float moveDirection =
            Mathf.Sign(
                _currentSpeed
            );


        Vector3 collisionDirection =
            forward
            * moveDirection;


        // --------------------------------------------------
        // 충돌 검사 지점
        // --------------------------------------------------

        Vector3 checkCenter =
            nextPosition
            + collisionDirection
            * _stats.CollisionLength;


        Vector3 checkLeft =
            checkCenter
            - right
            * _stats.CollisionWidth;


        Vector3 checkRight =
            checkCenter
            + right
            * _stats.CollisionWidth;


        // --------------------------------------------------
        // 육지 검사
        // --------------------------------------------------

        if (_worldMap != null)
        {
            bool blocked =
                !_worldMap.IsWater(
                    checkCenter
                )
                ||
                !_worldMap.IsWater(
                    checkLeft
                )
                ||
                !_worldMap.IsWater(
                    checkRight
                );


            if (blocked)
            {
                _currentSpeed =
                    0.0f;

                return;
            }
        }


        // --------------------------------------------------
        // 실제 이동
        // --------------------------------------------------

        Position =
            nextPosition;
    }
}