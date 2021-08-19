using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Constraints;

namespace VAdmin.Commands
{
	public class Ban : VAdminCommand
	{
		public override string Name => "ban";

		public override string Description => "Bans the target for the given timeframe.";

		public override string Category => "Administration";

		public override EchoType Echo => EchoType.ToAll;

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("target", CommandArgType.Client) {
				Constraints = new CommandConstraint[] {
					new SingleClientConstraint(),
				},
			},
			new CommandArg("time", CommandArgType.Timeframe),
			new CommandArg("reason", CommandArgType.String, true)
		};

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );
			var timeframe = args[1];
			var reason = args[2];

			if ( !VAdminSystem.ParseTimeframe( timeframe, out var seconds ) )
			{
				error = "Invalid timeframe";
				return false;
			}



			error = null;
			return true;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			VAdminSystem.ParseTimeframe(args[1], out var seconds);

			var format = VAdminSystem.FormatTimeframe( seconds );
			var reason = args[2];

			if ( reason  == null)
			{
				return $"Banned {VAdminSystem.KEYWORD_TARGET} for {format}";
			}
			else
			{
				return $"Banned {VAdminSystem.KEYWORD_TARGET} for {format} ( {reason} )";
			}
			
		}
	}
}
