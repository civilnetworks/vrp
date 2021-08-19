using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Constraints;
using VRP.Character;
using VRP.Finance;

namespace VAdmin.Commands
{
	public class SetMoney : VAdminCommand
	{
		public override string Name => "setmoney";

		public override string Description => "Sets the wallet money of the target character.";

		public override string Category => "Finance";

		public override CommandArg[] Args => new CommandArg[]
		{
			new CommandArg("target", CommandArgType.Client) {
				Constraints = new CommandConstraint[] {
					new SingleClientConstraint(),
				}
			},
			new CommandArg("amount", CommandArgType.Number) {
				Constraints = new CommandConstraint[]
				{
					new GreaterThanEqualToConstraint(0),
					new RoundToConstraint(2),
				},
			},
		};

		public override EchoType Echo => EchoType.ToAll;

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );
			var amt = double.Parse( args[1] );

			var character = CharacterManager.Instance.GetActiveCharacter( client );
			if (character == null)
			{
				error = "Client has no active character";
				return false;
			}

			return FinanceManager.SetBalance( client, "wallet", amt, out error );
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );
			var amt = double.Parse( args[1] );

			return $"Set the money of {VAdminSystem.KEYWORD_TARGET} to {amt}";
		}
	}
}
