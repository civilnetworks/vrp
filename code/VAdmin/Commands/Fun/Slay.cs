using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Commands
{
	public class Slay : VAdminCommand
	{
		public override string Name => "slay";

		public override string Description => "Slays the target.";

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("target", CommandArgType.Client),
		};

		public override string Category => "Fun";

		public override EchoType Echo => EchoType.ToStaff;

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );

			client.Pawn.Health = 0;
			client.Pawn.OnKilled();

			error = null;
			return true;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );

			return $"Slayed {VAdminSystem.KEYWORD_TARGET}";
		}
	}
}
