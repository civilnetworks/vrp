using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public class VGuiSwitchLabel : Panel
	{
		private Label _label;
		private VGuiSwitch _checkBox;
		private Panel _line;

		public VGuiSwitchLabel()
		{
			StyleSheet.Load( "/VGui/UI/VGuiSwitchLabel.scss" );

			_label = Add.Label( "Check Box", "label" );
			_line = Add.Panel( "line" );
			_checkBox = this.AddChild<VGuiSwitch>( "check" );
		}

		public VGuiSwitch CheckBox
			=> _checkBox;

		public Label Label
			=> _label;
	}
}
