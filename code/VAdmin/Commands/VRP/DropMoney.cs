using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Constraints;
using VRP;
using VRP.Finance;
using VRP.Finance.Mediums;

namespace VAdmin.Commands
{
	public class DropMoney : VAdminCommand
	{
		public override string Name => "dropmoney";

		public override string Description => "Drops cash on the floor.";

		public override string Category => "VRP";

		public override EchoType Echo => EchoType.ToSender;

		public override CommandArg[] Args => new CommandArg[]
		{
			new CommandArg("amount", CommandArgType.WholeNumber)
		};

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var amt = double.Parse( args[0] );

			if ( FinanceManager.TryDropWalletCash( caller, amt, out var ent, out error ) )
			{
				VrpSystem.SendChatMessage( To.Single( caller ), $"You Dropped {FinanceManager.FormatMoney( amt )} cash" );
				return true;
			}

			return false;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			return null;
		}
	}
}
