using Godot;
using System;




public partial class Follower : CharacterBody2D
{
<<<<<<<< HEAD:Scripts/Enemies/Follower.cs
	[Signal] public delegate void FollowerHitEventHandler();
	[Export] private Area2D followerArea;
	[Export] private Area2D followArea;
	public const float speed = 80.0f;
========
	public const float speed = 80.0f;
	private int vida = 2;
>>>>>>>> origin/Adônis:Scripts/Enemy/Follower.cs
	private CharacterBody2D Player;
	private bool Playerflag;
	
<<<<<<<< HEAD:Scripts/Enemies/Follower.cs
	public override void _Ready()
	{
========
	[Signal] public delegate void FollowerDeathEventHandler();


	public override void _Ready(){
		Area2D followerArea = GetNode<Area2D>("FollowerHitArea");
		Area2D followArea = GetNode<Area2D>("FollowArea");
>>>>>>>> origin/Adônis:Scripts/Enemy/Follower.cs
		followerArea.AreaEntered += OnAreaEntered;
		Player = GetParent().GetNode<CharacterBody2D>("Detonator");
		followArea.BodyEntered += OnBodyEntered;
		followArea.BodyExited += OnBodyExited;		
	}

	public override void _PhysicsProcess(double delta)
	{
	   Vector2 velocity = Vector2.Zero;
	   
		if (Playerflag && Player != null)
		{
			velocity = Position.DirectionTo(Player.Position) * speed;
		}
		else
		{
			velocity = Vector2.Zero;
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}
<<<<<<<< HEAD:Scripts/Enemies/Follower.cs

	private void OnAreaEntered(Area2D area)
	{
		
========
	
	
	private void OnAreaEntered(Area2D area){
		if(area.IsInGroup("Danos")){
			EmitSignal(SignalName.FollowerHit);
			vida -= 1;
			
			if (vida == 0){
				morrer();
			}
		}
	
>>>>>>>> origin/Adônis:Scripts/Enemy/Follower.cs
	}
	private void OnBodyEntered(Node2D body)
	{
		if(body.Name == "Detonator")
		{
			Playerflag = true;
		}
	
	}
	
	private void OnBodyExited(Node2D body)
	{
		if(body.Name == "Detonator"){
			Playerflag = false;
		}
	
	}
	private void morrer(){
		EmitSignal(SignalName.FollowerDeath);
		QueueFree();
		
	}
}
