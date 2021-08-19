using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Constraints;

namespace VAdmin.Commands
{
	public class Grant : VAdminCommand
	{
		public override string Name => "grant";

		public override string Description => "Grants a role to the target.";

		public override string Category => "Role Management";

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("target", CommandArgType.Client)
			{
				Constraints = new CommandConstraint[]
				{
					new SingleClientConstraint()
				},
			},
			new CommandArg("role", CommandArgType.Role),
		};

		public override EchoType Echo => EchoType.ToAll;

		public override bool Execute( Client caller, string[] args, out string error )
		{
			var target = VAdminSystem.FindClientTarget( args[0], caller );
			VAdminSystem.FindRoleByName( args[1], out var role );

			if (role == null)
			{
				error = $"Role '{args[1]}' not found.";
				return false;
			}

			return VAdminSystem.GrantRole( target, role.Id, out error );
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			var target = VAdminSystem.FindClientTarget( args[0], caller );
			VAdminSystem.FindRoleByName( args[1], out var role );

			return $"Granted '{role.Name}' to {target.Name}";
		}
	}
}
