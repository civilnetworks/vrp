using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin.Constraints
{
	public abstract class CommandConstraint {
		public abstract ArgValidation Validate( string value, out string error );
	}

	public abstract class CommandConstraint<T> : CommandConstraint
	{
		protected T _constraint;

		public CommandConstraint(T constraint)
		{
			_constraint = constraint;
		}
	}
}
