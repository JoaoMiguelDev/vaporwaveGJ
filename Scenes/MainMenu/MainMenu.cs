using Godot;
using System;

public partial class MainMenu : Godot.Control
{
	[Export] private Button start;
	[Export] private Button exit;
	private PackedScene start_level = GD.Load<PackedScene>("res://Scenes/Levels/level_1.tscn");
	
	
	
	public override void _Ready()
	{
		start.ButtonDown += OnStartButtonDown;
		exit.ButtonDown += OnExitButtonDown;
	}

	private void OnStartButtonDown(){
		GetTree().ChangeSceneToPacked(start_level);
	}
	
	private void OnExitButtonDown(){
		GetTree().Quit();
	}
}
