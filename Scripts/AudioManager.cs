using Godot;
using System;

public partial class AudioManager : Node
{
	public static AudioManager Instance;
	[Export] private PackedScene SfxHitScene;	
	[Export] private PackedScene SfxExplosionScene;

	public override void _Ready()
	{
		Instance = this;
	}

	public void PlaySfxHit()
	{
		var sfxHit = SfxHitScene.Instantiate<AudioStreamPlayer>();
		GetParent().AddChild(sfxHit);
		sfxHit.Play();
	   
		sfxHit.Finished += sfxHit.QueueFree;  
	}

	public void PlaySfxExplosion()
	{
		var sfxExplosion = SfxExplosionScene.Instantiate<AudioStreamPlayer>();
		GetParent().AddChild(sfxExplosion);
		sfxExplosion.Play();
	   
		sfxExplosion.Finished += sfxExplosion.QueueFree;  
	}
}
