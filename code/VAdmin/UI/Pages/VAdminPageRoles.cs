using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Permissions;
using VAdmin.UI.Elements;
using VRP.VGui.UI;

namespace VAdmin.UI
{
	public class VAdminPageRoles : Panel
	{
		private List<VAdminRolePanel> _rolePanels = new List<VAdminRolePanel>();
		private int _selectedRoleIndex;
		private Panel _roleDetailsPanel;

		private VAdminRole _selectedRole;

		private Panel _roleScroll;
		private int _numRoles;

		private VGuiTextEntry _nameEntry;
		private VGuiTextEntry _levelEntry;
		private RoleColorPicker _colorEntry;
		private VGuiSwitchLabel _displayCheck;
		private VGuiSwitchLabel _staffCheck;
		private VGuiSwitchLabel _enabledCheck;
		private List<VAdminPermissionPanel> _permissionPanel = new List<VAdminPermissionPanel>();
		private Panel _roleColorOption;
		private Panel _saveSettings;
		private Button _saveSettingsButton;
		private Button _revertSettingsButton;
		private Button _createRole;
		private Button _deleteRole;

		public VAdminPageRoles()
		{
			StyleSheet.Load( "/VAdmin/UI/Pages/VAdminPageRoles.scss" );

			_roleScroll?.Delete( true );
			_roleScroll = Add.Panel( "role-scroll" );

			this.CreateScrollUI();
			this.CreateRoleUI();
			this.SelectRole( 0 );

			VAdminSystem.OnRoleLoaded += this.OnRoleLoaded;
			VAdminSystem.OnNetworkRolesReceived += this.OnNetworkRolesReceived;
		}

		public override void Tick()
		{
			base.Tick();

			if ( _selectedRole != null )
			{
				var different = !_nameEntry.Text.Equals( _selectedRole.Name )
					|| !_levelEntry.Text.Equals( _selectedRole.Level.ToString() )
					|| _displayCheck.CheckBox.Checked != _selectedRole.Display
					|| _staffCheck.CheckBox.Checked != _selectedRole.Staff
					|| _enabledCheck.CheckBox.Checked != _selectedRole.Enabled
					|| !_colorEntry.Color.Equals( _selectedRole.Color );

				_saveSettings.SetClass( "active", different );
			}
		}

		public void SelectRole( VAdminRole role )
		{
			var curIndex = 0;
			foreach ( var panel in _rolePanels )
			{
				if ( panel.Role.Id == role.Id )
				{
					panel.SetRole( role );
					this.SelectRole( curIndex );
					return;
				}

				curIndex++;
			}
		}

		public void SelectRole( int index )
		{
			_selectedRoleIndex = index;

			var curIndex = 0;
			foreach ( var panel in _rolePanels )
			{
				panel.SetClass( "selected", curIndex == _selectedRoleIndex );
				curIndex++;
			}

			var rolePanel = _rolePanels[index];
			_selectedRole = rolePanel.Role;

			_nameEntry.SetText( rolePanel.Role.Name );

			_levelEntry.SetText( rolePanel.Role.Level.ToString() );

			_roleColorOption?.Delete( true );
			_roleColorOption = _colorEntry.AddColor( rolePanel.Role.Color );
			_colorEntry.SetColor( rolePanel.Role.Color );

			_displayCheck.CheckBox.Checked = rolePanel.Role.Display;
			_staffCheck.CheckBox.Checked = rolePanel.Role.Staff;
			_enabledCheck.CheckBox.Checked = rolePanel.Role.Enabled;

			foreach ( var panel in _permissionPanel )
			{
				panel.SetRole( rolePanel.Role );
			}
		}

		private void CreateScrollUI()
		{
			var roles = VAdminSystem.GetRoles();
			var index = 0;

			foreach (var panel in _rolePanels )
			{
				panel.Delete( true );
			}

			_rolePanels.Clear();

			foreach ( var role in roles )
			{
				var curIndex = index;

				var panel = _roleScroll.AddChild<VAdminRolePanel>();
				panel.SetRole( role );
				panel.AddEventListener( "onclick", ( e ) =>
				{
					this.SelectRole( curIndex );
					e.StopPropagation();
				} );

				_rolePanels.Add( panel );
				index++;
			}

			_createRole = _roleScroll.Add.Button( "Create Role", "create" );
			_createRole.AddEventListener( "onclick", e =>
			{
				VAdminSystem.CmdRunCommand( "create_role", new string[] { "New Role" }.ToList() );
				e.StopPropagation();
			} );

			_numRoles = roles.Count();
		}

