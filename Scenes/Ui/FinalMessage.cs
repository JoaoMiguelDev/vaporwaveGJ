using Godot;
using System;

public partial class FinalMessage : Control
{
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("Quit")){
			GetTree().Quit();
		}
	}
}
