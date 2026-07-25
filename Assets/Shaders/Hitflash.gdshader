shader_type canvas_item;

uniform vec4 HitFlashColor : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform bool HitFlashOn = false;

void fragment() {
	vec4 SpriteColor = texture(TEXTURE, UV);
	vec3 FinalRgb = HitFlashOn ? HitFlashColor.rgb : SpriteColor.rgb;
	COLOR = vec4(FinalRgb, SpriteColor.a);
}