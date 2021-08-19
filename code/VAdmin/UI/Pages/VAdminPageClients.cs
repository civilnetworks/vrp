using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.UI.Elements;

namespace VAdmin.UI
{
	public class VAdminPageClients : Panel
	{
		private Panel _content;

		public VAdminPageClients()
		{
			StyleSheet.Load( "/VAdmin/UI/Pages/VAdminPageClients.scss" );

			var banner = Add.Panel( "banner" );

			this.ListUsers( VAdminSystem.GetOnlineUsers( true ) );
		}

		public void ListUsers( VAdminUserInfo[] users )
		{
			_content?.Delete( true );
			_content = Add.Panel( "list-container" );

			var content = _content.Add.Panel( "content" );

			foreach ( var user in users )
			{
				var panel = content.AddChild<VAdminUserInfoPanel>();
				panel.SetUser( user );
			}
		}
	}
}
