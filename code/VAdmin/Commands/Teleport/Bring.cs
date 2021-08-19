using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Constraints;

namespace VAdmin.Commands
{
	public class Bring : VAdminCommand
	{
		public override string Name => "bring";

		public override string Description => "Teleports the target to the you.";

		public override string Category => "Teleport";

		public override EchoType Echo => EchoType.ToStaff;

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("target", CommandArgType.Client),
		};

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );

			return VAdminSystem.Teleport( client, caller, out error );
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );

			return $"Brought {VAdminSystem.KEYWORD_TARGET}";
		}
	}
}
