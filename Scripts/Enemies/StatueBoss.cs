using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

public partial class StatueBoss : CharacterBody2D
{
	// Index display of the ProjectileSpawnPoints list
	//		0
	//	  7	   1
	//	6		 2
	//	  5	   3			
	//		4
	[Signal] public delegate void BossHealthChangedEventHandler(int current, int max);
	[Signal] public delegate void BattleStartedEventHandler();
	[Signal] public delegate void BossDiedEventHandler();
	[Export] public Godot.Collections.Array<Marker2D> ProjectileSpawnPoints { get; set; } = new();
	[Export] public Godot.Collections.Array<BossLaser> Lasers { get; set; } = new();
	[Export] public Godot.Collections.Array<GpuParticles2D> LaserStartParticles { get; set; } = new();
	[Export] public Godot.Collections.Array<GpuParticles2D> OmnishootStartParticles { get; set; } = new();
	[Export] private Node2D LaserNode;
	[Export] private GameManager gameManager;
	[Export] private AnimationPlayer animation;
	[Export] private AnimatedSprite2D sprite2D;
	[Export] private PackedScene HitParticlesScene;
	[Export] private PackedScene BossProjectileScene;

	//States variables
	public enum BossState { Idle , LaserSpin , OmniShoot }
	private BossState CurrentState = BossState.Idle;
	[Export] private Timer IdleTimer; //Time he stays idle
	[Export] private Timer StartupLaserSpinTimer; //Time he charges the laser
	[Export] private Timer LaserSpinTimer; //Time he stays spinning 
	[Export] private Timer ShootIntervalTimer; //Time between shots
	[Export] private Timer StartupOmniShootTimer; //Time he charges the shots
 	[Export] private Timer OmnishootTimer; //Time in the Omnishoot state
	private bool CanShoot = true;

	//Health variables
	private const int MaxHealth = 40;
	private int _health = 40;
	public int Health
	{
		get => _health;
		private set
		{
			_health = Mathf.Clamp(value, 0, MaxHealth);
			EmitSignal(SignalName.BossHealthChanged, _health, MaxHealth);
			if (_health == 0)
			{
				EmitSignal(SignalName.BossDied);
				Die();
			} 
		}
	}

	public override void _Ready()
	{
		// EnterState(BossState.Idle);
	}

	public override void _PhysicsProcess(double delta)
	{
		float deltaF = (float) delta;

		switch (CurrentState)
		{
			case BossState.Idle:
				break;

			case BossState.LaserSpin:
				LaserSpin(deltaF);
				break;

			case BossState.OmniShoot:
				OmnidirectionShoot();
				break;
		}
	}

	private void EnterState(BossState newState)
	{
		CurrentState = newState;
		// AnimatedSprite.Play(newState.ToString());

		switch (newState)
		{
			case BossState.Idle:
				// DeactivateLasers();
				IdleTimer.Start();
				sprite2D.Play("idle");
				break;

			case BossState.LaserSpin:
				GD.Print("Vou começar a soltar laser");
				StartupLaserSpinTimer.Start();
				sprite2D.Play("attack");
				EmitLaserStartParticles();
				break;

			case BossState.OmniShoot:
				StartupOmniShootTimer.Start();
				sprite2D.Play("attack");
				EmitOmnishootStartParticles();
				break;
		}
	}

	//Laser spin state methods
	private void LaserSpin(float delta)
	{
		LaserNode.Rotation += 0.8f *delta;
	}

	private void ActivateLasers()
	{
		foreach(BossLaser laser in Lasers)
		{
			laser.IsCasting = true;
		}
	}

	private void DeactivateLasers()
	{
		foreach(BossLaser laser in Lasers)
		{
			laser.IsCasting = false;
		}		
	}

	private void EmitLaserStartParticles()
	{
		foreach(GpuParticles2D laserStartParticle in LaserStartParticles)
		{
			laserStartParticle.Emitting = true;
		}
	}

	private void StopLaserStartParticles()
	{
		foreach(GpuParticles2D laserStartParticle in LaserStartParticles)
		{
			laserStartParticle.Emitting = false;
		}
	}

	public void _on_start_up_laser_spin_timer_timeout()
	{
		LaserSpinTimer.Start();
		ActivateLasers();
	}

	public void _on_laser_spin_timer_timeout()
	{
		EnterState(BossState.Idle);
		StopLaserStartParticles();
		DeactivateLasers();
	}

	public void _on_idle_timer_timeout()
	{
		if (GD.Randi() % 2 == 0)
		{
			EnterState(BossState.LaserSpin);
		}
		else
		{
			EnterState(BossState.OmniShoot);
		}
	}

	//Omnishoot state methods

	private void OmnidirectionShoot()
	{
		if(!CanShoot)
			return;

		CanShoot = false;
		ShootIntervalTimer.Start();
		foreach (Marker2D spawnPoint in ProjectileSpawnPoints)
		{
			if (spawnPoint == null)
				continue;

			var projectile = BossProjectileScene.Instantiate<BossProjectile>();
			projectile.GlobalPosition = spawnPoint.GlobalPosition;

			Vector2 direction = -(GlobalPosition - spawnPoint.GlobalPosition).Normalized();
			projectile.SetDirection(direction);

			GetTree().CurrentScene.AddChild(projectile);
		}
	}

	private void EmitOmnishootStartParticles()
	{
		foreach(GpuParticles2D omnishootStartParticle in OmnishootStartParticles)
		{
			omnishootStartParticle.Emitting = true;
		}		
	}

	private void StopOmnishootStartParticles()
	{
		foreach(GpuParticles2D omnishootStartParticle in OmnishootStartParticles)
		{
			omnishootStartParticle.Emitting = false;
		}	
	}

	public void _on_shoot_interval_timer_timeout()
	{
		CanShoot = true;
	}	

	public void _on_omnishoot_timer_timeout()
	{
		EnterState(BossState.Idle);
		StopOmnishootStartParticles();
	}

	public void _on_start_up_omni_shoot_timer_timeout()
	{
		OmnidirectionShoot();
		OmnishootTimer.Start();
	}

	public void Die()
	{
		CallDeferred("queue_free");
	}

	public void TakeDamage(int amount)
	{
		animation.Play("hit");
		sprite2D.Play("hit");
		EmmitHitParticles();
		gameManager.ShakeCamera(0.5f, 0.2f);
		Health -= amount;		
	}

	private void EmmitHitParticles()
	{
		var hitParticles = HitParticlesScene.Instantiate<GpuParticles2D>();
		GetParent().AddChild(hitParticles);
		hitParticles.GlobalPosition = GlobalPosition;
		hitParticles.Emitting = true;
	   
		hitParticles.Finished += hitParticles.QueueFree;  
	}

	public void _on_boss_hitbox_area_entered(Area2D area)
	{
		if (area.IsInGroup("Danos"))
		{
			TakeDamage(1);
		}
	}

	public void ActivateBossFight()
	{
		EnterState(BossState.Idle);
	}

}
