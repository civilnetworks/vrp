using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.VGui.UI;

namespace VInventory.UI
{
	public class VInventoryAdminMenu : VGuiFrame
	{
		public static VInventoryAdminMenu Instance;

		private VGuiVerticalNavBar _nav;

		public VInventoryAdminMenu()
		{
			StyleSheet.Load( "/VInventory/UI/VInventoryAdminMenu.scss" );

			_nav = this.AddChild<VGuiVerticalNavBar>();

			_nav.AddNavButton( "Item Configs", "materials/vrp/settings.png", () =>
			 {
				 _nav.OpenPage<VInventoryItemConfigsPage>();
			 } );
		}

		public VGuiVerticalNavBar Nav
			=> _nav;

		public static VInventoryAdminMenu Create()
		{
			Host.AssertClient();

			Close();

			var frame = Local.Hud.AddChild<VInventoryAdminMenu>();
			frame.SetWidth( 900 );
			frame.SetHeight( 700 );
			frame.Header.SetTitle( "VInventory Admin Menu" );
			frame.Center();

			Instance = frame;

			return frame;
		}

		public static void Close()
		{
			Instance?.Delete();
		}
	}
}
