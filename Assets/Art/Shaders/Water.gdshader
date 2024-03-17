shader_type canvas_item;

uniform sampler2D noise1 : repeat_enable;
uniform sampler2D noise2 : repeat_enable;
uniform vec2 scroll1 = vec2(0.05, 0.05);
uniform vec2 scroll2 = vec2(-0.05, -0.05);
uniform float distortion_strength : hint_range(-1,1) = 0.2;
uniform vec4 tone_color : source_color;
uniform vec4 top_color : source_color;
uniform float light_start : hint_range(0.0, 1.0) = 0.35;
uniform float light_end : hint_range(0.0, 1.0) = 0.4;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_nearest;

varying vec2 tile_center;
varying vec2 tile_world;
varying vec2 tile_color;
uniform vec2 tile_size;

void vertex() {
    float xSize = tile_size[0];
    float ySize = tile_size[1];

    vec2 origin = VERTEX;

    // Calculate center of the tile based on VERTEX_ID and Tile Size
    if(VERTEX_ID == 0) {
        origin.x = origin.x + (xSize / 2.0);
        origin.y = origin.y + (ySize / 2.0);
    } else if(VERTEX_ID == 1) {
        origin.x = origin.x + (xSize / 2.0);
        origin.y = origin.y - (ySize / 2.0);
    } else if(VERTEX_ID == 2) {
        origin.x = origin.x - (xSize / 2.0);
        origin.y = origin.y - (ySize / 2.0);
    } else if(VERTEX_ID == 3) {
        origin.x = origin.x - (xSize / 2.0);
        origin.y = origin.y + (ySize / 2.0);
    }

    vec4 worldspace = (MODEL_MATRIX * vec4(origin, 0.0, 1.0));
    vec4 canvasspace = (CANVAS_MATRIX * worldspace);
    vec4 screenspace = (SCREEN_MATRIX * canvasspace);

    tile_center = vec2(screenspace[0], screenspace[1]);
	tile_world = vec2(worldspace[0], worldspace[1]);

    float min1 = -1.0;
    float max1 = 1.0;

    float min2 = 0.0;
    float max2 = 1.0;

    float __input_range = max1 - min1;
    float __output_range = max2 - min2;

    float valx = screenspace.x;
    tile_color[0] = min2 + __output_range * ((valx - min1) / __input_range);
    float valy = screenspace.y;
    tile_color[1] = min2 + __output_range * ((valy - min1) / __input_range);
}

void fragment() {
	float depth = texture(noise1, UV + tile_center + scroll1 * TIME).r * texture(noise2, UV + tile_center + scroll2 * TIME).r;
	vec4 screen_col = texture(SCREEN_TEXTURE, SCREEN_UV + distortion_strength * vec2(depth));
	vec4 top_light = smoothstep(light_start, light_end, depth) * top_color;
	COLOR = screen_col * tone_color + top_light;
	COLOR.a = 1.0;
}

void light() {
	// Called for every pixel for every light affecting the CanvasItem.
}
