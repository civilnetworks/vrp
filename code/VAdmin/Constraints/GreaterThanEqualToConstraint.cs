using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Constraints
{
	public class GreaterThanEqualToConstraint : NumberConstraint
	{
		public GreaterThanEqualToConstraint( double constraint ) : base( constraint )
		{
		}

		public override ArgValidation ValidateNumber( double value, out string error )
		{
			error = $"Must be greater than {_constraint}";
			return value >= _constraint ? ArgValidation.Valid : ArgValidation.Error;
		}
	}
}
