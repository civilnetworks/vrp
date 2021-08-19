using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Items
{
	public class VrpItemActivePickup
	{
		public VrpItemActivePickup( Client client, VrpItemEntity itemEnt, VrpItemPickupPoint[] points )
		{
			this.Client = client;
			this.Entity = itemEnt;
			this.Points = points;
		}

		public Client Client { get; private set; }

		public VrpItemEntity Entity { get; set; }

		public VrpItemPickupPoint[] Points { get; private set; }
	}
}
