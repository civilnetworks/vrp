using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using VRP.Character;

namespace VRP.VGui.UI
{
	/// <summary>
	/// Panel for rendering character models. Based on the main menu character panel.
	/// </summary>
	[Library]
	class CharacterModelPanel : Panel
	{
		private Model _model;
		private AnimSceneObject _modelObject;
		private readonly List<AnimSceneObject> _clothingObjects = new();
		private SpotLight _lightWarm;
		private SpotLight _lightBack;
		private Vector3 _lookPos = Vector3.Zero;
		private Vector3 _headPos = Vector3.Zero;
		private Vector3 _aimPos = Vector3.Zero;
		private List<(string, string)> _childObjects = new List<(string, string)>();
		private CharacterAppearance _appearance;

		private string _modelPath;
		private bool _dirty;

		public CharacterModelPanel()
		{
			StyleSheet.Load( "/vgui/ui/CharacterModelPanel.scss" );

			this.AvatarScene = Add.Scene( null, Vector3.Zero, Angles.Zero, 45f, "model" );
		}

		public SceneWorld AvatarWorld { get; set; }

		public Scene AvatarScene { get; set; }

		public Vector3 ModelPosition { get; set; } = Vector3.Zero;

		public override void Tick()
		{
			base.Tick();

			if ( !this.IsVisible )
			{
				return;
			}

			if ( _dirty )
			{
				this.CreateAvatarWorld();
				_dirty = false;
			}

			this.UpdateAvatar();
		}

		public void SetModel( string modelPath )
		{
			_modelPath = modelPath;
			_dirty = true;
		}

		public void SetDefaultModel()
		{
			this.SetModel( "models/citizen/citizen.vmdl" );
		}

		public void AddChildObject( string attachment, string modelPath )
		{
			if ( modelPath == null )
			{
				return;
			}

			if ( _childObjects.FirstOrDefault( c => c.Item1.Equals( attachment ) && c.Item2.Equals( modelPath ) ).Item1 != null )
			{
				return;
			}

			_childObjects.Add( (attachment, modelPath) );
			_dirty = true;
		}

		public void RemoveChildObject( string attachment, string modelPath = null )
		{
			if ( modelPath == null )
			{
				var toRemove = _childObjects.Where( c => c.Item1.Equals( attachment ) );
				foreach ( var item in toRemove )
				{
					_childObjects.Remove( item );
				}
			}
			else
			{
				var toRemove = _childObjects.Where( c => c.Item1.Equals( attachment ) && c.Item2.Equals( modelPath ) );
				foreach ( var item in toRemove )
				{
					_childObjects.Remove( item );
				}
			}
		}

		public void ClearChildObjects()
		{
			_childObjects.Clear();
		}

		public void SetCharacterAppearance( CharacterAppearance appearance, CharacterAppearanceSettings settings = null )
		{
			this.SetModel( appearance.Model );
			this.ClearChildObjects();

			foreach ( var pair in appearance.ClothingAttachments )
			{
				var att = pair.Value;

				if ( settings != null && settings.TryGetClothingAttachmentModel( att.Type, out var model ) )
				{
					this.AddChildObject( att.Attachment, model );
				}
				else
				{
					this.AddChildObject( att.Attachment, att.DefaultModel );
				}
			}

			_appearance = appearance;
		}

