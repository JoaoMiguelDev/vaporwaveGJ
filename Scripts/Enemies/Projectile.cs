using Godot;
using System;

public partial class Projectile : CharacterBody2D
{
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
			
			QueueFree();
			
	}
	private void OnBodyEntered(Node2D body){
		QueueFree();
	}
}
