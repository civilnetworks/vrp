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
using VRP.Player;

namespace VAdmin.Commands
{
	public class GiveMoney : VAdminCommand
	{
		public override string Name => "givemoney";

		public override string Description => "Gives cash to the player you are looking at.";

		public override string Category => "VRP";

		public override EchoType Echo => EchoType.ToSender;

		public override CommandArg[] Args => new CommandArg[]
		{
			new CommandArg("amount", CommandArgType.Number)
			{
				Constraints = new CommandConstraint []
				{
					new RoundToConstraint(2),
				}
			}
		};

		public override bool Execute( Client caller, string[] args, out string error )
		{
			if ( !(caller.Pawn is VrpPlayer player) )
			{
				error = "Player has no pawn";
				return false;
			}

			var tr = player.GetEyeTrace( 200f );
			if ( !(tr.Entity is VrpPlayer other) )
			{
				error = "Must be looking at another player";
				return false;
			}

			var amt = double.Parse( args[0] );

			FinanceManager.GetBalance( caller, typeof( WalletMedium ), out var originalBalance, out var getBalanceError );

			if ( !FinanceManager.Withdraw( caller, typeof( WalletMedium ), amt, out error ) )
			{
				return false;
			}

			if ( !FinanceManager.Deposit( other.GetClientOwner(), typeof( WalletMedium ), amt, out error ) )
			{
				if ( !FinanceManager.SetBalance( caller, typeof( WalletMedium ), originalBalance, out var refundError ) )
				{
					// This should never happen
					VrpSystem.SendChatMessage( $"Error refunding failed givemoney - {refundError}" );
				}

				return false;
			}

			VrpSystem.SendChatMessage( To.Single( caller ), $"You gave {FinanceManager.FormatMoney( amt )} to {other.GetRPName()}" );
			VrpSystem.SendChatMessage( To.Single( other.GetClientOwner() ), $"{VrpSystem.GetRPName( caller )} gave you {FinanceManager.FormatMoney( amt )}" );

			return true;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			return null;
		}
	}
}
