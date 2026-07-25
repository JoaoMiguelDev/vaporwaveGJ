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

        if (ActiveShakeTime > 0)
        {
            ShakeTime += deltaFloat * ShakeTimeSpeed;
            ActiveShakeTime -= deltaFloat;

            Offset = new Vector2(
                Noise.GetNoise2D(ShakeTime, 0) * ShakeIntensity,
                Noise.GetNoise2D(0, ShakeTime) * ShakeIntensity
            );

            ShakeIntensity = Mathf.Max(ShakeIntensity - ShakeDecay * deltaFloat, 0);
        }
        else
        {
            Offset = Offset.Lerp(Vector2.Zero, 10.5f * deltaFloat);
        }
    }

    public void ScreenShake(float intensity, float time)
    {
        GD.Randomize();
        Noise.Seed = (int)GD.Randi();
        Noise.Frequency = 2.0f;

        ShakeIntensity = intensity;
        ActiveShakeTime = time;
        ShakeTime = 0.0f;
    }
}

