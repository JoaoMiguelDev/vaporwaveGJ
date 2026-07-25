using Godot;
using System;

public partial class TrialRoom : Area2D
{
	[Export] public Godot.Collections.Array<Door> Doors { get; set; } = new();
	[Export] public Godot.Collections.Array<Marker2D> SpawnPoints { get; set; } = new();
	[Export] private PackedScene ShooterScene;
	[Export] private PackedScene FollowerScene;
	[Export] private Node2D EnemiesNode;
	private int EnemyQuantity;
	private bool Completed = false;

    public override void _Ready()
    {
       
    }

	private void StartTrial()
	{
		if(Completed)
			return;

		foreach(Door door in Doors)
		{
			door.Close();
		}

		for(int i = 0; i < SpawnPoints.Count; i++)
		{
			Marker2D spawnPoint = SpawnPoints[i];

			PackedScene enemyScene = (i % 2 == 0) ? ShooterScene : FollowerScene;
			Node2D enemy = enemyScene.Instantiate<Node2D>();

			if(enemy is Shooter shooter)
			{
				shooter.ShooterDeath += OnEnemyKilled;
			}
			if(enemy is Follower follower)
			{
				follower.FollowerDeath += OnEnemyKilled;
			}
			EnemiesNode.AddChild(enemy);
			enemy.GlobalPosition = spawnPoint.GlobalPosition;	
			EnemyQuantity ++;	
		}
	}

	private void StopTrial()
	{
		GD.Print("Entrei porra");
		foreach(Door door in Doors)
		{
			door.Open();
		}
		Completed = true;		
	}

	private void VerifyEnemiesQuantity()
	{
		GD.Print(EnemyQuantity);
		if(EnemyQuantity == 0)
		{
			StopTrial();
		}
	}

	private void OnEnemyKilled()
	{
		EnemyQuantity --;
		VerifyEnemiesQuantity();	
	}

	private void _on_body_entered(Node2D body)
	{
		if(body is Detonator)
		{
			CallDeferred(nameof(StartTrial));
		}
	}
}
