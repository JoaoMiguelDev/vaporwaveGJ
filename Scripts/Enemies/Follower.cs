using Godot;

public partial class Follower : CharacterBody2D
{
	[Signal]public delegate void FollowerHitEventHandler();

	[Signal]public delegate void FollowerDeathEventHandler();

	[Export]private Area2D followerArea;

	[Export]private Area2D followArea;

	[Export] private PackedScene HitParticlesScene;
	[Export] private PackedScene DieParticlesScene;
	[Export] private AnimatedSprite2D Sprite2D;
	[Export] private AnimationPlayer animation;
	
	private enum AnimationState { Idle, Walk }
	private AnimationState CurrentAnimationState = AnimationState.Idle;

	public const float Speed = 75.0f;
	private int vida = 2;

	private CharacterBody2D player;
	private GameManager gameManager;
	private bool playerFlag;
	private Vector2 direction;
	

	public override void _Ready()
	{
		
		followerArea = GetNode<Area2D>("FollowerHitArea");
		followArea = GetNode<Area2D>("FollowArea");

		followerArea.AreaEntered += OnAreaEntered;
		followArea.BodyEntered += OnBodyEntered;
		followArea.BodyExited += OnBodyExited;

		player = GetTree().CurrentScene?.FindChild("Detonator", true, false) as CharacterBody2D;
		gameManager = GetTree().CurrentScene?.GetNodeOrNull<GameManager>("GameManager");

	}

	public override void _PhysicsProcess(double delta)
	{

		Vector2 velocity = Vector2.Zero;
		direction = GlobalPosition.DirectionTo(player.GlobalPosition).Normalized();
		
		if (playerFlag && player != null)
		{
			PlayAnimation(AnimationState.Walk);
			FlipSprite(direction);
			velocity = direction  * Speed;	
			
		}
		else{
			PlayAnimation(AnimationState.Idle);
		}
		
		

		Velocity = velocity;
		MoveAndSlide();
	}

	private void OnAreaEntered(Area2D area)
	{

		
		if (area.IsInGroup("Danos"))
		{
			
			EmitSignal(SignalName.FollowerHit);
			gameManager.ShakeCamera(0.1f, 0.2f);
			EmmitHitParticles();
			animation.Play("hit");
			AudioManager.Instance.PlaySfxHit();
			vida--;

			if (vida <= 0)
			{
				Morre();
			}
		}

	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Detonator")
		{
			playerFlag = true;
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.Name == "Detonator")
		{
			playerFlag = false;
		}
	
	}



	private void Morre()
	{
		EmitSignal(SignalName.FollowerDeath);
		EmmitDieParticles();
		AudioManager.Instance.PlaySfxExplosion();
		QueueFree();
	}

	private void EmmitHitParticles()
	{
		var hitParticles = HitParticlesScene.Instantiate<GpuParticles2D>();
		GetParent().AddChild(hitParticles);
		hitParticles.GlobalPosition = GlobalPosition;
		hitParticles.Emitting = true;
	   
		hitParticles.Finished += hitParticles.QueueFree;  
	}

	private void EmmitDieParticles()
	{
		var dieParticles = DieParticlesScene.Instantiate<GpuParticles2D>();
		GetParent().AddChild(dieParticles);
		dieParticles.GlobalPosition = GlobalPosition;
		dieParticles.Emitting = true;
	   
		dieParticles.Finished += dieParticles.QueueFree;  
	}
	
	private void PlayAnimation(AnimationState newState)
	{	
		if (CurrentAnimationState == newState)
			return;

		CurrentAnimationState = newState;
		
		string animationName = newState switch
		{
			AnimationState.Idle => "Idle",
			AnimationState.Walk => "Walk",
			
			_ => "Idle"
		};

		Sprite2D.Play(animationName);
		
	}
	private void FlipSprite(Vector2 direction)
	{
		if (direction == Vector2.Zero)
			return;

		if (direction.X < 0)
		{
			Sprite2D.FlipH = true;
		}
		else if (direction.X > 0)
		{
			Sprite2D.FlipH = false;
		}
	
	}

}
