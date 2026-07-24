using Godot;
using System;

public partial class TrialRoom : Area2D
{
	[Export] public Godot.Collections.Array<Door> Doors { get; set; } = new();

	private void StartTrial()
	{
		foreach(Door door in Doors)
		{
			door.Close();
		}

		//Continue here
	}
}
