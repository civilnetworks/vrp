using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVehicle.WheelTypes;

namespace VVehicle
{
	public partial class VVehicleWheel
	{
		public static bool DebugWheels;

		private VVehicle _vehicle;
		private VVehicleWheelEntity _modelEntity;
		private VVehicleWheelType _wheelType;
		private VVehicleWheelConfig _config;
		private Particles _driftSmoke;

		public VVehicleWheel( VVehicle vehicle, VVehicleWheelConfig config, VVehicleWheelType wheelType )
		{
			_wheelType = wheelType;
			_config = config;
			_modelEntity = new VVehicleWheelEntity();
			_vehicle = vehicle;

			// Never parent to attachment here, position is calculated dynamically later.
			_modelEntity.SetModel( _wheelType.Model );
			_modelEntity.EnableDrawing = false;
			_modelEntity.SetParent( vehicle, null, new Transform( config.Position, config.WheelRight ? Rotation.From( 0, 0, 0 ) : Rotation.From( 0, 180, 0 ) ) );
			_modelEntity.CollisionGroup = CollisionGroup.Debris;
			_modelEntity.Vehicle = vehicle;

			if ( Host.IsServer )
			{
				this.ApplyWheelValues();
			}
		}

		public VVehicleWheelConfig Config
			=> _config;

		public VVehicleWheelEntity ModelEntity
			=> _modelEntity;

		public float GetBreakFrictionReduction()
		{
			if ( _vehicle.Handbreak )
			{
				return _vehicle.Config.HandbreakFrictionReduction;
			}
			else if ( _vehicle.Break )
			{
				return 0.2f;
			}
			else
			{
				return 0f;
			}
		}

		[ClientCmd( "vvehicle_debug_wheels" )]
		public static void ToggleDebugWheels()
		{
			DebugWheels = !DebugWheels;
			Log.Info( $"DebugWheels: {DebugWheels}" );
		}
	}
}
