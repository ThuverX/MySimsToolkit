using Godot;
using System;

public partial class MenuController : Control
{
	[Export]
	public Button MinimizeButton;
	
	[Export]
	public Button MaximizeButton;
	
	[Export]
	public Texture2D MaximizeMaximizeIcon;
	[Export]
	public Texture2D MaximizeMinimizeIcon;
	
	[Export]
	public Button CloseButton;

	[Export] public Control DeadSpace;

	private bool _mouseOverDeadspace;
	
	private TextureRect _maximizeTextureRect;
	
	public override void _Ready()
	{
		_maximizeTextureRect = MaximizeButton.GetNode<TextureRect>("TextureRect");
		_maximizeTextureRect.Texture = MaximizeMaximizeIcon;
		_maximizeTextureRect.Scale = new Vector2(0.6f, 0.6f);
		
		MinimizeButton.Pressed += () => GetTree().Root.Mode = Window.ModeEnum.Minimized;
		MaximizeButton.Pressed += () =>
		{
			var newState = GetTree().Root.Mode == Window.ModeEnum.Maximized
				? Window.ModeEnum.Windowed
				: Window.ModeEnum.Maximized;
			;
			GetTree().Root.Mode = newState;

			if (newState == Window.ModeEnum.Windowed)
			{
				_maximizeTextureRect.Texture = MaximizeMaximizeIcon;
				_maximizeTextureRect.Scale = new Vector2(0.6f, 0.6f);
			}
			else
			{
				_maximizeTextureRect.Texture = MaximizeMinimizeIcon;
				_maximizeTextureRect.Scale = new Vector2(0.8f, 0.8f);
			}
		};
		CloseButton.Pressed += () => GetTree().Quit();
		
		DeadSpace.MouseEntered += () => _mouseOverDeadspace = true;
		DeadSpace.MouseExited += () => _mouseOverDeadspace = false;
	}

	private Vector2 _baseOffset = Vector2.Zero;
	private bool _isDragging;

	public override void _Process(double delta)
	{
		if (_mouseOverDeadspace && Input.IsActionJustPressed("mouse_click"))
		{
			_baseOffset = GetGlobalMousePosition();
			_isDragging = true;
		}

		if (Input.IsActionJustReleased("mouse_click"))
		{
			_isDragging = false;
		}

		if (_isDragging && Input.IsActionPressed("mouse_click"))
		{
			var localMousePosition = GetGlobalMousePosition() - _baseOffset;
			DisplayServer.WindowSetPosition(
				DisplayServer.WindowGetPosition() + new Vector2I((int)localMousePosition.X, (int)localMousePosition.Y)
			);
		}
	}
}
