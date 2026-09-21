shader_type spatial;

render_mode diffuse_burley, specular_schlick_ggx;

uniform vec4 water_color : source_color =
    vec4(0.10, 0.62, 0.86, 1.0);

uniform vec4 wave_color : source_color =
    vec4(0.38, 0.80, 0.95, 1.0);

uniform float wave_speed = 0.7;
uniform float wave_scale = 5.0;
uniform float wave_strength = 0.18;

// C#에서 실제 타일 위치 전달
uniform vec2 tile_position = vec2(0.0);

varying vec2 world_wave_position;


void vertex()
{
    // 로컬 좌표 + 타일 위치
    // → 바다 전체가 하나의 좌표계를 사용
    world_wave_position =
        VERTEX.xz + tile_position;
}


void fragment()
{
    float t =
        TIME * wave_speed;

    vec2 p =
        world_wave_position;


    // 큰 물결
    float wave1 =
        sin(
            p.x * wave_scale
            + p.y * 1.4
            - t
        );


    // 약간 다른 방향의 작은 물결
    float wave2 =
        sin(
            p.x * 2.2
            - p.y * 3.2
            - t * 0.65
        );


    float wave =
        wave1 * 0.7
        + wave2 * 0.3;


    wave =
        wave * 0.5 + 0.5;


    // 이전보다 부드럽게
    wave =
        smoothstep(
            0.35,
            0.75,
            wave
        );


    wave *=
        wave_strength;


    ALBEDO =
        mix(
            water_color.rgb,
            wave_color.rgb,
            wave
        );


    ROUGHNESS = 0.72;
    METALLIC = 0.0;
}