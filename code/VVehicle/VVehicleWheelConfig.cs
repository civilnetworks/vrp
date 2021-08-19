using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVehicle
{
	public class VVehicleWheelConfig
	{
		public VVehicleWheelConfig( Vector3 pos, bool right = false )
		{
			this.Position = pos;
			this.WheelRight = right;
		}

		public VVehicleWheelConfig( string attachment, bool right = false )
		{
			this.UseAttachment = true;
			this.Attachment = attachment;
			this.WheelRight = right;
		}

		public bool UseAttachment { get; set; }

		public string Attachment { get; }

		public bool WheelRight { get; }

		public bool Handbreak { get; set; }

		public bool Steering { get; set; }

		public bool Drive { get; set; }

		public Vector3 Position { get; set; }

		public static VVehicleWheelConfig[] AttachmentWheels4x4()
		{
			// Need to use a method instead of a property for hotreloading.
			return new VVehicleWheelConfig[] {
				new VVehicleWheelConfig("Wheel_FR", true) {
					Steering = true,
					Drive = true,
				},
				new VVehicleWheelConfig("Wheel_FL") {
					Steering = true,
					Drive = true,
				},
				new VVehicleWheelConfig("Wheel_BR", true) {
					Handbreak = true,
				},
				new VVehicleWheelConfig("Wheel_BL"){
					Handbreak = true,
				},
			};
		}
	}
}
