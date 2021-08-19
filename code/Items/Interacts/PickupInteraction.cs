using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Items.Interacts
{
	public class PickupInteraction : VrpItemInteraction
	{
		public const string PICKUP_INTERACTION_KEY = "carry";
		public const string PICKUP_INTERACTION_NAME = "Pickup";

		public PickupInteraction() : base( PICKUP_INTERACTION_KEY, PICKUP_INTERACTION_NAME )
		{
		}

		public override string GetIconPath( Client caller, VrpItemEntity ent )
		{
			return "materials/vrp/interactions/pickup.png";
		}

		public override void ClientInteract( Client caller, VrpItemEntity ent, string[] args )
		{
			throw new NotImplementedException();
		}

		public override VrpItemInteractResult Interact( Client caller, VrpItemEntity ent, string[] args )
		{
			throw new NotImplementedException();
		}
	}
}
