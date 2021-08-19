using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.VGui.UI
{
	public class VGuiTextEntry : TextEntry
	{
		public VGuiTextEntry() : base()
		{
			StyleSheet.Load( "/VGui/UI/VGuiTextEntry.scss" );
		}

		public bool PreventUpDown { get; set; }

		public void SetFontSize(int fontSize)
		{
			this.Style.FontSize = fontSize;
			this.Style.MinHeight = fontSize * (40 / 24);
			this.Style.Padding = (int)((float)fontSize * (10f / 24f));
		}

		public override void OnButtonTyped( string button, KeyModifiers km )
		{
			CreateEvent( "onbuttontyped", button );

			// Useful for preventing base TextEntry behaviour forcing caret position.
			if (this.PreventUpDown && (button == "up" || button == "down"))
			{
				return;
			}

			base.OnButtonTyped( button, km );
		}
	}
}
