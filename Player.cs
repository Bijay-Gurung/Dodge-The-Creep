using Godot;
using System;
using System.Numerics;

public partial class Player : Area2D
{
	[Export]
	public int speed { get; set; } = 400; // player Movement speed (pixels/secc)

	public Godot.Vector2 ScreenSize; //Size of the Game Window

	[Signal]
	public delegate void HitEventHandler();

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
	}

	public override void _Process(double delta)
	{
		var velocity = Godot.Vector2.Zero; // Player's movement vector

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}
		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}
		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * speed; // To prevent from fast diagonal movement.
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}

		Position += velocity * (float)delta;
		Position = new Godot.Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		/*
		Now that the player can move, we need to change which animation the AnimatedSprite2D is playing based on its direction.
		We have the "walk" animation, which shows the player walking to the right. This animation should be flipped
		horizontally using the flip_h property for left movement. We also have the "up" animation, which should be flipped
		vertically with flip_v for downward movement.
		*/

		if (velocity.X != 0)
		{
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipV = false;
			animatedSprite2D.FlipH = velocity.X < 0;
		}
		else if (velocity.Y != 0)
		{
			animatedSprite2D.Animation = "up";
			animatedSprite2D.FlipV = velocity.Y > 0;
		}

	}

	private void OnBodyEntered(Node2D body)
	{
		 // Player disapper after being hit.
		EmitSignal(SignalName.Hit);
		Hide();
		/* Disabling the area's collision shape can cause an error if it happens in the middle of the engine's collision processing.
		using SetDeferred() tells godot to wait to disable the shape until it's safe to do so.
		*/
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	public void Start(Godot.Vector2 position)
	{
		Position = position;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
}
