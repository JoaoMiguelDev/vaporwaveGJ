using Godot;

public partial class Follower : CharacterBody2D
{
    [Signal]
    public delegate void FollowerHitEventHandler();

    [Signal]
    public delegate void FollowerDeathEventHandler();

    [Export]
    private Area2D followerArea;

    [Export]
    private Area2D followArea;

    public const float Speed = 80.0f;
    private int vida = 2;

    private CharacterBody2D player;
    private bool playerFlag;

    public override void _Ready()
    {
        followerArea = GetNode<Area2D>("FollowerHitArea");
        followArea = GetNode<Area2D>("FollowArea");

        followerArea.AreaEntered += OnAreaEntered;
        followArea.BodyEntered += OnBodyEntered;
        followArea.BodyExited += OnBodyExited;

        player = GetParent().GetNode<CharacterBody2D>("Detonator");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Vector2.Zero;

        if (playerFlag && player != null)
        {
            velocity = Position.DirectionTo(player.Position) * Speed;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.IsInGroup("Danos"))
        {
            EmitSignal(SignalName.FollowerHit);
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
        QueueFree();
    }
}