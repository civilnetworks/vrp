using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public class VGuiButton : Button
	{
		public VGuiButton()
		{
			StyleSheet.Load( "/VGui/UI/VGuiButton.scss" );

			this.SetText( "VGui Button" );

			var line = Add.Panel( "line" );
		}
	}
}
