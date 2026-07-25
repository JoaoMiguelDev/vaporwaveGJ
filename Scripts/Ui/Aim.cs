using Godot;
using System;

public partial class Aim : Sprite2D
{
	public override void _Process(double delta)
	{
		GlobalPosition = GetGlobalMousePosition();
	}
}
