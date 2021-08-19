using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Items
{
	public class VrpItemInteractResult
	{
		public VrpItemInteractResult( bool success, string displayError = null )
		{
			this.Success = success;
			this.DisplayError = displayError;
		}

		public bool Success { get; }

		public string DisplayError { get; }
	}
}
