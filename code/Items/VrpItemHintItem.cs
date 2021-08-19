using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.Items;

namespace vrp.Items
{
	public class VrpItemHintItem
	{
		public string DisplayText { get; set; }

		public Func<VrpItem, string> DisplayTextFunc { get; set; }

		public string GetDisplayText( VrpItem item )
		{
			if (this.DisplayTextFunc == null)
			{
				return this.DisplayText;
			}

			return this.DisplayTextFunc( item );
		}
	}
}
