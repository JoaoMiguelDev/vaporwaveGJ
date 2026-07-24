using Godot;
using System;




public partial class Follower : CharacterBody2D
{
	public const float speed = 200.0f;
	
	private CharacterBody2D Player;
	private bool Playerflag;
	[Signal]
	public delegate void FollowerHitEventHandler();
	


	public override void _Ready(){
		Area2D followerArea = GetNode<Area2D>("FollowerHitArea");
		Area2D followArea = GetNode<Area2D>("FollowArea");
		followerArea.AreaEntered += OnAreaEntered;
		Player = GetParent().GetNode<CharacterBody2D>("Detonator");
		followArea.BodyEntered += OnBodyEntered;
		followArea.BodyExited += OnBodyExited;
		
	}



	public override void _PhysicsProcess(double delta)
	{
	   Vector2 velocity = Vector2.Zero;
	   
	   
		if (Playerflag && Player != null){
			velocity = Position.DirectionTo(Player.Position) * speed;
		}
		else{
			velocity = Vector2.Zero;
		}
		

		Velocity = velocity;
		MoveAndSlide();
	}
	private void OnAreaEntered(Area2D area){
		
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

}
