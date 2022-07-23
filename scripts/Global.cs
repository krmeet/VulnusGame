using Godot;
using System;

public class Global : Node
{
	public static Global Instance;
	public Node CurrentScene { get; private set; }
	public Control Overlay { get; private set; }

	public override void _Ready()
	{
		Instance = this;
		Viewport root = GetTree().Root;
		CurrentScene = root.GetChild(root.GetChildCount() - 1);
		var overlayScene = (PackedScene)GD.Load("res://scenes/Overlay.tscn");
		Overlay = (Control)overlayScene.Instance();
		CallDeferred(nameof(AddOverlay));
	}
	private void AddOverlay()
	{
		GetTree().Root.AddChild(Overlay);
		GetTree().Root.MoveChild(Overlay, 1);
	}
	public void GotoScene(string path)
	{
		CallDeferred(nameof(DeferredGotoScene), path);
	}
	private void DeferredGotoScene(string path)
	{
		CurrentScene.Free();
		var nextScene = (PackedScene)GD.Load(path);
		CurrentScene = nextScene.Instance();
		GetTree().Root.AddChild(CurrentScene);
		GetTree().Root.MoveChild(CurrentScene, 0);
		GetTree().CurrentScene = CurrentScene;
	}
}