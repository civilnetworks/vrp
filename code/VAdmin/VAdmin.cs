using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin
{
	public static partial class VAdminSystem
	{
		static VAdminSystem()
		{
			LoadCommands();
			LoadPermissions();
			
			if (Host.IsServer)
			{
				AddRoles();
				LoadUsers();
			}
		}
	}
}
