using Godot;
using System;

public partial class Hud : Control
{
	[Export] public Godot.Collections.Array<BatteryHud> Batteries { get; set; } = new();
	[Export] private Sprite2D IconSprite;
	public void UpdateBatteries(int current)
	{
		if(current <= 1)
			UpdateIcon(current);

		for(int i = 0; i < Batteries.Count; i++)
    	{
       		if(i < current) Batteries[i].Refill();
        	else Batteries[i].Empty();	
    	}
	}

	private void UpdateIcon(int current)
	{
		switch (current)
		{
			case 0:
				IconSprite.Frame = 2;
				break;
			case 1:
				IconSprite.Frame = 1;
				break;
		}
	}
}
