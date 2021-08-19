using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Items
{
	public class VrpItemInteractArg
	{
		public VrpItemInteractArg( string name, VrpItemInteractArgType type, object defaultValue = null )
		{
			this.Name = name;
			this.Type = type;
			this.Default = defaultValue;
		}

		public string Name { get; }

		public VrpItemInteractArgType Type { get; }

		public object Default { get; }
	}
}
