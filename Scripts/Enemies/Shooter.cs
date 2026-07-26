using Godot;
using System;

public partial class Shooter : CharacterBody2D
{
	[Export] private PackedScene ProjectileScene;
	[Export] private Timer ShootTimer;
	[Export] private PackedScene HitParticlesScene;
	[Export] private PackedScene DieParticlesScene;
	[Export] private AnimationPlayer animation;
	[Export] private AnimatedSprite2D Sprite2D;
	[Export] private AudioStreamPlayer2D ShootSfx;
	public const float speed = 50.0f;
	private int vida = 1;
	private CharacterBody2D Player;
	private GameManager gameManager;
	private bool Playerflag;
	private bool CanShoot = true;
	private Vector2 direction;
	[Signal]
	public delegate void ShooterHitEventHandler();
	
	[Signal] public delegate void ShooterDeathEventHandler();

	private enum AnimationState { Idle }
	private AnimationState CurrentAnimationState = AnimationState.Idle;


	public override void _Ready()
	{
		Area2D ShooterHit = GetNode<Area2D>("ShooterHitArea");
		Area2D ShooterArea = GetNode<Area2D>("ShooterRunArea");
		ShooterHit.AreaEntered += OnAreaEntered;
		ShooterArea.BodyEntered += OnBodyEntered;
		ShooterArea.BodyExited += OnBodyExited;

		Player = GetTree().CurrentScene?.FindChild("Detonator", true, false) as CharacterBody2D;
		gameManager = GetTree().CurrentScene?.GetNodeOrNull<GameManager>("GameManager");
	}



	public override void _PhysicsProcess(double delta)
	{
	   Vector2 velocity = Vector2.Zero;
	   direction = -GlobalPosition.DirectionTo(Player.GlobalPosition);
	   PlayAnimation(AnimationState.Idle);
	
		if (Playerflag && Player != null){
			velocity = direction * speed;
			FlipSprite(direction);
		}
		
		
		Shoot();

		Velocity = velocity;
		MoveAndSlide();
	}
	
	
	private void OnAreaEntered(Area2D area){
		if(area.IsInGroup("Danos")){
			EmitSignal(SignalName.ShooterHit);
			gameManager.ShakeCamera(0.1f, 0.2f);
			EmmitHitParticles();
			animation.Play("hit");
			AudioManager.Instance.PlaySfxHit();
			vida -= 1;
			
			if (vida == 0){
				morrer();
			}
		}
	
	}
	private void OnBodyEntered(Node2D body){
		if(body.Name == "Detonator"){
			Playerflag = true;
		}
	
	}
	
	private void OnBodyExited(Node2D body){
		if(body.Name == "Detonator"){
			Playerflag = false;
		}
	
	}
	private void morrer(){
		EmitSignal(SignalName.ShooterDeath);
		EmmitDieParticles();
		AudioManager.Instance.PlaySfxExplosion();
		QueueFree();	
	}
	
	private void Shoot()
	{
		if(!CanShoot) return;
		
		CanShoot = false;
		ShootSfx.Play();
		ShootTimer.Start();
		var projectile = ProjectileScene.Instantiate<Projectile>();
		projectile.GlobalPosition = GlobalPosition;
		GetTree().CurrentScene.AddChild(projectile);
		
	}
	
	public void _on_shoot_timer_timeout(){
		CanShoot = true;
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
	
	private void PlayAnimation(AnimationState newState)
	{	
		if (CurrentAnimationState == newState)
			return;

		CurrentAnimationState = newState;
		
		string animationName = newState switch
		{
			AnimationState.Idle => "Idle",
			
			
			_ => "Idle"
		};

		Sprite2D.Play(animationName);
		
	}
	private void FlipSprite(Vector2 direction)
	{
		if (direction == Vector2.Zero)
			return;

		if (direction.X < 0)
		{
			Sprite2D.FlipH = true;
		}
		else if (direction.X > 0)
		{
			Sprite2D.FlipH = false;
		}
	
	}

}
