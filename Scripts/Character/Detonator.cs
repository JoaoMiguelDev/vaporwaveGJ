using Godot;
using System;

public partial class Detonator : CharacterBody2D
{
	[Signal] public delegate void HealthChangedEventHandler(int current, int max);
	[Signal] public delegate void DiedEventHandler();
	[Export] private Timer CanTakeDamageTimer;
	public const float Speed = 150.0f;
	public const float DashSpeed = 400f;
	public const float DashTime = 0.12f;
	private bool CanDash = true;
	private float DashTimer = 0f;
	private Vector2 DashDir = Vector2.Zero;
	private const float DashReloadTime = 2f;
	private float DashReloadTimer = 0; 

	//Health variables
	private const int MaxHealth = 3;
	private int _health = 3;
	public int Health
	{
    	get => _health;
    	private set
    	{
        	_health = Mathf.Clamp(value, 0, MaxHealth);
        	EmitSignal(SignalName.HealthChanged, _health, MaxHealth);
        	if (_health == 0)
			{
				EmitSignal(SignalName.Died);
				Die();
			} 
    	}
	}

	private bool IsDead = false;
	private bool CanTakeDamage = true;

	public override void _PhysicsProcess(double delta)
	{
		if(IsDead)
			return;
			
	    var deltaF = (float)delta;
	    Vector2 direction = Input.GetVector("left", "right", "up", "down");

	    if (DashTimer > 0f)
	    {
	        DashTimer -= deltaF;
	        Velocity = DashDir * DashSpeed;
	    }
	    else
	    {
	        if (!CanDash)
	        {
	            DashReloadTimer -= deltaF;
	            if (DashReloadTimer <= 0f)
	            {
	                CanDash = true;
	            }
	        }

	        if (CanDash && Input.IsActionJustPressed("dash") && direction != Vector2.Zero)
	        {
				GD.Print("Tô dando dash");
	            CanDash = false;
	            DashTimer = DashTime;
	            DashReloadTimer = DashReloadTime;
	            DashDir = direction.Normalized();
	            Velocity = DashDir * DashSpeed;
	        }
	        else
	        {
	            Velocity = direction != Vector2.Zero
	                ? direction.Normalized() * Speed
	                : Vector2.Zero;
	        }
	    }

	    MoveAndSlide();
	}

	public void Die()
    {
		if (IsDead)
			return;
		IsDead = true;
    }

	public void TakeDamage(int amount)
	{
		if(IsDead)
			return;
		if(!CanTakeDamage)
			return;

		CanTakeDamage = false;
		CanTakeDamageTimer.Start();
        Health -= amount;
	}

	public void _on_can_take_damage_timer_timeout()
	{
		CanTakeDamage = true;
	}
}
