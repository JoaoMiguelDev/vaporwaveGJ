using Godot;
using System;

public partial class BossProjectile : CharacterBody2D
{
	[Export] private PackedScene CollisionParticlesScene;
	public const float Speed = 80.0f;
	private Vector2 direction;

	public override void _PhysicsProcess(double delta)
	{
		Velocity = direction * Speed;
		MoveAndSlide();
	}

	public void SetDirection(Vector2 newDirection)
	{
		direction = newDirection.Normalized();
	}

	public void _on_hit_box_body_entered(Node2D body)
	{
		Explode();
	}

	public void _on_hit_box_area_entered(Area2D area)
	{
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
