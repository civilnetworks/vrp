using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVehicle.WheelTypes
{
	[Library( Title = "Mustang Wheel" )]
	public class MustangWheel : VVehicleWheelType
	{
		public override string Model => "models/civilgamers/vehicles/wheels/wheel_mustang.vmdl";

		public override float Radius => 15f;

		public override float Width => 10f;
	}
}
