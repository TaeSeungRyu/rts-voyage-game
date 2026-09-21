using Godot;

public partial class Wave : Node3D
{
    private Vector3 _startPosition;

    private float _time = 0.0f;

    // 물결마다 조금씩 다른 시작 시점
    private float _phase;

    public override void _Ready()
    {
        _startPosition = Position;

        _phase = GD.Randf() * Mathf.Tau;
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;

        // 위아래 움직임
        float y =
            Mathf.Sin(
                _time * 2.0f + _phase
            ) * 0.015f;

        // 좌우로 아주 약하게 움직임
        float x =
            Mathf.Sin(
                _time * 0.8f + _phase
            ) * 0.02f;

        Position =
            _startPosition
            + new Vector3(
                x,
                y,
                0
            );
    }
}