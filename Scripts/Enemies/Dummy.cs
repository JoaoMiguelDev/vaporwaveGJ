using Godot;
using System;

public partial class Dummy : CharacterBody2D
{
	[Export] private AnimatedSprite2D Sprite2D;
	[Export] private AnimationPlayer Animation;
	[Export] private Area2D Hitbox;
	[Export] private PackedScene HitParticlesScene;	
	[Export] private AudioStreamPlayer SfxHit;
	public override void _Ready()
	{
		Hitbox.AreaEntered += OnHitBoxEntered;
		
	}

	private async void OnHitBoxEntered(Area2D area){
		GD.Print("AAAAAAAAA");
		Sprite2D.Play("hit");
		Animation.Play("hit");
		SfxHit.Play();
		EmmitHitParticles();
	}

	private void EmmitHitParticles()
	{
		var hitParticles = HitParticlesScene.Instantiate<GpuParticles2D>();
		GetParent().AddChild(hitParticles);
		hitParticles.GlobalPosition = GlobalPosition;
		hitParticles.Emitting = true;
	   
		hitParticles.Finished += hitParticles.QueueFree;  
	}
}
