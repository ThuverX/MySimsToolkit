shader_type spatial;

uniform sampler2D Texture;

void fragment() {
	vec4 color = texture(Texture, UV);

	ALBEDO = color.rgb;
	ALPHA = color.a;
}