		private void CreateAvatarWorld()
		{
			/*if ( this.AvatarWorld != null )
			{
				foreach ( var obj in _clothingObjects )
				{
					_modelObject.RemoveChild( obj );
					obj.Delete(); // <- this causes (0xc0000005) 'Access violation'.
				}

				_modelObject?.Delete();
				_modelObject = null;
			}*/

			_clothingObjects.Clear();

			if ( _modelPath == null )
			{
				return;
			}

			this.AvatarWorld?.Delete();
			this.AvatarWorld = new SceneWorld();

			using ( SceneWorld.SetCurrent( this.AvatarWorld ) )
			{
				_model = Model.Load( _modelPath );
				_modelObject = new AnimSceneObject( _model, Transform.Zero );

				// TODO: we shouldn't have to directly work with masks
				var meshGroupMask = ulong.MaxValue;
				meshGroupMask &= ~_model.GetBodyPartMask( "feet" );
				meshGroupMask &= ~_model.GetBodyPartMask( "legs" );
				meshGroupMask &= ~_model.GetBodyPartMask( "chest" );
				_modelObject.MeshGroupMask = meshGroupMask;

				foreach ( var child in _childObjects )
				{
					if ( child.Item2 == null || child.Item2.Length == 0 )
					{
						continue;
					}

					var clothingObject = new AnimSceneObject( Model.Load( child.Item2 ), Transform.Zero );
					_modelObject.AddChild( child.Item1, clothingObject );
					_clothingObjects.Add( clothingObject );
				}

				_lightWarm = new SpotLight( Vector3.Up * 10.0f + Vector3.Forward * 100.0f + Vector3.Right * 100.0f, Color.White * 15000.0f );
				_lightWarm.Ang = Rotation.LookAt( -_lightWarm.Pos ).Angles();
				_lightWarm.SpotCone = new SpotLightCone { Inner = 90, Outer = 90 };

				_lightBack = new SpotLight( Vector3.Up * 10.0f + Vector3.Forward * 100.0f + Vector3.Right * 100.0f, Color.White * 15000.0f );
				_lightBack.Ang = Rotation.LookAt( -_lightBack.Pos ).Angles();
				_lightBack.SpotCone = new SpotLightCone { Inner = 90, Outer = 90 };

				var angles = new Angles( 25, 180, 0 );
				var pos = Vector3.Up * 40 + angles.Direction * -100;

				this.AvatarScene.World = this.AvatarWorld;
				this.AvatarScene.Position = pos;
				this.AvatarScene.Angles = angles;
				this.AvatarScene.FieldOfView = 28;
				this.AvatarScene.AmbientColor = Color.Gray * 0.2f;
			}
		}

		private void UpdateAvatar()
		{
			if ( _modelObject == null )
			{
				return;
			}

			var transform = _modelObject.Transform;
			transform.Position = this.ModelPosition;
			_modelObject.Transform = transform;

			// Get mouse position
			var mousePosition = this.AvatarScene.MousePosition;

			// subtract what we think is about the player's eye position
			mousePosition.x -= this.AvatarScene.Box.Rect.width * 0.5f;
			mousePosition.y -= this.AvatarScene.Box.Rect.height * 0.25f;
			mousePosition /= this.AvatarScene.ScaleToScreen;

			// convert it to an imaginary world position
			var worldpos = new Vector3( 200, mousePosition.x, -mousePosition.y ) - this.ModelPosition * 15f;
			var localPos = _modelObject.Transform.PointToLocal( worldpos );

			// convert that to local space for the model
			_lookPos = localPos;
			_headPos = Vector3.Lerp( _headPos, localPos, Time.Delta * 20.0f );
			_aimPos = Vector3.Lerp( _aimPos, localPos, Time.Delta * 5.0f );

			_modelObject.SetAnimBool( "b_grounded", true );
			_modelObject.SetAnimVector( "aim_eyes", _lookPos );
			_modelObject.SetAnimVector( "aim_head", _headPos );
			_modelObject.SetAnimVector( "aim_body", _aimPos );
			_modelObject.SetAnimFloat( "aim_body_weight", 1.0f );

			_modelObject.Update( Time.Now );

			var angles = new Angles( 15, 180, 0 );
			var pos = Vector3.Up * 45 + angles.Direction * -80;

			this.AvatarScene.Position = pos;
			this.AvatarScene.Angles = angles;

			_lightWarm.Pos = Vector3.Up * 100.0f + Vector3.Forward * 150.0f + Vector3.Right * -200;
			_lightWarm.LightColor = new Color( 1.0f, 0.93f, 0.77f ) * 85.0f;
			_lightWarm.Ang = Rotation.LookAt( -_lightWarm.Pos ).Angles();
			_lightWarm.SpotCone = new SpotLightCone { Inner = 90, Outer = 90 };

			_lightBack.Pos = Vector3.Up * 100.0f + Vector3.Forward * -100.0f + Vector3.Right * 100;
			_lightBack.LightColor = new Color( 0.1f, 0.3f, 0.4f ) * 30.0f;
			_lightBack.Ang = Rotation.LookAt( -_lightBack.Pos ).Angles();
			_lightBack.SpotCone = new SpotLightCone { Inner = 90, Outer = 90 };

			foreach ( var clothingObject in _clothingObjects )
			{
				clothingObject.Update( Time.Now );
			}
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			this.AvatarWorld?.Delete();
		}
	}
}
