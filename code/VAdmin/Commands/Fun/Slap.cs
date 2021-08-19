using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Constraints;

namespace VAdmin.Commands
{
	public class Slap : VAdminCommand
	{
		public override string Name => "slap";

		public override string Description => "Slaps the target with the given damage.";

		public override string Category => "Fun";

		public override EchoType Echo => EchoType.ToStaff;

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("target", CommandArgType.Client),
			new CommandArg("damage", CommandArgType.WholeNumber)
		};

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );
			var damage = int.Parse( args[1] );

			if (client.Pawn == null )
			{
				error = "Client has no Pawn";
				return false;
			}

			client.Pawn.TakeDamage( new DamageInfo()
			{
				Damage = damage,
				Attacker = caller.Pawn,
				Force = Vector3.Up * 100000,
			} );

			error = null;
			return true;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			var client = VAdminSystem.GetCommandTarget( args, caller );
			var damage = int.Parse( args[1] );

			return $"Slapped {VAdminSystem.KEYWORD_TARGET} for {damage}";
		}
	}
}
