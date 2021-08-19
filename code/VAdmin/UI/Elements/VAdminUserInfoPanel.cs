using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRP.VGui;
using VRP.VGui.UI;

namespace VAdmin.UI.Elements
{
	public class VAdminUserInfoPanel : Panel
	{
		private VAdminUserInfo _user;
		private Label _name;
		private Panel _top;
		private Panel _bottom;
		private VAdminRolePanel _rolePanel;

		public VAdminUserInfoPanel()
		{
			StyleSheet.Load( "/VAdmin/UI/Elements/VAdminUserInfoPanel.scss" );

			VAdminSystem.OnUserLoaded += this.OnUserLoaded;
		}

		public override void Tick()
		{
			base.Tick();

			if ( _user == null )
			{
				return;
			}

			var client = Client.All.FirstOrDefault( c => c.SteamId == _user.SteamId );
			if ( client != null )
			{
				_name.SetText( client.Name );
			}
		}

		public void SetUser( VAdminUserInfo user )
		{
			_user = user;

			var displayRole = VAdminSystem.GetUserDisplayRole( user, true );
			var roles = VAdminSystem.GetUserRoles( user );

			_top?.Delete( true );
			_top = Add.Panel( "user-info-top" );

			_bottom?.Delete( true );
			_bottom = Add.Panel( "user-info-bottom" );

			_rolePanel?.Delete( true );
			_rolePanel = _top.AddChild<VAdminRolePanel>( "role" );

			if ( displayRole != null )
			{
				_rolePanel.SetRole( displayRole );
				_rolePanel.AddEventListener( "onclick", ( e ) =>
				 {
					 var rolesPage = VAdminMenu.Instance.Nav.OpenPage<VAdminPageRoles>();
					 rolesPage.SelectRole( displayRole );
					 e.StopPropagation();
				 } );
			}

			_name?.Delete( true );
			_name = _top.Add.Label( user.SteamId.ToString(), "name" );

			foreach ( var role in roles )
			{
				var rolePanel = _bottom.Add.Label( role.Name, "mini-role" );
				rolePanel.Style.BackgroundColor = VGuiHelpers.Saturate( role.Color, 0.5f, 0.7f, 0.5f );

				var delete = rolePanel.Add.Button( "X", "delete" );
				delete.AddEventListener( "onclick", ( e ) =>
				 {
					 VGuiConfirmBox.CreateConfirmBox( $"Revoke '{role.Name}' from {_name.Text}?", ( confirm ) =>
					  {
						  if ( confirm )
						  {
							  VAdminSystem.CmdRunCommand( "revokeid", new string[] { user.SteamId.ToString(), role.Name }.ToList() );
						  }
					  } );
					 e.StopPropagation();
				 } );
			}

			VAdminRoleAddWindow roleAddWindow = null;

			var addRole = _bottom.Add.Image( "materials/vrp/add.png", "add-role" );
			addRole.AddEventListener( "onclick", ( e ) =>
			 {
				 if ( roleAddWindow != null && !roleAddWindow.IsDeleting )
				 {
					 roleAddWindow.Delete();
					 return;
				 }

				 roleAddWindow = VAdminRoleAddWindow.Create( addRole, ( roleId ) =>
				  {
					  VAdminSystem.TryGetRole( roleId, out var role );
					  VAdminSystem.CmdRunCommand( "grant", new string[] { _name.Text, role.Name }.ToList() );
				  }, roles.Select( r => r.Id ).ToArray() );
				 e.StopPropagation();
			 } );
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			VAdminSystem.OnUserLoaded -= this.OnUserLoaded;
		}

		private void OnUserLoaded( VAdminUserInfo user )
		{
			if ( user.SteamId == _user?.SteamId )
			{
				this.SetUser( user );
			}
		}
	}
}
