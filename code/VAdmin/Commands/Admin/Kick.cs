using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Constraints;

namespace VAdmin.Commands
{
	public class Kick : VAdminCommand
	{
		public override string Name => "kick";

		public override string Description => "Kicks the client from the game.";

		public override string Category => "Administration";

		public override EchoType Echo => EchoType.ToAll;

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("target", CommandArgType.Client) {
				Constraints = new CommandConstraint[] {
					new SingleClientConstraint()
				},
			},
			new CommandArg("reason", CommandArgType.String, true)
		};

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var client = VAdminSystem.FindClientTarget( args[0], caller );
			var reason = args[1] ?? String.Empty;

			// TODO: kick them properly with a reason
			ConsoleSystem.Run( "kick", new object[] { client.Name } );

			error = null;
			return true;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			var client = VAdminSystem.FindClientTarget( args[0], caller );
			var reason = args[1];

			if ( reason  != null)
			{
				return $"Kicked {client.Name} from the game ( {reason} )";
			}
			else
			{
				return $"Kicked {client.Name} from the game";
			}
		}
	}
}
