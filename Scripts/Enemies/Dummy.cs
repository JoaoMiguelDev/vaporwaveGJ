using Godot;
using System;

public partial class Dummy : CharacterBody2D
{
	[Export] private AnimatedSprite2D animation;
	[Export] private Area2D Hitbox;
	public override void _Ready(){
		
		animation.Animation = "Hit";
		Hitbox.AreaEntered += OnHitBoxEntered;
		
	}

	

	private async void OnHitBoxEntered(Area2D area){
		GD.Print("AAAAAAAAA");
		animation.Play();
		await ToSignal(GetTree().CreateTimer(2.0f), Timer.SignalName.Timeout);
		animation.Stop();
	}
}
