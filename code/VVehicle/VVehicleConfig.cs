using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVehicle.WheelTypes;

namespace VVehicle
{
	public abstract class VVehicleConfig
	{
		protected VVehicleWheelType _defaultWheelType;

		public VVehicleConfig()
		{
			_defaultWheelType = this.GetDefaultWheelType();
		}

		public string VehicleTitle { get; set; }

		public abstract string Model { get; }

		public VVehicleWheelType DefaultWheelType
			=> _defaultWheelType;

		public abstract VVehicleWheelConfig[] Wheels { get; }

		public virtual float Power { get; } = 300f;

		public virtual float PowerSustain { get; } = 4f;

		public virtual float TurnSpeed { get; } = 100f;

		/// <summary>
		/// Multiplier for turn speed when returning to 0 turn angle (no steering input).
		/// </summary>
		public virtual float TurnZeroMultiplier { get; } = 3.5f;

		public float TurnAngle { get; private set; } = 35f;

		public virtual float SuspensionHeight { get; } = 6f;

		public virtual float PeakSuspensionForce { get; } = 4000f;

		public virtual float SuspensionExponent { get; } = 2f;

		public virtual float SuspensionDampening { get; } = 2f;

		public virtual float WheelPeakFriction { get; } = 140f;

		public virtual float WheelFriction { get; } = 2f;

		public virtual float BreakPeakFriction { get; } = 300f;

		public virtual float BreakFriction { get; } = 1f;

		public virtual float WheelRayRadius { get; } = 5f;

		public virtual float Drag { get; } = 10f;

		/// <summary>
		/// Amount to reduce friction when handbreaking, for drifting, should be between 0-1.
		/// </summary>
		public virtual float HandbreakFrictionReduction { get; } = 0.7f;

		protected abstract VVehicleWheelType GetDefaultWheelType();
	}
}
