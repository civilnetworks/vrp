using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin;
using VAdmin.Commands;
using VAdmin.Constraints;
using VRP.Character;
using VRP.Finance;

namespace VAdmin.Commands
{
	public class AddMoney : VAdminCommand
	{
		public AddMoney()
		{
		}

		public override string Name => "addmoney";

		public override string Description => "Adds the amount to the wallet of the target character.";

		public override string Category => "Finance";

		public override CommandArg[] Args => new CommandArg[]
		{
			new CommandArg("target", CommandArgType.Client){
				Constraints = new CommandConstraint[] {
					new SingleClientConstraint(),
				}
			},
			new CommandArg("amount", CommandArgType.Number) {
				Constraints = new CommandConstraint[]
				{
					new RoundToConstraint(2),
				}
			},
		};

		public override EchoType Echo => EchoType.ToAll;

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );
			var amt = double.Parse( args[1] );

			var character = CharacterManager.Instance.GetActiveCharacter( client );
			if ( character == null )
			{
				error = "Client has no active character";
				return false;
			}

			if ( amt == 0 )
			{
				error = "Amount is 0";
				return false;
			}

			if ( amt > 0 )
			{
				return FinanceManager.Deposit( client, "wallet", amt, out error );
			}
			else
			{
				return FinanceManager.Withdraw( client, "wallet", Math.Abs( amt ), out error );
			}
		}
		public override string FormatMessage( Client caller, string[] args )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );
			var amt = double.Parse( args[1] );

			return $"Set added {amt} to {VAdminSystem.KEYWORD_TARGET}";
		}
	}
}
