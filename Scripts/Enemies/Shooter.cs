using Godot;
using System;

public partial class Shooter : CharacterBody2D
{
	[Export] private PackedScene ProjectileScene;
	[Export] private Timer ShootTimer;
	public const float speed = 50.0f;
	private int vida = 1;
	private CharacterBody2D Player;
	private bool Playerflag;
	private bool CanShoot = true;
	[Signal]
	public delegate void ShooterHitEventHandler();
	
	[Signal] public delegate void ShooterDeathEventHandler();


	public override void _Ready(){
		Area2D ShooterHit = GetNode<Area2D>("ShooterHitArea");
		Area2D ShooterArea = GetNode<Area2D>("ShooterRunArea");
		ShooterHit.AreaEntered += OnAreaEntered;
		Player = GetParent().GetNode<CharacterBody2D>("Detonator");
		ShooterArea.BodyEntered += OnBodyEntered;
		ShooterArea.BodyExited += OnBodyExited;	
	}



	public override void _PhysicsProcess(double delta)
	{
	   Vector2 velocity = Vector2.Zero;
	   
	   
		if (Playerflag && Player != null){
			velocity = -Position.DirectionTo(Player.Position) * speed;
			
		}
		else{
			velocity = Vector2.Zero;
		}
		
		Shoot();

		Velocity = velocity;
		MoveAndSlide();
	}
	
	
	private void OnAreaEntered(Area2D area){
		if(area.IsInGroup("Danos")){
			EmitSignal(SignalName.ShooterHit);
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
		QueueFree();	
	}
	
	private void Shoot()
	{
		if(!CanShoot) return;
		
		CanShoot = false;
		ShootTimer.Start();
		GD.Print("To atirando porra");
		var projectile = ProjectileScene.Instantiate<Projectile>();
		projectile.GlobalPosition = Position;
		GetTree().CurrentScene.AddChild(projectile);
		
	}
	
	public void _on_shoot_timer_timeout(){
		CanShoot = true;
	}
}
