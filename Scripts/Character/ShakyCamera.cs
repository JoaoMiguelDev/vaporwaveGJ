using Godot;

public partial class ShakyCamera : Camera2D
{
	private float ShakeIntensity;
	private float ActiveShakeTime;
	[Export] public float ShakeDecay { get; set; } = 5.0f;
	[Export] public float ShakeTimeSpeed { get; set; } = 20.0f;
	private float ShakeTime;
	private FastNoiseLite Noise = new FastNoiseLite();

	public override void _PhysicsProcess(double delta)
	{
		float deltaFloat = (float)delta;

		if (ActiveShakeTime > 0f)
		{
			ShakeTime += deltaFloat * ShakeTimeSpeed;
			ActiveShakeTime -= deltaFloat;

			float noiseX = Noise.GetNoise2D(ShakeTime, 0f);
			float noiseY = Noise.GetNoise2D(0f, ShakeTime);

			Offset = new Vector2(
				noiseX * ShakeIntensity,
				noiseY * ShakeIntensity
			);

			ShakeIntensity = Mathf.Max(ShakeIntensity - ShakeDecay * deltaFloat, 0f);
		}
		else
		{
			Offset = Offset.Lerp(Vector2.Zero, 5f * deltaFloat);
		}
	}

	public void ScreenShake(float intensity, float time)
	{
		Noise.Seed = (int)GD.Randi();
		Noise.Frequency = 2.0f;
		Noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;

		ShakeIntensity = Mathf.Max(intensity * 25f, 10f);
		ActiveShakeTime = Mathf.Max(time, 0.2f);
		ShakeTime = 0f;
	}
}
