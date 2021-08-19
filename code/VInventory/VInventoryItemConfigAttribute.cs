using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VInventory
{
	public class VInventoryItemConfigAttribute
	{
		public VInventoryItemConfigAttribute( Func<object> getter, Action<object> setter )
		{
			this.Get = getter;
			this.Set = setter;
		}

		public Func<object> Get { get; }

		public Action<object> Set { get; }

	}
}
