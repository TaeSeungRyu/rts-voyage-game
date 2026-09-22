using Godot;
using System;

public partial class WorldMap : Node3D
{
    // --------------------------------------------------
    // 타일 타입
    // --------------------------------------------------
    private const int Water = 0; //바다
    private const int Grass = 1; //육지
    private const int Port = 2; //항구
    //

    private const float WaterHeight = 0.0f;
    private const float GrassHeight = 0.08f;

    // --------------------------------------------------
    // 맵 설정
    // --------------------------------------------------
    private float _tileWidth = 1.0f;
    private float _tileHeight = 1.0f;
    private string[] _tiles = Array.Empty<string>();


    // --------------------------------------------------
    // GLB Scene
    // --------------------------------------------------

    private PackedScene _waterScene = null!;
    private PackedScene _grassScene = null!;
    private PackedScene _portScene = null!;
    private Shader _waterShader = null!;


    // --------------------------------------------------
    // Ready
    // --------------------------------------------------

    public override void _Ready()
    {
        LoadScenes();

        LoadMap();

        BuildMap();
    }


    // --------------------------------------------------
    // GLB 로딩
    // --------------------------------------------------

    private void LoadScenes()
    {
        _waterScene =
            GD.Load<PackedScene>(
                "res://assets/models/water.glb"
            );

        _grassScene =
            GD.Load<PackedScene>(
                "res://assets/models/grass.glb"
            );
        _waterShader =
            GD.Load<Shader>(
                "res://assets/shaders/water.gdshader"
            );            

        _portScene  =
            GD.Load<PackedScene>(
                "res://assets/models/port-small.glb"
            );            

        if (_waterScene == null)
        {
            GD.PrintErr(
                "water.glb를 불러올 수 없습니다."
            );
        }

        if (_grassScene == null)
        {
            GD.PrintErr(
                "grass.glb를 불러올 수 없습니다."
            );
        }

        if (_waterShader == null)
        {
            GD.PrintErr(
                "water.gdshader를 불러올 수 없습니다."
            );
        }
    }


    // --------------------------------------------------
    // JSON 맵 로딩
    // --------------------------------------------------

    private void LoadMap()
    {
        string path =
            "res://assets/maps/world.json";


        using FileAccess file =
            FileAccess.Open(
                path,
                FileAccess.ModeFlags.Read
            );


        if (file == null)
        {
            GD.PrintErr(
                $"맵 파일을 불러올 수 없습니다: {path}"
            );

            return;
        }


        string jsonText =
            file.GetAsText();


        Json json =
            new Json();


        Error error =
            json.Parse(jsonText);


        if (error != Error.Ok)
        {
            GD.PrintErr(
                $"JSON 파싱 실패: {json.GetErrorMessage()}"
            );

            return;
        }


        Godot.Collections.Dictionary data =
            json.Data.AsGodotDictionary();


        // 타일 크기
        _tileWidth =
            (float)data["tileWidth"];

        _tileHeight =
            (float)data["tileHeight"];


        // 타일 데이터
        Godot.Collections.Array tileData =
            data["tiles"].AsGodotArray();


        _tiles =
            new string[tileData.Count];


        for (int i = 0; i < tileData.Count; i++)
        {
            _tiles[i] =
                tileData[i].AsString();
        }


        if (_tiles.Length > 0)
        {
            GD.Print(
                $"WorldMap Loaded: " +
                $"{_tiles[0].Length} x {_tiles.Length}"
            );
        }
    }


    // --------------------------------------------------
    // 맵 생성
    // --------------------------------------------------

