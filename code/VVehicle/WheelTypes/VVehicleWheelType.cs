using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVehicle.WheelTypes
{
	public abstract class VVehicleWheelType
	{
		public abstract string Model { get; }

		public abstract float Radius { get; }

		public abstract float Width { get; }
	}
}
