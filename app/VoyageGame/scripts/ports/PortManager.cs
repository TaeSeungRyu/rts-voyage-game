using Godot;
using System.Collections.Generic;

public partial class PortManager : Node3D
{
    private const string PortDataPath =
        "res://assets/data/ports.json";


    private WorldMap _worldMap = null!;


    private readonly List<PortData> _ports =
        new List<PortData>();


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
    // 항구 로딩
    // --------------------------------------------------

    public void LoadPorts()
    {
        if (_worldMap == null)
        {
            GD.PrintErr(
                "PortManager에 WorldMap이 연결되지 않았습니다."
            );

            return;
        }


        using FileAccess file =
            FileAccess.Open(
                PortDataPath,
                FileAccess.ModeFlags.Read
            );


        if (file == null)
        {
            GD.PrintErr(
                $"항구 데이터 파일을 불러올 수 없습니다: {PortDataPath}"
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
                $"항구 JSON 파싱 실패: {json.GetErrorMessage()}"
            );

            return;
        }


        Godot.Collections.Dictionary root =
            json.Data
                .AsGodotDictionary();


        if (!root.ContainsKey("ports"))
        {
            GD.PrintErr(
                "ports.json에 ports 배열이 없습니다."
            );

            return;
        }


        Godot.Collections.Array ports =
            root["ports"]
                .AsGodotArray();


        foreach (Variant item in ports)
        {
            Godot.Collections.Dictionary data =
                item.AsGodotDictionary();


            PortData port =
                new PortData
                {
                    Id =
                        data["id"]
                            .AsString(),

                    Name =
                        data["name"]
                            .AsString(),

                    X =
                        data["x"]
                            .AsInt32(),

                    Z =
                        data["z"]
                            .AsInt32(),

                    Rotation =
                        (float)data["rotation"],
                    Height =
                        (float)data["height"],                        
                    Model =
                        data["model"]
                            .AsString()
                };


            _ports.Add(
                port
            );


            CreatePort(
                port
            );
        }


        GD.Print(
            $"Ports Loaded: {_ports.Count}"
        );
    }


    // --------------------------------------------------
    // 항구 생성
    // --------------------------------------------------

    private void CreatePort(
        PortData port
    )
    {
        PackedScene scene =
            GD.Load<PackedScene>(
                port.Model
            );


        if (scene == null)
        {
            GD.PrintErr(
                $"항구 모델을 불러올 수 없습니다: {port.Model}"
            );

            return;
        }


        Node3D model =
            scene.Instantiate<Node3D>();


        model.Name =
            $"Port_{port.Id}";


        // 타일 좌표 → 실제 월드 좌표
        Vector3 position =
            _worldMap.GetTileWorldPosition(
                port.X,
                port.Z
            );

        position.Y =
            port.Height;

        model.Position =
            position;


        model.RotationDegrees =
            new Vector3(
                0.0f,
                port.Rotation,
                0.0f
            );


        AddChild(
            model
        );


        GD.Print(
            $"Port Created: {port.Name} ({port.X}, {port.Z})"
        );
    }
}