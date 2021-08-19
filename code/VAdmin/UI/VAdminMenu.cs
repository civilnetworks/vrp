using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.VGui.UI;

namespace VAdmin.UI
{
	public partial class VAdminMenu : Panel
	{
		public static VAdminMenu Instance;

		private static VGuiFrame _adminFrame;

		private VGuiVerticalNavBar _nav;

		public VAdminMenu()
		{
			Instance = this;

			StyleSheet.Load( "/VAdmin/UI/VAdminMenu.scss" );

			_nav = this.AddChild<VGuiVerticalNavBar>();

			_nav.AddNavButton( "Commands", "materials/vrp/commands.png", () =>
			{
				_nav.OpenPage<VAdminPageCommands>();
			} );

			_nav.AddNavButton( "Clients", "materials/vrp/clients.png", () =>
			{
				_nav.OpenPage<VAdminPageClients>();
			} );

			_nav.AddNavButton( "Roles", "materials/vrp/admin.png", () =>
			{
				_nav.OpenPage<VAdminPageRoles>();
			} );
		}

		public VGuiVerticalNavBar Nav
			=> _nav;

		[ClientRpc]
		public static void Open()
		{
			Close();

			_adminFrame = Local.Hud.AddChild<VGuiFrame>();
			_adminFrame.Header.SetTitle( "VAdmin" );
			_adminFrame.SetWidth( 900 );
			_adminFrame.SetHeight( 700 );
			_adminFrame.Center();


			var menu = _adminFrame.AddChild<VAdminMenu>();
		}

		[ClientRpc]
		public static void Close()
		{
			_adminFrame?.Delete();
		}
	}
}
