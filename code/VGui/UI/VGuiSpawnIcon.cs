using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public class VGuiSpawnIcon : Panel
	{
		private Scene _scenePanel;
		private SceneWorld _world;
		private Model _model;
		private SceneObject _modelObject;
		private Angles _modelAngle = Angles.Zero;
		private VGuiContextList _context;
		private string _modelPath;

		public VGuiSpawnIcon()
		{
			StyleSheet.Load( "/VGui/UI/VGuiSpawnIcon.scss" );

			_scenePanel = Add.Scene( null, Vector3.Zero, Angles.Zero, 45f, "modelicon" );
		}

		public Action CustomCallback { get; set; }

		public override void Tick()
		{
			base.Tick();

			if ( _world == null )
			{
				return;
			}

			if ( this.HasHovered != !_scenePanel.RenderOnce )
			{
				_scenePanel.RenderOnce = !this.HasHovered;
			}

			if ( this.HasHovered )
			{
				_modelAngle += new Angles( 0f, Time.Delta * 55f, 0f );
				var transform = _modelObject.Transform;
				transform.Rotation = Rotation.From( _modelAngle );
				_modelObject.Transform = transform;
			}
		}

		protected override void OnRightClick( MousePanelEvent e )
		{
			base.OnRightClick( e );

			if ( _context != null )
			{
				_context?.Delete();
				_context = null;
				return;
			}

			_context = VGuiContextList.Create( this, ( name, value ) =>
			{
				// TODO make it copy to clipboard when they add the feature...
			} );
			_context.AddOption( _modelPath, null );
		}

		public void SetModel( string modelPath )
		{
			_modelPath = modelPath;

			this.AddEventListener( "onclick", ( e ) =>
			{
				_context?.Delete();

				if ( this.CustomCallback == null )
				{
					ConsoleSystem.Run( "spawn", modelPath );
				}
				else
				{
					this.CustomCallback();
				}
				
				e.StopPropagation();
			} );

			_world = new SceneWorld();

			using ( SceneWorld.SetCurrent( _world ) )
			{
				_model = Model.Load( modelPath );
				_modelObject = new SceneObject( _model, Transform.Zero );

				var lightWarm = new SpotLight( Vector3.Up * 10.0f + Vector3.Forward * 100.0f + Vector3.Right * 100.0f, Color.White * 15000.0f );
				lightWarm.Pos = Vector3.Up * 100.0f + Vector3.Forward * 150.0f + Vector3.Right * -200;
				lightWarm.LightColor = new Color( 1.0f, 0.93f, 0.77f ) * 85.0f;
				lightWarm.Ang = Rotation.LookAt( -lightWarm.Pos ).Angles();
				lightWarm.SpotCone = new SpotLightCone { Inner = 90, Outer = 90 };

				var lightBack = new SpotLight( Vector3.Up * 10.0f + Vector3.Forward * 100.0f + Vector3.Right * 100.0f, Color.White * 15000.0f );
				lightBack.Pos = Vector3.Up * 100.0f + Vector3.Forward * -100.0f + Vector3.Right * 100;
				lightBack.LightColor = new Color( 0.1f, 0.3f, 0.4f ) * 30.0f;
				lightBack.Ang = Rotation.LookAt( -lightBack.Pos ).Angles();
				lightBack.SpotCone = new SpotLightCone { Inner = 90, Outer = 90 };

				//var modelRadius = _model.;
				var angles = new Angles( 25, 180, 0 );
				var pos = Vector3.Up * 40 + angles.Direction * -100;

				_scenePanel.World = _world;
				_scenePanel.Position = pos;
				_scenePanel.Angles = angles;
				_scenePanel.FieldOfView = 28;
				_scenePanel.AmbientColor = Color.Gray * 0.2f;

				_scenePanel.RenderOnce = true;
			}
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			_world?.Delete();
			_scenePanel?.Delete();
			_context?.Delete();
		}
	}
}
