using Godot;
using System;

public partial class Detonator : CharacterBody2D
{
	[Signal] public delegate void HealthChangedEventHandler(int current, int max);
	[Signal] public delegate void DiedEventHandler();
	[Export] private Timer CanTakeDamageTimer;
	[Export] private AnimatedSprite2D sprite2D;
	[Export] private GameManager gameManager;
	[Export] private AnimationPlayer animation;
	[Export] private PackedScene HitParticlesScene;
	[Export] private PackedScene DieParticlesScene;
	[Export] private PackedScene DashParticlesScene;
	public const float Speed = 150.0f;
	public const float DashSpeed = 400f;
	public const float DashTime = 0.2f;
	private bool CanDash = true;
	private float DashTimer = 0f;
	private Vector2 DashDir = Vector2.Zero;
	private const float DashReloadTime = 2f;
	private float DashReloadTimer = 0; 
	private bool HitFlag;

	//Animation shenanigans

	private enum AnimationState { Idle, Walk, Dash, Die }
	private AnimationState CurrentAnimationState = AnimationState.Idle;

	//Health variables
	private const int MaxHealth = 3;
	private int _health = 3;
	public int Health
	{
		get => _health;
		private set
		{
			_health = Mathf.Clamp(value, 0, MaxHealth);
			EmitSignal(SignalName.HealthChanged, _health, MaxHealth);
			if (_health == 0)
			{
				EmitSignal(SignalName.Died);
				Die();
			} 
		}
	}

	private bool IsDead = false;
	private bool CanTakeDamage = true;

	public override void _PhysicsProcess(double delta)
	{
		if(HitFlag){
			TakeDamage(1);
		}
		
		
		if(IsDead)
			return;

		var deltaF = (float)delta;
		Vector2 direction = Input.GetVector("left", "right", "up", "down");

		if (DashTimer > 0f)
		{
			DashTimer -= deltaF;
			Velocity = DashDir * DashSpeed;
			PlayAnimation(AnimationState.Dash);
		}
		else
		{
			if (!CanDash)
			{
				DashReloadTimer -= deltaF;
				if (DashReloadTimer <= 0f)
				{
					CanDash = true;
				}
			}

			if (CanDash && Input.IsActionJustPressed("dash") && direction != Vector2.Zero)
			{
				GD.Print("Tô dando dash");
				EmmitDashParticles();
				CanDash = false;
				DashTimer = DashTime;
				DashReloadTimer = DashReloadTime;
				DashDir = direction.Normalized();
				Velocity = DashDir * DashSpeed;
				PlayAnimation(AnimationState.Dash);
			}
			else
			{
				FlipSprite(direction);
				if (direction != Vector2.Zero)
				{
					Velocity = direction.Normalized() * Speed;
					PlayAnimation(AnimationState.Walk);
				}
				else
				{
					Velocity = Vector2.Zero;
					PlayAnimation(AnimationState.Idle);
				}
			}
		}

		MoveAndSlide();
	}

	private void PlayAnimation(AnimationState newState)
	{	
		if (CurrentAnimationState == newState)
			return;

		CurrentAnimationState = newState;
		
		string animationName = newState switch
		{
		    AnimationState.Idle => "idle",
		    AnimationState.Walk => "walk",
		    AnimationState.Dash => "dash",
		    AnimationState.Die => "die",
		    _ => "idle"
		};

		sprite2D.Play(animationName);
		
	}

	private void FlipSprite(Vector2 direction)
	{
		if (direction == Vector2.Zero)
			return;

		if (direction.X < 0)
		{
			sprite2D.FlipH = true;
		}
		else if (direction.X > 0)
		{
			sprite2D.FlipH = false;
		}
	
	}

	public void Die()
	{
		if (IsDead)
			return;
		IsDead = true;
		EmmitDieParticles();
		PlayAnimation(AnimationState.Die);
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		GetNode<Node2D>("Sword").QueueFree();
	}

	public void TakeDamage(int amount)
	{
		if(IsDead)
			return;
		if(!CanTakeDamage)
			return;

		CanTakeDamage = false;
		animation.Play("hit");
		EmmitHitParticles();
		gameManager.ShakeCamera(0.5f, 0.2f);
		CanTakeDamageTimer.Start();
		Health -= amount;
		
	}

	public void _on_can_take_damage_timer_timeout()
	{
		CanTakeDamage = true;
	}
	private void OnHitBoxEntered(Area2D area)
	{
		if(area.IsInGroup("Enemies"))
		{
			HitFlag = true;
		}	
	
	}
	
	private void OnHitBoxExited(Area2D area){
		if(area.IsInGroup("Enemies"))
		{
			HitFlag = false;
		}
	}


	private void EmmitHitParticles()
	{
		var hitParticles = HitParticlesScene.Instantiate<GpuParticles2D>();
		GetParent().AddChild(hitParticles);
		hitParticles.GlobalPosition = GlobalPosition;
		hitParticles.Emitting = true;
	   
		hitParticles.Finished += hitParticles.QueueFree;  
	}

	private void EmmitDieParticles()
	{
		var dieParticles = DieParticlesScene.Instantiate<GpuParticles2D>();
		GetParent().AddChild(dieParticles);
		dieParticles.GlobalPosition = GlobalPosition;
		dieParticles.Emitting = true;
	   
		dieParticles.Finished += dieParticles.QueueFree;  
	}

	private void EmmitDashParticles()
	{
		var dashParticles = DashParticlesScene.Instantiate<GpuParticles2D>();
		GetParent().AddChild(dashParticles);
		dashParticles.GlobalPosition = GlobalPosition;
		dashParticles.Emitting = true;
	   
		dashParticles.Finished += dashParticles.QueueFree;  
	}

}
