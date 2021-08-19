using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Constraints;

namespace VAdmin
{
	public class CommandArg
	{
		public CommandArg( string name, CommandArgType type, bool optional = false )
		{
			this.Name = name;
			this.Type = type;
			this.Optional = optional;
		}

		public string Name { get; }

		public CommandArgType Type { get; }

		public bool Optional { get; }

		public CommandConstraint[] Constraints { get; set; } = new CommandConstraint[] { };

		public Func<string, string[]> Suggestions { get; set; }

		public ArgValidation Validate( string value, out string error )
		{
			foreach ( var constraint in this.Constraints )
			{
				var constraintResult = constraint.Validate( value, out error );
				if ( constraintResult != ArgValidation.Valid )
				{
					return constraintResult;
				}
			}

			error = null;
			return ArgValidation.Valid;
		}
	}
}
