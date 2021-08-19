using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin;

namespace VAdmin
{
	public class ArgInputStatus
	{
		public CommandArg Arg { get; set; }

		public bool InProgress { get; set; }

		public ArgValidation Validation { get; set; } = ArgValidation.Valid;
	}
}
