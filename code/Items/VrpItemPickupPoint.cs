using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VRP.Items
{
	public class VrpItemPickupPoint
	{
		public VrpItemPickupPoint() { }

		public VrpItemPickupPoint( VrpItemEntity ent )
		{
			this.Entity = ent;
		}

		public VrpItemPickupPoint( VrpItemEntity ent, bool rightHand, Location location )
		{
			this.Entity = ent;
			this.RightHand = rightHand;
			this.Location = location ?? this.Location;
		}

		[JsonIgnore]
		public VrpItemEntity Entity { get; set; }

		public Location Location { get; set; } = new Location();

		public bool RightHand { get; set; } = true;

		public Vector3 HoldPosition { get; set; } = Vector3.Zero;

		[JsonIgnore]
		public Vector3 PrevPos { get; set; } = Vector3.Zero;
	}
}
