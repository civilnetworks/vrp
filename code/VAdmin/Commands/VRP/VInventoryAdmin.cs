using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VInventory;

namespace VAdmin.Commands
{
	class VInventoryAdmin : VAdminCommand
	{
		public override string Name => "vinventory_admin";

		public override string Description => "Admin access to VInventory settings.";

		public override string Category => "VRP";

		public override EchoType Echo => EchoType.Silent;

		public override CommandArg[] Args => new CommandArg[] { };

		public override bool Execute( Client caller, string[] args, out string error )
		{
			VInventorySystem.RpcOpenAdminMenu( To.Single( caller ) );
			error = null;
			return true;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			return null;
		}
	}
}
