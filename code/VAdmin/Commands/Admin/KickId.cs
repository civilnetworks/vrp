using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Commands
{
	public class KickId : VAdminCommand
	{
		public override string Name => "kickid";

		public override string Description => "Kicks the Client with the given SteamId64";

		public override string Category => "Administration";

		public override EchoType Echo => EchoType.ToAll;

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("steamid", CommandArgType.ClientId),
			new CommandArg("reason", CommandArgType.String, true)
		};

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var client = VAdminSystem.FindClientTargetById( args[0] );
			var reason = args[1] ?? String.Empty;

			// TODO: kick them properly with a reason
			ConsoleSystem.Run( "kickid", new object[] { client.NetworkIdent } );

			error = null;
			return true;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			var client = VAdminSystem.FindClientTargetById( args[0] );
			var reason = args[1];

			if ( reason != null )
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
