using Sandbox.UI;

namespace VRP.VGui.UI
{
	class VGuiLabel : Label
	{
		public VGuiLabel()
		{
			this.SetClass( "v-text", true );
		}

		public void SetFontSize( Length? fontSize )
		{
			this.Style.FontSize = fontSize;
		}
	}
}
