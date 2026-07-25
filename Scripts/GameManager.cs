using Godot;
using System;

public partial class GameManager : Node
{
	[Export] private Detonator detonator;
	[Export] private Timer DeathTimer;
	[Export] private Hud hud;
	[Export] private ShakyCamera shakyCamera;
	public override void _Ready()
	{
		detonator.HealthChanged += OnPlayerHealthChanged;
		detonator.Died += DetonatorDied;
	}
	private void OnPlayerHealthChanged(int current, int max)
	{
		hud.UpdateBatteries(current);
	}

	private void DetonatorDied()
	{
		DeathTimer.Start();
	}

	public void _on_death_timer_timeout()
	{
		GetTree().ReloadCurrentScene();
	}

	public void ShakeCamera(float intensity, float time)
	{
		shakyCamera.ScreenShake(intensity, time);
	}	

}
