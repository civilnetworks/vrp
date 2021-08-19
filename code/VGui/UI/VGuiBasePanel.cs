using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace VRP.VGui.UI
{
	[Library]
	public class VGuiBasePanel : Panel
	{
		public VGuiBasePanel()
		{
			this.OnLoad();
		}

		public void SetShadow( bool shadow )
		{
			this.SetClass( "v-shadow", shadow );
		}

		public void SetVisible(bool visible)
		{
			this.Style.Opacity = visible ? 1f : 0f;
		}

		protected virtual void OnLoad()
		{
			StyleSheet.Load( "/vgui/ui/VGuiBasePanel.scss" );
		}
	}
}
