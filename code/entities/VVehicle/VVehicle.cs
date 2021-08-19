using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Player;

namespace VVehicle
{
	struct VVehicleInput
	{
		public float Throttle;
		public float Turn;
		public float Handbreak;
		public float Boost;
		public bool Hydraulic;

		public void Reset()
		{
			Throttle = 0;
			Turn = 0;
			Handbreak = 0;
			Boost = 0;
			Hydraulic = false;
		}
	}

	[Library( "ent_vvehicle", Title = "VVehicle", Spawnable = true )]
	public partial class VVehicle : Prop, IUse
	{
		private VVehicleConfig _config;
		private VVehicleWheel[] _wheels;
		private bool _spawned;
		private bool _shouldReload;

		private VVehicleInput _input;

		public VVehicle()
		{
			
		}

		public VVehicleConfig Config
			=> _config;

		public VVehicleWheel[] Wheels
			=> _wheels;

		public float Hydraulic { get; set; }

		[Net]
		public string VehicleTitle { get; set; }

		[Net]
		public bool Handbreak { get; set; } = true;

		[Net]
		public bool Break { get; set; } = true;

		[Net]
		public float Turn { get; set; }

		[Net]
		public float Throttle { get; set; }

		[Net]
		public Player Driver { get; private set; }

		[Net]
		internal List<VrpPlayer> Passengers { get; private set; }

		public void SpawnVehicle( VVehicleConfig config )
		{
			if ( _spawned )
			{
				throw new Exception( "Vehicle already spawned" );
			}

			this.VehicleTitle = config.VehicleTitle;

			_spawned = true;
			_config = config;

			this.SetModel( config.Model );
			this.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

			var wheels = new List<VVehicleWheel>();

			foreach ( var wheelConfig in config.Wheels )
			{
				var wheel = new VVehicleWheel( this, wheelConfig, _config.DefaultWheelType );
				wheels.Add( wheel );
			}

			_wheels = wheels.ToArray();

			var data = VVehicleSystem.GetVehicleData( this.VehicleTitle );

			this.Passengers = new List<VrpPlayer>();

			for ( var i = 0; i < data.Seats.Length; i++ )
			{
				this.Passengers.Add( null );
			}

			if ( IsServer )
			{
				_passengers = new VrpPlayer[data.Seats.Length];
				_passengerEnterDirections = new Vector3[data.Seats.Length];
			}
		}

		[Event.Frame]
		public void OnFrame()
		{
			if ( IsServer )
			{
				return;
			}

			if ( VVehicleSystem.DebugSeats )
			{
				var data = VVehicleSystem.GetVehicleData( this.VehicleTitle );
				var enterPos = this.GetEnterSeatPos( Local.Pawn as VrpPlayer );
				var enterIndex = this.GetEnterSeat( Local.Pawn as VrpPlayer );
				var index = 0;

				foreach ( var seat in data.Seats )
				{
					var seatPos = this.Transform.PointToWorld( seat.Location.GetPos() );
					DebugOverlay.Line( this.Position, seatPos, Color.White, 0, false );

					if ( enterIndex == index )
					{
						DebugOverlay.Line( enterPos, seatPos, Color.Green, 0, false );
					}

					index++;
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (IsServer)
			{
				this.ExitAllSeats();
			}
		}

		public bool AnyWheelsGrounded()
		{
			foreach ( var wheel in _wheels )
			{
				if ( wheel.ModelEntity.Grounded )
				{
					return true;
				}
			}

			return false;
		}

		private Vector3 GetEnterSeatPos( VrpPlayer player )
		{
			return player.Position + player.EyeRot.Forward * 40f;
		}

		private int GetEnterSeat( VrpPlayer player )
		{
			var data = VVehicleSystem.GetVehicleData( this.VehicleTitle );
			var testPos = this.GetEnterSeatPos( player );

			var closestDistance = float.MaxValue;
			var closestSeat = -1;

			for ( var i = 0; i < data.Seats.Length; i++ )
			{
				var seat = data.Seats[i];

				var pos = this.Transform.PointToWorld( seat.Location.GetPos() );
				var dist = Vector3.DistanceBetween( pos, testPos );

				if ( dist < closestDistance && dist < 40 )
				{
					closestDistance = dist;
					closestSeat = i;
				}
			}

			return closestSeat;
		}

		[Event.Hotload]
		private void Hotload()
		{
			_shouldReload = true;
		}
	}
}
