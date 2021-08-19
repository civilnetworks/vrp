using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Constraints;

namespace VAdmin.Commands
{
	public class Send : VAdminCommand
	{
		public override string Name => "send";

		public override string Description => "Sends target A to target B.";

		public override string Category => "Teleport";

		public override EchoType Echo => EchoType.ToStaff;

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("target", CommandArgType.Client),
			new CommandArg("send to", CommandArgType.Client) {
				Constraints = new CommandConstraint[] {
					new SingleClientConstraint()
				}
			}
		};

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );
			var target = VAdminSystem.FindClientTarget( args[1], caller );

			return VAdminSystem.Teleport( client, target, out error );
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );
			var target = VAdminSystem.FindClientTarget( args[1], caller );

			return $"Sent {VAdminSystem.KEYWORD_TARGET} to {target.Name}";
		}
	}
}
