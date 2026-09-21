shader_type spatial;

render_mode unshaded;


// 기본 바다색
uniform vec4 water_color : source_color =
    vec4(0.08, 0.58, 0.82, 1.0);


// 흐름 속도
uniform float flow_speed = 1.0;


// 색 변화량
// 정말 약하게
uniform float flow_strength = 0.025;


// C#에서 전달받는 타일 위치
uniform vec2 tile_position = vec2(0.0);


varying vec2 world_pos;


void vertex()
{
    // Mesh 자체는 절대 움직이지 않는다.
    world_pos =
        VERTEX.xz
        + tile_position;
}


void fragment()
{
    vec2 p = world_pos;

    float t =
        TIME * flow_speed;


    // ------------------------------------------------
    // 첫 번째 흐름
    //
    // 매우 넓고 완만한 대각선 흐름
    // ------------------------------------------------

    float flow1 =
        sin(
            p.x * 0.65
            + p.y * 0.28
            - t
        );


    // ------------------------------------------------
    // 두 번째 흐름
    //
    // 방향과 속도를 다르게
    // ------------------------------------------------

    float flow2 =
        sin(
            p.x * 0.31
            - p.y * 0.52
            - t * 0.63
        );


    // ------------------------------------------------
    // 세 번째 흐름
    //
    // 더 넓고 아주 느리게
    // ------------------------------------------------

    float flow3 =
        sin(
            p.x * 0.17
            + p.y * 0.21
            + t * 0.35
        );


    // ------------------------------------------------
    // 합성
    // ------------------------------------------------

    float flow =
        flow1 * 0.50
        + flow2 * 0.30
        + flow3 * 0.20;


    // 너무 선명하지 않게
    flow =
        clamp(
            flow,
            -1.0,
            1.0
        );


    // ------------------------------------------------
    // 색 변화
    //
    // RGB 전체를 바꾸는 대신
    // 밝기만 아주 조금 변화
    // ------------------------------------------------

    vec3 color =
        water_color.rgb;


    color *=
        1.0
        + flow * flow_strength;


    ALBEDO = color;
}