using Godot;

public partial class WorldMap : Node3D
{
    // GLB 모델 크기에 맞춰 조절할 값
    public const float TileWidth = 1.0f;
    public const float TileHeight = 1.0f;

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
        BuildMap();
    }


    public int[,] GetMap()
    {
        return _map;
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

                Vector3 position = GetTilePosition(x, z);

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
    // Hex 타일 좌표 계산
    // --------------------------------------------------

    private Vector3 GetTilePosition(int x, int z)
    {
        // 육각형은 좌우 타일이 일부 겹쳐서 배치됨
        float posX = x * TileWidth * 0.75f;

        // 세로 방향 기본 간격
        float posZ = z * TileHeight;

        // 홀수 번째 열은 아래로 반 칸 이동
        if (x % 2 == 1)
        {
            posZ += TileHeight * 0.5f;
        }

        return new Vector3(
            posX,
            0,
            posZ
        );
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

        // 현재 GLB 방향에 맞춤
        tile.RotationDegrees =
            new Vector3(0, 90, 0);

        AddChild(tile);
    }
}