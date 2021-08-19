using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Constraints;

namespace VAdmin.Commands
{
	public class Teleport : VAdminCommand
	{
		public override string Name => "teleport";

		public override string Description => "Teleports the target to where you are looking.";

		public override string Category => "Teleport";

		public override EchoType Echo => EchoType.ToStaff;

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("target", CommandArgType.Client),
		};

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );

			var trace = Trace.Ray( caller.Pawn.EyePos, caller.Pawn.EyePos + caller.Pawn.EyeRot.Forward * 5000f )
				.HitLayer( CollisionLayer.Player, false ).Ignore( caller.Pawn );

			var tr = trace.Run();

			return VAdminSystem.Teleport( client, tr.EndPos, out error );
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );

			return $"Teleported {VAdminSystem.KEYWORD_TARGET}";
		}
	}
}
