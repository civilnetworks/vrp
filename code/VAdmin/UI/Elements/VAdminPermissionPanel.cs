using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Permissions;
using VRP.VGui.UI;

namespace VAdmin.UI.Elements
{
	public class VAdminPermissionPanel : Panel
	{
		private VAdminPermission _permission;
		private VAdminRole _role;
		private Panel _top;
		private Panel _bottom;
		private VGuiSwitch _enabledCheck;
		private VGuiSwitch _priorityCheck;

		public VAdminPermissionPanel()
		{
			StyleSheet.Load( "/VAdmin/UI/Elements/VAdminPermissionPanel.scss" );

			_top = Add.Panel( "top" );
			_bottom = Add.Panel( "bottom" );

			VAdminSystem.OnRoleLoaded += this.OnRoleLoaded;
		}

		public VAdminPermission Permission
			=> _permission;

		public VAdminRole Role
			=> _role;

		public void SetRole( VAdminRole role )
		{
			_role = role;

			var permissions = role.GetPermission( this.Permission.Name );

			_enabledCheck.Checked = permissions.Granted;
			_priorityCheck.Checked = permissions.Priority;
		}

		public void SetPermission( VAdminPermission permission )
		{
			_permission = permission;

			_enabledCheck = _top.AddChild<VGuiSwitch>();
			_enabledCheck.Tooltip = "Grant this permission.";
			_enabledCheck.OnValueChanged += ( panel, value ) =>
			{
				if ( value )
				{
					VAdminSystem.CmdRunCommand( "permission_grant", new string[] { this.Role.Name, permission.Name }.ToList() );
				}
				else
				{
					VAdminSystem.CmdRunCommand( "permission_revoke", new string[] { this.Role.Name, permission.Name }.ToList() );
				}
				
				//VAdminSystem.CmdEditRolePermission( this.Role.Id, permission.Name, value, rolePermission.OverrideLevel, rolePermission.Level, rolePermission.Priority );
			};

			var label = _top.Add.Label( permission.GetDisplayName(), "label" );
			var right = _top.Add.Panel( "right" );

			_priorityCheck = right.AddChild<VGuiSwitch>();
			_priorityCheck.SetOnColor( Color.Orange );
			_priorityCheck.Tooltip = "Permission override over other roles.";
			_priorityCheck.OnValueChanged += ( panel, value ) =>
			{
				VAdminSystem.CmdRunCommand( "permission_set_override", new string[] { this.Role.Name, permission.Name, value.ToString() }.ToList() );

				//VAdminSystem.CmdEditRolePermission( this.Role.Id, permission.Name, rolePermission.Granted, rolePermission.OverrideLevel, rolePermission.Level, value );
			};

			var desc = _permission.Description;
			if ( desc != null && desc.Length > 0 )
			{
				this.SetClass( "big", true );
				var descLabel = _bottom.Add.Label( desc, "desc" );
			}
		}

		public override void OnDeleted()
		{
			base.OnDeleted();

			VAdminSystem.OnRoleLoaded -= this.OnRoleLoaded;
		}

		private void OnRoleLoaded( VAdminRole role )
		{
			if ( role.Id == _role?.Id )
			{
				this.SetRole( role );
			}
		}
	}
}
