using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VChat;

namespace VAdmin.Commands
{
	public class DeleteRole : VAdminCommand
	{
		public override string Name => "delete_role";

		public override string Description => "Deletes the role with the given name.";

		public override string Category => "Role Management";

		public override CommandArg[] Args => new CommandArg[] {
			new CommandArg("role name", CommandArgType.Role),
		};

		public override EchoType Echo => EchoType.ToStaff;

		public override bool Execute( Client caller, string[] args, out string error )
		{
			if ( VAdminSystem.DeleteRole( args[0], out error, out var numRemovedUsers ) )
			{
				VChatSystem.AddInformation( To.Multiple( VAdminSystem.FindStaff() ), $"{numRemovedUsers} users removed from '{args[0]}'" );
				return true;
			}
			else
			{
				return false;
			}
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			return $"Deleted the role '{args[0]}'";
		}
	}
}
