using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVehicle.WheelTypes;

namespace VVehicle.VehicleTypes
{
	[Library( Title = "Mustang" )]
	public class Mustang : VVehicleConfig
	{
		public override string Model => "models/civilgamers/vehicles/mustang/mustang_body.vmdl";

		public override VVehicleWheelConfig[] Wheels => VVehicleWheelConfig.AttachmentWheels4x4();

		protected override VVehicleWheelType GetDefaultWheelType()
		{
			return new MustangWheel();
		}

		public override float SuspensionHeight { get; } = 6f;

		public override float PeakSuspensionForce { get; } = 4000f;

		public override float SuspensionExponent { get; } = 2f;

		public override float SuspensionDampening { get; } = 2f;

		public override float WheelPeakFriction { get; } = 140f;

		public override float WheelFriction { get; } = 2f;

		public override float BreakPeakFriction { get; } = 300f;

		public override float BreakFriction { get; } = 1f;

		public override float Drag { get; } = 10f;
	}
}
