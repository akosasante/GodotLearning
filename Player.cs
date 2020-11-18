using System;
using Godot;

public class Player : Area2D
{
	[Export]
	public int Speed = 400; // How fast the player will move (pixels/sec)

	[Signal]
	public delegate void Hit();

	private Vector2 _screenSize; // Size of the game window

	// Add this variable to hold the clicked position.
	private Vector2 _target;

	private Vector2 _spriteSize; // Size of sprite

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		_screenSize = GetViewport().Size;
		var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_spriteSize = animatedSprite.Frames.GetFrame("walk", 0).GetSize();
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		var velocity = getUpdatedVelocity();
		playSprite (velocity);
		velocity = velocity.Normalized() * Speed;
		updatePosition (velocity, delta);
	}

	public void Start(Vector2 pos)
	{
		Position = pos;
		_target = pos;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	// Change the target whenever a touch event happens.
	public override void _Input(InputEvent @event)
	{
		if (
			@event is InputEventScreenTouch eventMouseButton &&
			eventMouseButton.Pressed
		)
		{
			_target = (@event as InputEventScreenTouch).Position;
		}
	}

	private Vector2 getUpdatedVelocity()
	{
		var velocity = new Vector2(); // the player's movement vector

		// touch controls
		// if (Position.DistanceTo(_target) > 10)
		// {
		// 	velocity = _target - Position;
		// }
		// Remove keyboard controls in favour of touch controls
		if (Input.IsActionPressed("ui_right"))
		{
			velocity.x += 1;
		}

		if (Input.IsActionPressed("ui_left"))
		{
			velocity.x -= 1;
		}
		if (Input.IsActionPressed("ui_up"))
		{
			velocity.y -= 1;
		}
		if (Input.IsActionPressed("ui_down"))
		{
			velocity.y += 1;
		}

		return velocity;
	}

	private void playSprite(Vector2 velocity)
	{
		var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		var x = GetNode<CollisionShape2D>("CollisionShape2D");

		if (velocity.Length() > 0)
		{
			if (velocity.x != 0)
			{
				animatedSprite.Animation = "walk";
				animatedSprite.FlipV = false;
				animatedSprite.FlipH = velocity.x < 0;
			}
			else if (velocity.y != 0)
			{
				animatedSprite.Animation = "up";
				animatedSprite.FlipV = velocity.y > 0;
			}
			animatedSprite.Play();
		}
		else
		{
			animatedSprite.Stop();
		}
	}

	private void updatePosition(Vector2 velocity, float delta)
	{
		Position += (velocity * delta);
		var QuarterWidth = _spriteSize.x * 0.25f;
		var QuarterHeight = _spriteSize.y * 0.25f;
		Position =
			new Vector2(x: Mathf
					.Clamp(Position.x,
					0 + QuarterWidth,
					_screenSize.x - QuarterWidth),
				y: Mathf
					.Clamp(Position.y,
					0 + QuarterHeight,
					_screenSize.y - QuarterHeight));
	}

	private void _on_Player_body_entered(object body)
	{
		Hide();
		EmitSignal("Hit");
		GetNode<CollisionShape2D>("CollisionShape2D")
			.SetDeferred("disabled", true);
	}
}
