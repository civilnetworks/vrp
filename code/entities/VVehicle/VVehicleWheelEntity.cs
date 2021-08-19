using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVehicle
{
	public partial class VVehicleWheelEntity : ModelEntity
	{
		private static readonly string[] DIRT_SURFACES = new string[] { "dirt", "grass", "mud" };

		private Vector3 _prevWheelPos = Vector3.Zero;
		private Particles _driftSmoke;
		private Particles _driftSmokeBits;

		private ModelEntity _wheel;
		private float _rotation;
		private float _rotationSpeed;
		private Vector3 _prevPos;
		private string _prevSurfaceName;
		private float _curMovementDiff;

		public VVehicleWheelEntity()
		{

		}

		[Net]
		public VVehicle Vehicle { get; set; }

		[Net]
		public string Attachment { get; set; }

		[Net]
		public float Radius { get; set; }

		[Net]
		public float Width { get; set; }

		[Net]
		public bool UseAttachment { get; set; }

		[Net]
		public Vector3 WheelPos { get; set; }

		[Net]
		public float SuspensionHeight { get; set; }

		[Net]
		public float DriftAmt { get; set; }

		[Net]
		public bool Handbreak { get; set; }

		[Net]
		public float WheelRayRadius { get; set; }

		public bool Grounded { get; set; }

		public Vector3 GetWheelPos()
		{
			if ( this.UseAttachment )
			{
				var attachment = this.Vehicle.GetAttachment( this.Attachment );
				if ( attachment != null )
				{
					var attachTransform = (Transform)attachment;
					return attachTransform.Position + this.Vehicle.Transform.Rotation.Up * this.WheelRayRadius;
				}

				return this.Position;
			}
			else
			{
				return this.Vehicle.Transform.PointToWorld( this.WheelPos ) + this.Vehicle.Transform.Rotation.Up * this.WheelRayRadius;
			}
		}

		public float GetSuspensionHeight()
		{
			if ( this.Handbreak )
			{
				return this.SuspensionHeight * (1f + this.Vehicle.Hydraulic * 4f);
			}
			else
			{
				return this.SuspensionHeight;
			}
		}

		public TraceResult GetGroundTrace( out float trDistance )
		{
			var numFwdTraces = 5;
			var numRightTraces = 1;
			var pos = this.GetWheelPos();

			var up = this.Rotation.Up;
			var fwd = this.Rotation.Forward;
			var right = this.Rotation.Right;
			var fwdGap = this.Radius / numFwdTraces * 2f;
			var rightGap = this.Width / numRightTraces;
			var minTraceFrac = float.MaxValue;
			var minTraceDist = 0f;
			var minTrace = new TraceResult();

			var curY = numRightTraces > 1 ? -((numRightTraces - 1) / 2f) * rightGap : 0;

			for ( var y = 0; y < numRightTraces; y++ )
			{
				var curX = -((numFwdTraces - 1) / 2f) * fwdGap;

				for ( var x = 0; x < numFwdTraces; x++ )
				{
					var p = (float)x / (float)(numFwdTraces - 1) * (float)Math.PI;
					var z = 1f - MathF.Sin( p );
					var tracePos = pos + (fwd * curX) + (right * curY) + (up * z * this.Radius * 0.5f);
					var tr = this.GetGroundTraceInternal( tracePos, out var dist );

					if ( tr.Fraction < minTraceFrac )
					{
						minTraceFrac = tr.Fraction;
						minTraceDist = dist;
						minTrace = tr;
					}

					curX += fwdGap;
				}

				curY += rightGap;
			}

			this.Grounded = minTrace.Hit;

			trDistance = minTraceDist;
			return minTrace;
		}

		[Event.Frame]
		public void OnFrame()
		{
			if ( !(this.Vehicle?.IsValid() ?? false) )
			{
				return;
			}

			var dt = Time.Delta;

			if ( _wheel == null )
			{
				_wheel = new ModelEntity();
				_wheel.SetModel( this.GetModel() );
				_wheel.SetParent( this.Vehicle );
			}

			_wheel.Position = this.Position;
			var rot = this.LocalRotation.Angles();
			rot.pitch = _rotation;
			_wheel.LocalRotation = rot.ToRotation();

			var throttle = this.Vehicle.Throttle;
			var pos = _wheel.Position;

			var diff = pos - _prevPos;
			var diffLen = diff.Length;
			var fwdMove = Vector3.Dot( this.Rotation.Forward, diff.Normal ) * diffLen;
			_curMovementDiff += (diffLen - _curMovementDiff) * dt * 4f;

			if ( this.Vehicle.Handbreak && this.Handbreak )
			{
				if ( _rotationSpeed > 0f )
				{
					_rotationSpeed = Math.Max( 0f, _rotationSpeed - _rotationSpeed * dt * 8f );
				}
				else
				{
					_rotationSpeed = Math.Min( 0f, _rotationSpeed - _rotationSpeed * dt * 8f );
				}
			}
			else if ( this.Grounded )
			{
				_rotationSpeed = ((float)(180 / Math.PI) * (fwdMove / this.Radius) * 0.75f) / dt;
			}
			else
			{
				_rotationSpeed += (-throttle * 4000f - _rotationSpeed) * dt;
			}

			_rotation += _rotationSpeed * dt;
			_prevPos = pos;

			var tr = this.GetGroundTrace( out var trDistance );
			var driftAmt = this.DriftAmt;
			var surfaceName = tr.Surface.Name.ToLower();
			var isDirt = DIRT_SURFACES.Contains( surfaceName );

			if ( isDirt )
			{
				driftAmt = Math.Max( driftAmt, Math.Min( _curMovementDiff * 0.1f, 0.33f ) );
			}

			if ( tr.Hit && _prevSurfaceName == surfaceName && (isDirt && driftAmt > 0.03f || driftAmt > 0.2f) )
			{
				// TODO: This is shit
				if ( isDirt && _driftSmokeBits == null )
				{
					_driftSmokeBits = Particles.Create( "particles/civilgamers/drift_smoke_dirt_bits.vpcf", tr.EndPos );
				}

				if ( isDirt && driftAmt > 0.2f && _driftSmoke == null )
				{
					_driftSmoke = Particles.Create( "particles/civilgamers/drift_smoke_dirt.vpcf", tr.EndPos );
				}

				if ( !isDirt && _driftSmoke == null )
				{
					_driftSmoke = Particles.Create( "particles/civilgamers/drift_smoke.vpcf", tr.EndPos );
				}

				_driftSmoke?.SetPosition( 0, tr.EndPos );
				_driftSmoke?.SetPosition( 1, new Vector3( driftAmt, 0, 0 ) );
				_driftSmoke?.SetPosition( 2, (-diff.Normal + this.Rotation.Up) * driftAmt * 1f );

				_driftSmokeBits?.SetPosition( 0, tr.EndPos );
				_driftSmokeBits?.SetPosition( 1, new Vector3( driftAmt, 0, 0 ) );
				_driftSmokeBits?.SetPosition( 2, (-diff.Normal + this.Rotation.Up) * driftAmt * 1f );
			}
			else
			{
				_driftSmoke?.Destroy();
				_driftSmoke = null;

				_driftSmokeBits?.Destroy();
				_driftSmokeBits = null;
			}

			_prevSurfaceName = surfaceName;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_wheel?.Delete();
			_wheel = null;

			_driftSmoke?.Destroy();
			_driftSmokeBits?.Destroy();
		}

		private TraceResult GetGroundTraceInternal( Vector3 rayOrigin, out float trDistance )
		{
			var suspensionHeight = this.GetSuspensionHeight();
			var up = this.Vehicle.Transform.Rotation.Up;
			var rayDistance = suspensionHeight + this.Radius;

			rayOrigin -= this.Transform.Rotation.Right * this.WheelRayRadius * 0.5f;

			var rayReduction = this.Radius * 0.5f;
			rayOrigin -= up * rayReduction;
			rayDistance -= rayReduction;

			var rayEnd = rayOrigin - up * rayDistance;

			var trace = Trace.Ray( rayOrigin, rayEnd )
				.WorldAndEntities()
				.UseHitboxes()
				.HitLayer( CollisionLayer.All ^ CollisionLayer.Debris ^ CollisionLayer.Player )
				.Ignore( this.Vehicle )
				.Ignore( this )
				.Radius( this.WheelRayRadius );
			var tr = trace.Run();

			trDistance = tr.Distance + rayReduction;

			if ( VVehicleWheel.DebugWheels )
			{
				//DebugOverlay.Line( rayOrigin, rayEnd, Color.White, 0.01f, false );
				DebugOverlay.Line( rayOrigin, tr.EndPos, Color.Green, 0.01f, false );
			}

			return tr;
		}
	}
}
