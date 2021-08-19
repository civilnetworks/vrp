using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Commands;

namespace VAdmin
{
	public class CommandEvaluation
	{
		public VAdminCommand[] Commands { get; set; }

		public ArgInputStatus[] ArgStatusList { get; set; }

		public string[] Args { get; set; }

		public ArgValidation Validation { get; set; }

		public bool IsValid
			=> this.Validation == ArgValidation.Valid;

		public string CurrentError { get; set; }
	}
}
