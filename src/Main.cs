// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using Godot;
using System;

public class Main : Spatial {

	public AudioStreamPlayer Audio { get; } = new AudioStreamPlayer();
	private bool isTouchingScreen = false;

	private void InitSound() {
		if (!Lib.Node.SoundEnabled) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}

	public override void _Notification(int what) {
		if (what is MainLoop.NotificationWmGoBackRequest) {
			GetTree().ChangeScene("res://scenes/Menu.tscn");
		}
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton te) {
			isTouchingScreen = te.Pressed ? true : false;
		}
	}

	public override void _Ready() {
		GetNode<WorldEnvironment>("sky").Environment.BackgroundColor = new Color(Lib.Node.BackgroundColorHtmlCode);
		InitSound();
		AddChild(Audio);

		var arrow = GetNode("Arrow").GetNode<RigidBody>("ArrowPhysics");
		arrow.AxisLockLinearX = arrow.AxisLockLinearY = arrow.AxisLockLinearZ = arrow.AxisLockAngularX = arrow.AxisLockAngularZ = true;
		arrow.RotateY(Mathf.Deg2Rad((float)GD.RandRange(0, 360)));
		arrow.AngularDamp = 0.5f;

		var arrowMesh = GetNode("Arrow").GetNode<RigidBody>("ArrowPhysics").GetNode("ArrowCollision").GetNode<MeshInstance>("Arrow");
		arrowMesh.MaterialOverride = new ShaderMaterial() {
			Shader = new Shader() {
				Code = @"
					shader_type spatial;
						void fragment(){
							float rotatedX = UV.x * cos(1.57) - UV.y * sin(1.57);
							float rotatedY = UV.x * sin(1.57) + UV.y * cos(1.57);
							ALBEDO = vec3(0f, 0f, rotatedY);
							EMISSION = vec3(0.3, 0.3, 0.3);
						}"
			}
		};

		GetNode("Rose").GetNode<MeshInstance>("Rose").MaterialOverride = new ShaderMaterial() {
			Shader = new Shader() {
				Code = @"
					shader_type spatial;
						void fragment(){
							ALBEDO = vec3(0f, SCREEN_UV);
							EMISSION = vec3(0f, SCREEN_UV);
						}"
			}
		};
	}

	public override void _Process(float delta) {
		if (isTouchingScreen) {
			GetNode("Arrow").GetNode<RigidBody>("ArrowPhysics").ApplyTorqueImpulse(new Vector3(0, 1, 0));
		}
	}

}
