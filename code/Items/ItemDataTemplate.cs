using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrp.Items
{
	public class ItemDataTemplate
	{
		public ItemDataTemplate() { }

		public ItemDataTemplate( string key, object defaultValue, bool networked = false )
		{
			this.Key = key;
			this.DefaultValue = defaultValue;
			this.Networked = networked;
		}

		public string Key { get; set; }

		public object DefaultValue { get; set; }

		public bool Networked { get; set; }
	}
}
