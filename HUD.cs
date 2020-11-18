using Godot;
using System;

public class HUD : CanvasLayer
{
	private Label _messageNode;

	[Signal]
	public delegate void StartGame();

	public override void _Ready()
	{
		_messageNode = GetNode<Label>("Message");
	}

	async public void ShowGameOver()
	{
		// Show Game Over message, block on the timer (which is set to expire in 2 seconds), then show the title screen
		ShowMessage("Game Over");

		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, "timeout");

		_messageNode.Text = "Dodge the Creeps!";
		_messageNode.Show();

		// alternative to using a timer node, is to just use the scenes "Create Timer" method
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		GetNode<Button>("StartButton").Show();
	}

	public void ShowMessage(string text)
	{
		_messageNode.Text = text;
		_messageNode.Show();

		GetNode<Timer>("MessageTimer").Start();
	}

	public void UpdateScore(int score)
	{
		GetNode<Label>("ScoreLabel").Text = score.ToString();
	}

	public void OnStartButtonPressed()
	{
		GetNode<Button>("StartButton").Hide();
		EmitSignal("StartGame");
	}

	public void OnMessageTimerTimeout()
	{
		GetNode<Label>("Message").Hide();
	}
}
