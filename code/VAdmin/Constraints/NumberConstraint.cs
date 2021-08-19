using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Constraints
{
	public class NumberConstraint : CommandConstraint<double>
	{
		public NumberConstraint( double constraint ) : base( constraint )
		{
		}

		public override ArgValidation Validate( string value, out string error )
		{
			if ( !double.TryParse( value, out var parsed ) )
			{
				error = "Must be a number";
				return ArgValidation.Error;
			}

			return this.ValidateNumber( parsed, out error );
		}

		public virtual ArgValidation ValidateNumber( double value, out string error )
		{
			error = null;
			return ArgValidation.Valid;
		}
	}
}
