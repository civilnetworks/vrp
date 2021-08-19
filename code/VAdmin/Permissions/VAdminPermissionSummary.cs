using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Permissions
{
	public class VAdminPermissionSummary
	{
		public VAdminPermissionSummary( string permissionId )
		{
			this.Permission = permissionId;
		}

		public string Permission { get; }

		public bool Granted { get; set; }

		public string[] GrantedBy { get; set; }

		public bool OverrideLevel { get; set; }

		public string[] OverrideLevelBy { get; set; }

		public int Level { get; set; }

		public string LevelBy { get; set; }
	}
}
