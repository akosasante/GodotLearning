using Godot;
using System;

public class Main : Node
{
	[Export]
	public PackedScene Mob;

	private AudioStreamPlayer _bgMusic;

	private AudioStreamPlayer _deathSound;

	private int _score;

	// We use 'System.Random' as an alternative to GDScript's random methods.
	private Random _random = new Random();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_bgMusic = GetNode<AudioStreamPlayer>("BgMusic");
		_deathSound = GetNode<AudioStreamPlayer>("DeathSound");
	}

	// We'll use this later because C# doesn't support GDScript's randi().
	private float RandRange(float min, float max)
	{
		return (float)_random.NextDouble() * (max - min) + min;
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
	private void GameOver()
	{
		_bgMusic.Stop();
		_deathSound.Play();
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<HUD>("HUD").ShowGameOver();
		GetTree().CallGroup("mobs", "queue_free");
	}

	public void NewGame()
	{
		_score = 0;
		_bgMusic.Play();

		var hud = GetNode<HUD>("HUD");
		hud.UpdateScore(_score);
		hud.ShowMessage("Get Ready!");

		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Position2D>("PlayerStartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();
	}

	private void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}

	private void OnScoreTimerTimeout()
	{
		_score++;
		GetNode<HUD>("HUD").UpdateScore(_score);
	}

	private void OnMobTimerTimeout()
	{
		// Choose a random location on Path2D.
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.Offset = _random.Next();

		// Create a Mob instance and add it to the scene.
		var mobInstance = (RigidBody2D)Mob.Instance();
		AddChild(mobInstance);

		// Set the mob's direction perpendicular to the path direction.
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

		// Add some randomness to the direction.
		direction += RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mobInstance.Rotation = direction;

		// Set the mob's position to the current position of path2dfollow
		mobInstance.Position = mobSpawnLocation.Position;

		// Choose the velocity.
		mobInstance.LinearVelocity = new Vector2(RandRange(150f, 250f), 0).Rotated(direction);
	}

}
