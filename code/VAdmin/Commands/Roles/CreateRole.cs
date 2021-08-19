using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Commands
{
	public class CreateRole : VAdminCommand
	{
		public override string Name => "create_role";

		public override string Description => "Creates a role with the given name.";

		public override string Category => "Role Management";

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("role name", CommandArgType.String),
		};

		public override EchoType Echo => EchoType.ToStaff;

		public override bool Execute( Client caller, string[] args, out string error )
		{
			return VAdminSystem.CreateRole( args[0], out error );
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			return $"Created a new role '{args[0]}'";
		}
	}
}
