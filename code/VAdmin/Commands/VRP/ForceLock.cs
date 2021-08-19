using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP;
using VRP.Items;
using VRP.Items.Interacts;

namespace VAdmin.Commands
{
	public class ForceLock : VAdminCommand
	{
		public override string Name => "forcelock";

		public override string Description => "Locks or Unlocks an item.";

		public override string Category => "VRP";

		public override EchoType Echo => EchoType.ToStaff;

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("lock/unlock", CommandArgType.Bool, true)
		};

		public override bool Execute( Client caller, string[] args, out string error )
		{
			bool.TryParse( args[0] ?? "true", out var doLock );

			var tr = VrpSystem.GetEyeTrace( caller );

			if ( !tr.Hit )
			{
				error = "Trace hit nothing";
				return false;
			}

			var hit = tr.Entity;
			if ( hit == null )
			{
				error = "Trace hit null entity";
				return false;
			}

			if ( !(hit is VrpItemEntity) )
			{
				error = "Must look at a VrpItemEntity";
				return false;
			}

			var itemEnt = (VrpItemEntity)hit;

			if ( !itemEnt.Item.HasInteraction( typeof( LockUnlockInteraction ) ) )
			{
				error = "Item is not lockable";
				return false;
			}

			if ( doLock )
			{
				LockUnlockInteraction.Lock( itemEnt );
			}
			else
			{
				LockUnlockInteraction.Unlock( itemEnt );
			}

			error = null;
			return true;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			bool.TryParse( args[0] ?? "true", out var doLock );
			var tr = VrpSystem.GetEyeTrace( caller );
			var itemEnt = (VrpItemEntity)tr.Entity;

			if (doLock)
			{
				return $"Force locked '{itemEnt.Item.GetDisplayName()}'";
			}
			else
			{
				return $"Force unlocked '{itemEnt.Item.GetDisplayName()}'";
			}
		}
	}
}
