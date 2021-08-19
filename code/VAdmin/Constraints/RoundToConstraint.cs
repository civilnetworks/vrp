using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Constraints
{
	public class RoundToConstraint : NumberConstraint
	{
		public RoundToConstraint( double constraint ) : base( constraint )
		{
		}

		public override ArgValidation ValidateNumber( double value, out string error )
		{
			error = $"Must round to {_constraint} decimal places";
			return Math.Round(value, (int)_constraint) == value ? ArgValidation.Valid : ArgValidation.Error;
		}
	}
}
