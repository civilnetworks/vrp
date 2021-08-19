using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Permissions
{
	public class VAdminRolePermission
	{
		public string Permission { get; set; }

		public bool Granted { get; set; }

		public bool OverrideLevel { get; set; }

		public int Level { get; set; }

		public bool Priority { get; set; }
	}
}