		private void CreateRoleUI()
		{
			_roleDetailsPanel?.Delete( true );
			_roleDetailsPanel = Add.Panel( "role-details" );

			_deleteRole = _roleDetailsPanel.Add.Button( "Delete Role", "delete-role" );
			_deleteRole.AddEventListener( "onclick", ( e ) =>
			 {
				 VGuiConfirmBox.CreateConfirmBox( $"Delete {_selectedRole.Name}?", ( confirm ) =>
				  {
					  if ( confirm )
					  {
						  VAdminSystem.CmdRunCommand( "delete_role", new string[] { _selectedRole.Name }.ToList() );
					  }
				  } );

				 e.StopPropagation();
			 } );

			_saveSettings = _roleDetailsPanel.Add.Panel( "save-settings" );
			_revertSettingsButton = _saveSettings.Add.Button( "Revert", "revert" );
			_revertSettingsButton.AddEventListener( "onclick", ( e ) =>
			 {
				 _nameEntry.SetText( _selectedRole.Name );
				 _levelEntry.SetText( _selectedRole.Level.ToString() );
				 _colorEntry.SetColor( _selectedRole.Color );
				 _displayCheck.CheckBox.Checked = _selectedRole.Display;
				 _staffCheck.CheckBox.Checked = _selectedRole.Staff;
				 _enabledCheck.CheckBox.Checked = _selectedRole.Enabled;

				 e.StopPropagation();
			 } );
			_saveSettingsButton = _saveSettings.Add.Button( "Save Settings", "save" );
			_saveSettingsButton.AddEventListener( "onclick", ( e ) =>
			 {
				 if ( _enabledCheck.CheckBox.Checked != _selectedRole.Enabled
					 || int.Parse( _levelEntry.Text ) != _selectedRole.Level )
				 {
					 VAdminSystem.CmdEditRolePermissionSettings( _selectedRole.Id,
						 _enabledCheck.CheckBox.Checked,
						 int.Parse( _levelEntry.Text ) );
				 }

				 if ( !_nameEntry.Text.Equals( _selectedRole.Name )
					|| !_colorEntry.Color.Equals( _selectedRole.Color )
					|| _displayCheck.CheckBox.Checked != _selectedRole.Display
					|| _staffCheck.CheckBox.Checked != _selectedRole.Staff )
				 {
					 VAdminSystem.CmdEditRoleSettings( _selectedRole.Id,
						 _nameEntry.Text,
						 _colorEntry.Color.r, _colorEntry.Color.g, _colorEntry.Color.b,
						 _displayCheck.CheckBox.Checked,
						 _staffCheck.CheckBox.Checked );
				 }
			 } );

			var fill = _roleDetailsPanel.Add.Panel( "role-details-scroll" );

			_nameEntry = _roleDetailsPanel.AddChild<VGuiTextEntry>();
			_nameEntry.SetFontSize( 18 );
			_roleDetailsPanel.Add.Label( "Role Name", "details-text" );

			var content = fill.Add.Panel( "role-details-container" );

			content.Add.Label( "Role Color", "details-text" );
			_colorEntry = content.AddChild<RoleColorPicker>();

			content.Add.Label( "Role Settings", "details-text" );
			_displayCheck = content.AddChild<VGuiSwitchLabel>();
			_displayCheck.Label.SetText( "Display Role" );

			_staffCheck = content.AddChild<VGuiSwitchLabel>();
			_staffCheck.Label.SetText( "Is Staff" );

			content.Add.Label( "Role Permission Level", "details-text" );
			_levelEntry = content.AddChild<VGuiTextEntry>();
			_levelEntry.Numeric = true;
			_levelEntry.SetFontSize( 12 );

			content.Add.Label( "Role Permissions", "details-text" );

			_enabledCheck = content.AddChild<VGuiSwitchLabel>();
			_enabledCheck.Label.SetText( "Enabled" );
			_enabledCheck.Style.MarginBottom = 22;

			var categories = VAdminSystem.GetPermissionCategories();
			foreach ( var category in categories )
			{
				var categoryTitle = content.Add.Label( category, "permission-category" );

				foreach ( var permission in VAdminSystem.GetPermissions( category ) )
				{
					var panel = content.AddChild<VAdminPermissionPanel>();
					panel.SetPermission( permission );

					_permissionPanel.Add( panel );
				}
			}
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			VAdminSystem.OnRoleLoaded -= this.OnRoleLoaded;
			VAdminSystem.OnNetworkRolesReceived -= this.OnNetworkRolesReceived;
		}

		private void OnRoleLoaded( VAdminRole role )
		{
			if ( role.Id == _selectedRole?.Id )
			{
				this.SelectRole( role );
			}

			if ( VAdminSystem.GetRoles().Count() != _numRoles )
			{
				this.CreateScrollUI();
			}
		}

		private void OnNetworkRolesReceived()
		{
			if ( VAdminSystem.GetRoles().Count() != _numRoles )
			{
				this.CreateScrollUI();
			}

			if (!VAdminSystem.FindRoleByName(_selectedRole.Name, out var role))
			{
				this.SelectRole( 0 );
			}
		}
	}
}
