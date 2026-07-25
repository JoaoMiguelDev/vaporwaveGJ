using Godot;
using System;

public partial class BossLaser : RayCast2D
{
    [Export]
    public float CastSpeed { get; set; } = 500.0f;
    [Export]
    public float MaxLength { get; set; } = 2000.0f;
    [Export]
    public float StartDistance { get; set; } = 10.0f;

    [Export]
    public float GrowthTime { get; set; } = 0.1f;

    private Color _color = Colors.White;
    [Export]
    public Color Color
    {
        get => _color;
        set => SetColor(value);
    }

    private bool _isCasting;
    [Export]
    public bool IsCasting
    {
        get => _isCasting;
        set => SetIsCasting(value);
    }

    private Tween _tween;
    [Export] private Line2D _line2D;
	[Export] private GpuParticles2D CollisionParticles;
	private float _lineWidth;

    public override void _Ready()
    {
        // _castingParticles = GetNode<GpuParticles2D>("%CastingParticles2D");
        // _collisionParticles = GetNode<GpuParticles2D>("%CollisionParticles2D");
        // _beamParticles = GetNode<GpuParticles2D>("%BeamParticles2D");

        _lineWidth = _line2D.Width;

        SetColor(_color);
        SetIsCasting(_isCasting);

        _line2D.Points = new Vector2[]
        {
            Vector2.Right * StartDistance,
            Vector2.Zero
        };
        _line2D.Visible = false;
        // _castingParticles.Position = _line2D.Points[0];

        if (!Engine.IsEditorHint())
        {
            SetPhysicsProcess(false);
        }
		
    }

    public override void _PhysicsProcess(double delta)
    {
        TargetPosition = TargetPosition.MoveToward(
            Vector2.Right * MaxLength,
            CastSpeed * (float)delta
        );

        Vector2 laserEndPosition = TargetPosition;
        ForceRaycastUpdate();

        if (IsColliding())
        {
            laserEndPosition = ToLocal(GetCollisionPoint());
            // CollisionParticles.GlobalRotation = GetCollisionNormal().Angle();
            CollisionParticles.Position = laserEndPosition;

            if (GetCollider() is Detonator detonator)
                detonator.TakeDamage(1);
            
        }

        _line2D.Points = new Vector2[]
        {
            _line2D.Points[0],
            laserEndPosition
        };

        Vector2 laserStartPosition = _line2D.Points[0];
        // _beamParticles.Position = laserStartPosition +
        //     (laserEndPosition - laserStartPosition) * 0.5f;

        // if (_beamParticles.ProcessMaterial is ParticleProcessMaterial processMaterial)
        // {
        //     processMaterial.EmissionBoxExtents = new Vector3(
        //         laserEndPosition.DistanceTo(laserStartPosition) * 0.5f,
        //         processMaterial.EmissionBoxExtents.Y,
        //         processMaterial.EmissionBoxExtents.Z
        //     );
        // }

        CollisionParticles.Emitting = IsColliding();
    }

    private void SetIsCasting(bool newValue)
    {
        if (_isCasting == newValue)
            return;

        _isCasting = newValue;
        SetPhysicsProcess(_isCasting);

        // if (_beamParticles == null)
        //     return;

        // _beamParticles.Emitting = _isCasting;
        // _castingParticles.Emitting = _isCasting;

        if (_isCasting)
        {
            Vector2 laserStart = Vector2.Right * StartDistance;
            // _line2D.Points = new Vector2[] { laserStart, laserStart };
            // _castingParticles.Position = laserStart;

            Appear();
        }
        else
        {
            TargetPosition = Vector2.Zero;
            CollisionParticles.Emitting = false;
            Disappear();
        }
    }

    public void Appear()
    {
        _line2D.Visible = true;

        _tween?.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(_line2D, "width", _lineWidth, GrowthTime * 2.0f)
            .From(0.0f);
    }

    public void Disappear()
    {
        _tween?.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(_line2D, "width", 0.0f, GrowthTime)
            .FromCurrent();
        _tween.TweenCallback(Callable.From(_line2D.Hide));
    }

    private void SetColor(Color newColor)
    {
        _color = newColor;

        if (_line2D == null)
            return;

        _line2D.Modulate = newColor;
        // _castingParticles.Modulate = newColor;
        // _collisionParticles.Modulate = newColor;
        // _beamParticles.Modulate = newColor;
    }			
}
