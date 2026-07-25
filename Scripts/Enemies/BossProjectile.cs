using Godot;
using System;

public partial class BossProjectile : CharacterBody2D
{
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
		QueueFree();
	}

	public void _on_hit_box_area_entered(Area2D area)
	{
		QueueFree();
	}	
}
