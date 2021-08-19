using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Player;

namespace VVehicle
{
	public class VVehicleCamera : Camera
	{
		protected virtual float MinFov => 80.0f;
		protected virtual float MaxFov => 100.0f;
		protected virtual float MinOrbitPitch => -25.0f;
		protected virtual float MaxOrbitPitch => 70.0f;
		protected virtual float OrbitSmoothingSpeed => 25.0f;
		protected virtual float OrbitDistance => 250f;
		protected virtual float OrbitHeight => 60f;
		protected virtual float CollisionRadius => 5f;

		private TimeSince _sinceOrbit;
		private Angles _orbitAngles;
		private Rotation _orbitYawRot;
		private Rotation _orbitPitchRot;
		private bool _thirdPerson = true;
		private bool _orbiting = true;

		public override void Activated()
		{
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			_orbitAngles = Angles.Zero;
			_orbitYawRot = Rotation.Identity;
			_orbitPitchRot = Rotation.Identity;
		}

		public override void Update()
		{
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			var car = (pawn as VrpPlayer)?.Vehicle as VVehicle;
			if ( !car.IsValid() ) return;

			var body = car.PhysicsBody;
			if ( !body.IsValid() )
				return;

			var dt = Time.Delta;
			var carRot = car.Transform.Rotation;
			var speed = body.Velocity.Length;

			var slerpAmount = Time.Delta * this.OrbitSmoothingSpeed;
			var carPitch = 0f;

			if ( _sinceOrbit > 1)
			{
				_orbiting = false;
			}

			if (_thirdPerson)
			{
				if (!_orbiting )
				{
					var orbitSpeedP = Math.Clamp( speed / 200f, 0f, 1f);
					var targetYaw = carRot.y + 180f;

					_orbitAngles.yaw += (targetYaw - _orbitAngles.yaw) * dt * orbitSpeedP;
				}

				_orbitYawRot = Rotation.Slerp( _orbitYawRot, Rotation.From( 0.0f, _orbitAngles.yaw, 0.0f ), slerpAmount );
				_orbitPitchRot = Rotation.Slerp( _orbitPitchRot, Rotation.From( _orbitAngles.pitch + carPitch, 0.0f, 0.0f ), slerpAmount );

				DoThirdPerson(car, body);
			}
			else
			{
				_orbitYawRot = Rotation.Slerp( _orbitYawRot, Rotation.From( 0.0f, _orbitAngles.yaw, 0.0f ), slerpAmount );
				_orbitPitchRot = Rotation.Slerp( _orbitPitchRot, Rotation.From( _orbitAngles.pitch + carPitch, 0.0f, 0.0f ), slerpAmount );

				DoFirstPerson( );
			}

			FieldOfView = this.MinFov;
		}

		private void DoThirdPerson( VVehicle car, PhysicsBody body )
		{
			Rot = _orbitYawRot * _orbitPitchRot;

			var carPos = car.Position + car.Rotation * (body.LocalMassCenter * car.Scale);
			var startPos = carPos;
			var targetPos = startPos + Rot.Backward * (this.OrbitDistance * car.Scale) + (Vector3.Up * (this.OrbitHeight * car.Scale));

			var tr = Trace.Ray( startPos, targetPos )
				.Ignore( car )
				.Radius( Math.Clamp( this.CollisionRadius * car.Scale, 2.0f, 10.0f ) )
				.WorldOnly()
				.Run();

			Pos = tr.EndPos;

			Viewer = null;
		}

		private void DoFirstPerson( )
		{
			Rot = Local.Pawn.Rotation * (_orbitYawRot * _orbitPitchRot);
			Pos = Local.Pawn.EyePos;

			Viewer = Local.Pawn;
		}

		public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );

			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			var car = (pawn as VrpPlayer)?.Vehicle as VVehicle;
			if ( !car.IsValid() ) return;

			if ( input.Pressed( InputButton.View ) )
			{
				_thirdPerson = !_thirdPerson;
			}

			if ( (Math.Abs( input.AnalogLook.pitch ) + Math.Abs( input.AnalogLook.yaw )) > 0.0f )
			{
				/*if ( !orbitEnabled )
				{
					orbitAngles = (orbitYawRot * orbitPitchRot).Angles();
					orbitAngles = orbitAngles.Normal;

					orbitYawRot = Rotation.From( 0.0f, orbitAngles.yaw, 0.0f );
					orbitPitchRot = Rotation.From( orbitAngles.pitch, 0.0f, 0.0f );
				}*/

				_orbitAngles.yaw += input.AnalogLook.yaw;
				_orbitAngles.pitch += input.AnalogLook.pitch;
				_orbitAngles = _orbitAngles.Normal;
				_orbitAngles.pitch = _orbitAngles.pitch.Clamp( this.MinOrbitPitch, this.MaxOrbitPitch );

				_sinceOrbit = 0;
				_orbiting = true;
			}

			if ( _thirdPerson )
			{
				input.ViewAngles = _orbitAngles;
			}
			else
			{
				
				input.ViewAngles = (car.Rotation * Rotation.From( _orbitAngles )).Angles();
			}

			input.ViewAngles = input.ViewAngles.Normal;
		}
	}
}
