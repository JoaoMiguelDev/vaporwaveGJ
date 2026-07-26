using Godot;
using System;

public partial class Sword : Node2D
{
	[Export] private AnimationPlayer Animation;
	[Export] private AudioStreamPlayer SfxSlash;
	// [Export] private Area2D Hitbox;
	private bool CanSlash = true;
	public override void _Process(double delta)
	{
		LookAt(GetGlobalMousePosition());
	}

    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("attack") && CanSlash)
		{
			Animation.Play("slash");
			SfxSlash.Play();
			CanSlash = false;
		}
    }

	public void _on_animation_player_animation_finished(StringName anim_name)
	{
		if(anim_name == "slash")
		{
			// CanSlash = true;
			Animation.Play("return");
		}
		if(anim_name == "return")
		{
			CanSlash = true;
		}
	}

}
