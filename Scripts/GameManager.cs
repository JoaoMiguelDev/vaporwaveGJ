using Godot;
using System;

public partial class GameManager : Node
{
	[Export] private Detonator detonator;
	[Export] private Timer DeathTimer;
	[Export] private Hud hud;
	[Export] private ShakyCamera shakyCamera;
	[Export] private StatueBoss statueboss;
	private PackedScene message = GD.Load<PackedScene>("res://Scenes/Ui/FinalMessage.tscn");
	public override void _Ready()
	{
		detonator.HealthChanged += OnPlayerHealthChanged;
		detonator.Died += DetonatorDied;
		statueboss.BossDied += BossDied;
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

	private async void BossDied(){
		await ToSignal(GetTree().CreateTimer(2.0f), Timer.SignalName.Timeout);
		detonator.AddChild(message.Instantiate());
		
	}
}
