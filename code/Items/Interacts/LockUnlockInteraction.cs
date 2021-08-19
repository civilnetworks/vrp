using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Items.Interacts;

namespace VRP.Items.Interacts
{
	public class LockUnlockInteraction : ToggleInteraction
	{
		public const string DEFAULT_DATA_KEY = "locked";

		public LockUnlockInteraction( string lockDisplayName = "Lock", string unlockDisplayName = "Unlock" ) : base( "lock_unlock", "Lock / Unlock", unlockDisplayName, lockDisplayName )
		{
			this.DataToggleKey = DEFAULT_DATA_KEY;
			this.OnIconPath = "materials/vrp/interactions/locked.png";
			this.OffIconPath = "materials/vrp/interactions/unlocked.png";
		}

		public override bool VisibleInInteractMenu => false;

		public override bool RequiresClaim => true;

		public static bool IsLockable( VrpItemEntity ent )
		{
			return ent.Item.HasInteraction( typeof( LockUnlockInteraction ) );
		}
		public static bool IsLocked( VrpItemEntity ent )
		{
			return bool.TryParse( ent.Item.GetData( DEFAULT_DATA_KEY, false ).ToString(), out var locked ) && locked;
		}

		public static void Lock( VrpItemEntity ent )
		{
			ent.Item.SetData( DEFAULT_DATA_KEY, true );
		}

		public static void Unlock( VrpItemEntity ent )
		{
			ent.Item.SetData( DEFAULT_DATA_KEY, false );
		}
	}
}
