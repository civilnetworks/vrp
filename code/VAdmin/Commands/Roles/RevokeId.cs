using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Commands
{
	public class RevokeId : VAdminCommand
	{
		public override string Name => "revokeid";

		public override string Description => "Revokes a role from the target SteamId.";

		public override string Category => "Role Management";

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("steamid64", CommandArgType.WholeNumber),
			new CommandArg("role", CommandArgType.Role)
		};

		public override EchoType Echo => EchoType.ToAll;

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var steamid = ulong.Parse( args[0] );
			VAdminSystem.FindRoleByName( args[1], out var role );

			if ( role == null )
			{
				error = $"Role '{args[1]}' not found.";
				return false;
			}

			return VAdminSystem.RevokeRole( steamid, role.Id, out error, true );
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			var steamid = ulong.Parse( args[0] );
			var target = Client.All.FirstOrDefault( c => c.SteamId == steamid );
			VAdminSystem.FindRoleByName( args[1], out var role );

			return $"Revoked '{role.Name}' from {target?.Name ?? steamid.ToString()}";
		}
	}
}
