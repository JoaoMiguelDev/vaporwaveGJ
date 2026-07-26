using Godot;
using System;

public partial class Projectile : CharacterBody2D
{
	[Export] private PackedScene CollisionParticlesScene;
	public const float Speed = 80.0f;
	private CharacterBody2D Player;
	private Vector2 direction;
	private Vector2 velocity;
	public override void _Ready(){
		Player = GetParent().GetNode<CharacterBody2D>("Detonator");
		direction = Position.DirectionTo(Player.Position);
		Area2D HitArea = GetNode<Area2D>("HitBox");
		HitArea.AreaEntered += OnAreaEntered;
		HitArea.BodyEntered += OnBodyEntered;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		velocity = direction * Speed;
		Velocity = velocity;
		MoveAndSlide();
			
	}
	private void OnAreaEntered(Area2D area){
		Explode();
			
	}
	private void OnBodyEntered(Node2D body){
		Explode();
	}

	private void Explode()
	{
		EmitCollisionParticles();
		QueueFree();
	}

	private void EmitCollisionParticles()
	{
		var collisionParticles = CollisionParticlesScene.Instantiate<GpuParticles2D>();
		GetParent().AddChild(collisionParticles);
		collisionParticles.GlobalPosition = GlobalPosition;
		collisionParticles.Emitting = true;
	   
		collisionParticles.Finished += collisionParticles.QueueFree;  		
	}
}
