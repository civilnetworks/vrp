using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public delegate void OnPanelClosed();
	public interface IClosablePanel
	{
		public event OnPanelClosed OnClosed;
	}
}
