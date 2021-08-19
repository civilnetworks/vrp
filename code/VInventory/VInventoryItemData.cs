using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VInventory
{
	public class VInventoryItemData
	{
		[JsonIgnore]
		public bool Networked { get; set; }

		public object Value { get; set; }
	}
}
