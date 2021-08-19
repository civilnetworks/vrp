using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Items.Interacts
{
	public class OpenCloseInteraction : ToggleInteraction
	{
		private const string DEFAULT_DATA_KEY = "open";

		public OpenCloseInteraction( string openDisplayName = "Open", string closeDisplayName = "Close" ) : base( "open_close", "Open / Close", closeDisplayName, openDisplayName )
		{
			this.DataToggleKey = DEFAULT_DATA_KEY;
			this.OnIconPath = "materials/vrp/interactions/open_case.png";
			this.OffIconPath = "materials/vrp/interactions/close_case.png";
		}

		public override bool CanToggle( Client caller, VrpItemEntity ent, bool newValue, out string reason )
		{
			if ( LockUnlockInteraction.IsLocked( ent ) )
			{
				reason = "Locked";
				return false;
			}

			reason = null;
			return base.CanToggle( caller, ent, newValue, out reason );
		}
	}
}
