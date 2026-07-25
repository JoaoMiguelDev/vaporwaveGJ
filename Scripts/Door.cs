using Godot;
using System;

public partial class Door : StaticBody2D
{
	[Export] private CollisionShape2D collision;
	[Export] private Sprite2D sprite;
	private bool opened = true;

    public override void _Ready()
    {
			
    }

	public void Open()
	{
		if(opened)
			return;

		opened = true;

		sprite.Visible = false;
		collision.CallDeferred("set_disabled", true);
	}

	public void Close()
	{
		if (!opened)
			return;

		opened = false;

		sprite.Visible = true;
		collision.CallDeferred("set_disabled", false);
	}

}