    private void BuildMap()
    {
        if (_tiles.Length == 0)
        {
            GD.PrintErr(
                "맵 데이터가 없습니다."
            );

            return;
        }


        int rows =
            _tiles.Length;

        int columns =
            _tiles[0].Length;


        for (int z = 0; z < rows; z++)
        {
            for (int x = 0; x < columns; x++)
            {
                int tileType =
                    _tiles[z][x] - '0';
                Vector3 position =
                    GetTilePosition(
                        x,
                        z
                    );
                if (tileType == Water)
                {
                    position.Y = WaterHeight;
                    CreateWaterTile(
                        position
                    );
                }
                else if (tileType == Grass)
                {
                    position.Y = GrassHeight;
                    CreateTile(
                        _grassScene,
                        position
                    );
                }
                else if (tileType == Port)
                {
                    // 항구도 기본적으로 육지이므로
                    // 먼저 Grass 생성
                    Vector3 grassPosition =
                        position;

                    grassPosition.Y =
                        GrassHeight;

                    CreateTile(
                        _grassScene,
                        grassPosition
                    );


                    // 그 위에 항구 건물 생성
                    Vector3 portPosition =
                        position;

                    portPosition.Y =
                        GrassHeight;

                    CreateTile(
                        _portScene,
                        portPosition
                    );
                }                
            }
        }


        GD.Print(
            $"WorldMap Build Complete: {rows * columns} tiles"
        );
    }


    // --------------------------------------------------
    // Hex 타일 위치 계산
    // --------------------------------------------------

    private Vector3 GetTilePosition(
        int x,
        int z
    )
    {
        float posX =
            x
            * _tileWidth
            * 0.75f;


        float posZ =
            z
            * _tileHeight;


        // 홀수 열은 반 칸 아래로 이동
        if (x % 2 == 1)
        {
            posZ +=
                _tileHeight
                * 0.5f;
        }


        return new Vector3(
            posX,
            0,
            posZ
        );
    }


    // --------------------------------------------------
    // 일반 타일 생성
    // --------------------------------------------------

    private void CreateTile(
        PackedScene scene,
        Vector3 position
    )
    {
        if (scene == null)
        {
            return;
        }


        Node3D tile =
            scene.Instantiate<Node3D>();


        tile.Position =
            position;


        tile.RotationDegrees =
            new Vector3(
                0,
                90,
              0
            );


        AddChild(tile);
    }


    // --------------------------------------------------
    // 바다 타일 생성
    // --------------------------------------------------
    private void CreateWaterTile(Vector3 position)
    {
        if (_waterScene == null)
            return;

        Node3D water =
            _waterScene.Instantiate<Node3D>();

        water.Position = position;

        water.RotationDegrees =
            new Vector3(0, 90, 0);

        ApplyWaterShader(
            water,
            position
        );

        AddChild(water);
    }

    private void ApplyWaterShader(
        Node node,
        Vector3 position
    )
    {
        if (node is MeshInstance3D mesh)
        {
            ShaderMaterial material =
                new ShaderMaterial();

            material.Shader =
                _waterShader;

            material.SetShaderParameter(
                "tile_position",
                new Vector2(
                    position.X,
                    position.Z
                )
            );

            mesh.MaterialOverride =
                material;
        }

        foreach (Node child in node.GetChildren())
        {
            ApplyWaterShader(
                child,
                position
            );
        }
    }    


    // --------------------------------------------------
    // 맵 중앙 좌표
    // --------------------------------------------------

    public Vector3 GetCenter()
    {
        if (_tiles.Length == 0)
        {
            return Vector3.Zero;
        }


        int rows =
            _tiles.Length;

        int columns =
            _tiles[0].Length;


        float mapWidth =
            (columns - 1)
            * _tileWidth
            * 0.75f;


        float mapHeight =
            (rows - 1)
            * _tileHeight;


        return new Vector3(
            mapWidth / 2.0f,
            0,
            mapHeight / 2.0f
        );
    }


    public bool IsWater(Vector3 worldPosition)
    {
        if (_tiles.Length == 0)
            return false;

        int columns = _tiles[0].Length;
        int rows = _tiles.Length;

        // 가장 가까운 Hex 열 계산
        int x =
            Mathf.RoundToInt(
                worldPosition.X
                / (_tileWidth * 0.75f)
            );

        if (x < 0 || x >= columns)
            return false;


        // 홀수 열의 Z 오프셋
        float zOffset =
            (x % 2 == 1)
                ? _tileHeight * 0.5f
                : 0.0f;


        int z =
            Mathf.RoundToInt(
                (worldPosition.Z - zOffset)
                / _tileHeight
            );

        if (z < 0 || z >= rows)
            return false;


        int tileType =
            _tiles[z][x] - '0';


        return tileType == Water;
    }    
}