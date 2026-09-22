using Godot;

public partial class PlayerShip : Node3D
{
    private const float MoveSpeed = 3.0f;
    private const float RotationSpeed = 5.0f;
    private WorldMap _worldMap = null!;
    public void SetWorldMap(
        WorldMap worldMap
    )
    {
        _worldMap = worldMap;
    }

    public override void _Process(double delta)
    {
        Vector3 direction =
            Vector3.Zero;


        // W / ↑
        if (Input.IsKeyPressed(Key.W)
            || Input.IsKeyPressed(Key.Up))
        {
            direction.Z -= 1.0f;
        }


        // S / ↓
        if (Input.IsKeyPressed(Key.S)
            || Input.IsKeyPressed(Key.Down))
        {
            direction.Z += 1.0f;
        }


        // A / ←
        if (Input.IsKeyPressed(Key.A)
            || Input.IsKeyPressed(Key.Left))
        {
            direction.X -= 1.0f;
        }


        // D / →
        if (Input.IsKeyPressed(Key.D)
            || Input.IsKeyPressed(Key.Right))
        {
            direction.X += 1.0f;
        }


        if (direction == Vector3.Zero)
        {
            return;
        }


        direction =
            direction.Normalized();


        MoveShip(
            direction,
            (float)delta
        );


        RotateShip(
            direction,
            (float)delta
        );
    }


    // --------------------------------------------------
    // 이동
    // --------------------------------------------------

    private void MoveShip(
        Vector3 direction,
        float delta
    )
    {
        Vector3 nextPosition =
            Position
            + direction
            * MoveSpeed
            * delta;        

        // 이동할 곳이 바다가 아니면 이동하지 않는다.
        if (_worldMap != null
            && !_worldMap.IsWater(nextPosition))
        {
            return;
        }
        Position =
            nextPosition;
    }


    // --------------------------------------------------
    // 이동 방향으로 회전
    // --------------------------------------------------

    private void RotateShip(
        Vector3 direction,
        float delta
    )
    {
        float targetAngle =
            Mathf.Atan2(
                direction.X,
                direction.Z
            );


        Rotation = new Vector3(
            Rotation.X,

            Mathf.LerpAngle(
                Rotation.Y,
                targetAngle,
                RotationSpeed * delta
            ),

            Rotation.Z
        );
    }
}