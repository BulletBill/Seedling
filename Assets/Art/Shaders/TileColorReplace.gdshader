shader_type canvas_item;

//uniform vec4 dirt_color : source_color = vec4(1.0, 0.84, 0.50, 1.0);
uniform vec4 dirt_color : source_color = vec4(0.89, 0.74, 0.44, 1.0);
uniform vec4 grass_color : source_color = vec4(0.47, 0.74, 0.32, 1.0);
uniform vec4 water_color : source_color = vec4(0.30, 0.65, 0.71, 1.0);
uniform vec4 sand_color : source_color = vec4(1.0, 1.0, 0.71, 1.0);

void fragment() {
	// Called for every pixel the material is visible on.
	float alpha = COLOR.a;
	if (COLOR.r > COLOR.g && COLOR.r > COLOR.b) // Dirt
	{
		float correction = 0.89;
		vec4 NewColor = (dirt_color / correction) * (COLOR.r);
		COLOR = NewColor;
	}
	else if (COLOR.g > COLOR.r && COLOR.g > COLOR.b) // Grass
	{
		float correction = 0.74;
		vec4 NewColor = (grass_color / correction) * (COLOR.g);
		COLOR = NewColor;
	}
	else if (COLOR.b > COLOR.r && COLOR.b > COLOR.g) // Water
	{
		float correction = 0.71;
		vec4 NewColor = (water_color / correction) * (COLOR.b);
		COLOR = NewColor;
	}
	else if (COLOR.r == COLOR.g && COLOR.r != COLOR.b)
	{
		vec4 NewColor = sand_color * COLOR.r;
		COLOR = NewColor;
	}
	COLOR.a = alpha;
}