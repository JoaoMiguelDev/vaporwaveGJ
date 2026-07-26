using Godot;
using System;

public partial class TrialRoom : Area2D
{
	[Export] public Godot.Collections.Array<Door> Doors { get; set; } = new();
	[Export] public Godot.Collections.Array<Marker2D> SpawnPoints { get; set; } = new();
	[Export] private PackedScene ShooterScene;
	[Export] private PackedScene FollowerScene;
	[Export] private Node2D EnemiesNode;
	[Export] private AudioStreamPlayer TrialSong;
    private const int TotalWaves = 3;
    private int CurrentWave = 0;
    private int EnemyQuantity;
    private bool Completed = false;

    public override void _Ready()
    {
       
    }

    private void StartTrial()
    {
        if (Completed)
            return;

        CurrentWave = 0;
        EnemyQuantity = 0;
        Completed = false;

        foreach (Door door in Doors)
        {
            door.Close();
        }

        TrialSong.Play();
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (Completed)
            return;

        CurrentWave++;

        if (CurrentWave > TotalWaves)
        {
            StopTrial();
            return;
        }

        EnemyQuantity = 0;

        for (int i = 0; i < SpawnPoints.Count; i++)
        {
            Marker2D spawnPoint = SpawnPoints[i];

            PackedScene enemyScene = Random.Shared.Next(0, 2) == 0 ? ShooterScene : FollowerScene;
            Node2D enemy = enemyScene.Instantiate<Node2D>();

            if (enemy is Shooter shooter)
            {
                shooter.ShooterDeath += OnEnemyKilled;
            }
            if (enemy is Follower follower)
            {
                follower.FollowerDeath += OnEnemyKilled;
            }

            EnemiesNode.AddChild(enemy);
            enemy.GlobalPosition = spawnPoint.GlobalPosition;
            EnemyQuantity++;
        }
    }

    private void StopTrial()
    {
        foreach (Door door in Doors)
        {
            door.Open();
        }

        TrialSong.Stop();
        Completed = true;
    }

    private void VerifyEnemiesQuantity()
    {
        if (EnemyQuantity == 0)
        {
            if (CurrentWave < TotalWaves)
            {
                StartNextWave();
            }
            else
            {
                StopTrial();
            }
        }
    }

    private void OnEnemyKilled()
    {
        EnemyQuantity--;
        // VerifyEnemiesQuantity();
        CallDeferred(nameof(VerifyEnemiesQuantity));
    }

    private void _on_body_entered(Node2D body)
    {
        if (body is Detonator)
        {
            CallDeferred(nameof(StartTrial));
        }
    }
}
