using Godot;
using System;

public partial class BossRoom : Area2D
{
	[Export] public Godot.Collections.Array<Door> Doors { get; set; } = new();
	[Export] private StatueBoss statueBoss;
	[Export] private AudioStreamPlayer BossMusic;
	private bool Completed = false;

	public override void _Ready()
	{
		statueBoss.BossDied += StopBossFight;
	}

	private void StartBossFight()
	{
		if(Completed)
			return;
		foreach(Door door in Doors)
		{
			door.Close();
		}

		statueBoss.ActivateBossFight();
		BossMusic.Play();
	}

	private void StopBossFight()
	{
		foreach(Door door in Doors)
		{
			door.Open();
		}
		BossMusic.Stop();
		Completed = true;		
	}
	private void _on_body_entered(Node2D body)
	{
		if(body is Detonator)
		{
			CallDeferred(nameof(StartBossFight));
		}
	}
}
