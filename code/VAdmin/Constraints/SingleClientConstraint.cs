using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Constraints
{
	public class SingleClientConstraint : CommandConstraint
	{
		public override ArgValidation Validate( string value, out string error )
		{
			if (VAdminSystem.FindClients(value).Length > 1)
			{
				error = "Cannot target multiple clients";
				return ArgValidation.Warning;
			}

			error = null;
			return ArgValidation.Valid;
		}
	}
}
