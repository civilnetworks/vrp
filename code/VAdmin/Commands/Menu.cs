using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.UI;

namespace VAdmin.Commands
{
	public class Menu : VAdminCommand
	{
		public override string Name => "menu";

		public override string Description => "Opens the VAdmin menu.";

		public override CommandArg[] Args => new CommandArg[] { };

		public override string Category => null;

		public override EchoType Echo => EchoType.Silent;

		public override bool Execute( Client caller, string[] args, out string error )
		{
			VAdminMenu.Open(To.Single( caller ));

			error = null;
			return true;
		}

		public override string FormatMessage( Client caller, string[] args )
		{
			return null;
		}
	}
}
