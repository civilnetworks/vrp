using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Permissions
{
	public class VAdminPermission
	{
		public VAdminPermission(string name, string description, TargetDefault targetDefault)
		{
			this.Name = name;
			this.Description = description;
			this.TargetDefault = targetDefault;
		}

		public string Name { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public string Category { get; set; }

		public TargetDefault TargetDefault { get; set; }

		public string GetDisplayName()
		{
			if (this.DisplayName != null && this.DisplayName.Length > 0)
			{
				return this.DisplayName;
			}

			return this.Name;
		}

		public string GetCategory()
		{
			if ( this.Category?.Length > 0 )
			{
				return this.Category;
			}

			return "Other";
		}
	}
}
