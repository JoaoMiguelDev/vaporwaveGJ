using Godot;
using System;

public partial class MainMenu : Godot.Control
{
	[Export] private Button start;
	[Export] private AnimatedSprite2D art;
	private PackedScene start_level = GD.Load<PackedScene>("res://Scenes/Levels/level_1.tscn");
	
	
	
	public override void _Ready()
	{
		start.ButtonDown += OnStartButtonDown;
		start.MouseEntered += OnMouseEntered;
		start.MouseExited += OnMouseExited;
		
	}

	private void OnStartButtonDown(){
		GetTree().ChangeSceneToPacked(start_level);
	}
	
	private void OnMouseEntered(){
		art.Animation = "Activated";
	}

	private void OnMouseExited(){
		art.Animation = "Normal";
	}
}
