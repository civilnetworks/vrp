using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Constraints;

namespace VAdmin.Commands
{
	public class Goto : VAdminCommand
	{
		public override string Name => "goto";

		public override string Description => "Teleports to the target.";

		public override string Category => "Teleport";

		public override EchoType Echo => EchoType.ToStaff;

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("target", CommandArgType.Client) {
				Constraints = new CommandConstraint[] {
					new SingleClientConstraint()
				}
			}
		};

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );

			return VAdminSystem.Teleport( caller, client, out error );
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );

			return $"Teleported to {VAdminSystem.KEYWORD_TARGET}";
		}
	}
}
