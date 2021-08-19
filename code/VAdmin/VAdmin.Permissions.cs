using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VAdmin.Permissions;
using VChat;

namespace VAdmin
{
	public delegate void VAdminRoleEvent( VAdminRole role );
	public delegate void VAdminRolesNetworkedEvent();

	public static partial class VAdminSystem
	{
		public static VAdminRoleEvent OnRoleLoaded;
		public static VAdminRolesNetworkedEvent OnNetworkRolesReceived;

		public const int MIN_ROLE_NAME_LENGTH = 2;
		public const int MAX_ROLE_NAME_LENGTH = 30;

		public static VAdminRole DEFAULT_USER_ROLE = new VAdminRole( "user", "User", false, Color.White, 0 )
		{
			Display = true,
		};
		public static VAdminRole DEFAULT_ADMIN_ROLE = new VAdminRole( "admin", "Admin", false, new Color( 0.1f, 0.3f, 0.8f ), 500 )
		{
			Display = true,
		};
		public static VAdminRole DEFAULT_OWNER_ROLE = new VAdminRole( "owner", "Owner", false, Color.Orange, 1000 )
		{
			Display = true,
			Root = true,
		};

		private static Dictionary<string, VAdminRole> ROLES = new Dictionary<string, VAdminRole>();

		private static Dictionary<string, VAdminPermission> PERMISSIONS = new Dictionary<string, VAdminPermission>();

		public static void LoadPermissions()
		{
			PERMISSIONS.Clear();

			LoadPermission( new VAdminPermission( "edit_role_settings", "Allows editing role name, color and settings.", TargetDefault.Relative )
			{
				Category = "Role Management",
				DisplayName = "Edit Role Settings",
			} );
			LoadPermission( new VAdminPermission( "edit_role_permissions", "Allows editing role level and permissions.", TargetDefault.Relative )
			{
				Category = "Role Management",
				DisplayName = "Edit Role Permissions",
			} );

			// VRP //

			LoadPermission( new VAdminPermission( "vrp_spawn_items", "Allows spawning of VRP items.", TargetDefault.All )
			{
				Category = "VRP",
				DisplayName = "Spawn VRP Items",
			} );
			LoadPermission( new VAdminPermission( "vrp_spawn_vehicles", "Allows spawning of vehicles.", TargetDefault.All )
			{
				Category = "VRP",
				DisplayName = "Spawn Vehicles",
			} );
			LoadPermission( new VAdminPermission( "vrp_noclip", "Allows access to noclip.", TargetDefault.All )
			{
				Category = "VRP",
				DisplayName = "Noclip",
			} );

			foreach ( var pair in COMMANDS )
			{
				var command = pair.Value;

				var permissionName = "cmd_" + command.Name;
				var perm = LoadPermission( new VAdminPermission( permissionName, $"Allows access to command '{command.Name}'", command.TargetDefault ) );
				perm.DisplayName = command.Name;
				perm.Category = command.Category;

				command.PermissionName = permissionName;
			}
		}

		public static VAdminPermission LoadPermission( VAdminPermission permission )
		{
			if ( PERMISSIONS.ContainsKey( permission.Name ) )
			{
				PERMISSIONS.Remove( permission.Name );
			}

			PERMISSIONS[permission.Name] = permission;
			return permission;
		}

		public static bool TestPermission( Client client, string permission, int requiredLevel = 0 )
		{
			var user = GetUserInfo( client );
			if ( !user.CanTarget( permission, requiredLevel, out var summary ) )
			{
				SendPermissionSummaryMessage( To.Single( client ), summary, requiredLevel );
				return false;
			}

			return true;
		}

		public static bool TestCommand( Client client, string command, int requiredLevel = 0 )
		{
			return TestPermission( client, "cmd_" + command, requiredLevel );
		}

		public static List<VAdminPermission> GetPermissions( string category = null )
		{
			if ( category == null )
			{
				return PERMISSIONS.Select( p => p.Value ).ToList();
			}
			else
			{
				return PERMISSIONS.Select( p => p.Value ).Where( p => p.GetCategory().Equals( category ) ).ToList();
			}
		}

		public static bool TryGetPermission( string permissionId, out VAdminPermission permission )
		{
			return PERMISSIONS.TryGetValue( permissionId, out permission );
		}

		public static string[] GetPermissionCategories()
		{
			return PERMISSIONS.Select( p => p.Value.GetCategory() ).Where( c => !c.Equals( "Other" ) ).Distinct().OrderBy( c => c ).Append( "Other" ).ToArray();
		}

		public static string SanitizeRoleName( string roleName )
		{
			if ( roleName == null )
			{
				return String.Empty;
			}

			return roleName.Trim();
		}

		public static bool ValidRoleName( string roleName, out string error )
		{
			if ( roleName.Length < MIN_ROLE_NAME_LENGTH )
			{
				error = $"Role name too short ( < {MIN_ROLE_NAME_LENGTH} )";
				return false;
			}

			if ( roleName.Length > MAX_ROLE_NAME_LENGTH )
			{
				error = $"Role name too long ( > {MAX_ROLE_NAME_LENGTH} )";
				return false;
			}

			if ( ROLES.FirstOrDefault( p => p.Value.Name.ToLower().Equals( roleName.ToLower() ) ).Value != null )
			{
				error = "Role name already taken";
				return false;
			}

			error = null;
			return true;
		}

		public static VAdminRole LoadRole( VAdminRole role )
		{
			// Clean role since it was just loaded.
			role.MakeDirty( false );

			if ( ROLES.ContainsKey( role.Id ) )
			{
				ROLES.Remove( role.Id );
			}

			ROLES.Add( role.Id, role );

			OnRoleLoaded?.Invoke( role );

			return role;
		}

		public static List<VAdminRole> GetRoles()
		{
			return ROLES.Select( pair => pair.Value ).OrderByDescending( r => r.Level ).ToList();
		}

		public static bool TryGetRole( string id, out VAdminRole role )
		{
			return ROLES.TryGetValue( id, out role );
		}

		public static bool FindRoleByName( string name, out VAdminRole role )
		{
			role = ROLES.FirstOrDefault( r => r.Value.Name.ToLower().Equals( name.ToLower() ) ).Value;
			return role != null;
		}

		public static Client[] FindStaff()
		{
			var staff = new List<Client>();

			foreach ( var cl in Client.All )
			{
				var user = GetUserInfo( cl );

				if ( user.IsStaff() )
				{
					staff.Add( cl );
				}
			}

			return staff.ToArray();
		}

		#region ServerCmd

		[ServerCmd]
		public static void CmdEditRolePermissionSettings( string id, bool roleEnabled, int level )
		{
			if ( !ROLES.TryGetValue( id, out var role ) )
			{
				return;
			}

			var client = ConsoleSystem.Caller;
			var user = GetUserInfo( client );

			if ( !user.CanTarget( "edit_role_permissions", role.Level, out var summary ) )
			{
				SendPermissionSummaryMessage( To.Single( client ), summary, role.Level );
				return;
			}

			var success = false;

			if ( role.Enabled != roleEnabled )
			{
				success = true;
				role.Enabled = roleEnabled;
			}

			if ( role.Level != level )
			{
				success = true;
				role.Level = level;
			}

			SaveRoles();
			NetworkRole( To.Everyone, role );

			if ( success )
			{
				VAdminSystem.SendChatMessage( $"{client.Name} modified '{role.Name}' permission settings" );
			}
		}

		[ServerCmd]
		public static void CmdEditRoleSettings( string id, string roleName, float r, float g, float b, bool roleDisplay, bool isStaff )
		{
			if ( !ROLES.TryGetValue( id, out var role ) )
			{
				return;
			}

			var client = ConsoleSystem.Caller;
			var user = GetUserInfo( client );

			if ( !user.CanTarget( "edit_role_settings", role.Level, out var summary ) )
			{
				SendPermissionSummaryMessage( To.Single( client ), summary, role.Level );
				return;
			}

			var success = false;

			if ( !role.Name.Equals( roleName ) )
			{
				if ( TrySetRoleName( id, roleName, out var nameErr ) )
				{
					success = true;
				}
				else if ( nameErr != null )
				{
					// TODO: Notify error
					VAdminSystem.SendChatMessage( To.Single( client ), nameErr );
				}
			}

			var color = new Color( r, g, b );

			if ( !role.Color.Equals( color ) )
			{
				role.Color = color;
				success = true;
			}

			if ( role.Display != roleDisplay )
			{
				role.Display = roleDisplay;
				success = true;
			}

			if ( role.Staff != isStaff )
			{
				role.Staff = isStaff;
				success = true;
			}

			SaveRoles();
			NetworkRole( To.Everyone, role );

			if ( success )
			{
				VAdminSystem.SendChatMessage( $"{client.Name} modified '{role.Name}' settings" );
			}
		}

		[ServerCmd]
		public static void CmdEditRolePermission( string id, string permission, bool granted, bool overrideLevel, int level, bool priority )
		{
			var client = ConsoleSystem.Caller;

			var perm = new VAdminRolePermission()
			{
				Permission = permission,
				Granted = granted,
				OverrideLevel = overrideLevel,
				Level = level,
				Priority = priority
			};

			// TODO: Check permission

			if ( !ROLES.TryGetValue( id, out var role ) )
			{
				return;
			}

			var originalPermission = role.GetPermission( permission );

			if ( !TrySetRolePermission( id, perm, out var error ) )
			{
				// TODO: Notify error
				VAdminSystem.SendChatMessage( To.Single( client ), error );

				// Re-network the role to correct client mistake in UI.
				NetworkRole( To.Single( client ), role );

				return;
			}

			SaveRoles();
			NetworkRole( To.Everyone, role );

			var newPermission = role.GetPermission( permission );

			var msg = $"{client.Name} modified '{role.Name}' permission '{newPermission.Permission}'";

			if ( originalPermission.Granted != newPermission.Granted )
			{
				msg += $" - Granted [{newPermission.Granted}]";
			}
			if ( originalPermission.OverrideLevel != newPermission.OverrideLevel )
			{
				msg += $" - Override Level [{newPermission.OverrideLevel}]";
			}
			if ( originalPermission.Level != newPermission.Level )
			{
				msg += $" - Level [{newPermission.Level}]";
			}
			if ( originalPermission.Priority != newPermission.Priority )
			{
				msg += $" - Priority [{newPermission.Priority}]";
			}

			VAdminSystem.SendChatMessage( msg );
		}

		#endregion

		#region ClientRpc

		[ClientRpc]
		public static void RpcNetworkRole( string roleJson )
		{
			Log.Info( "VAdmin receive RpcNetworkRole" );

			var role = JsonSerializer.Deserialize<VAdminRole>( roleJson, new JsonSerializerOptions()
			{
				IncludeFields = true,
			} );

			Log.Info( $"Received role '{role.Name}'" );

			LoadRole( role );
		}

		[ClientRpc]
		public static void RpcNetworkRoles( string rolesJson )
		{
			Log.Info( "VAdmin receive RpcNetworkRoles" );

			var roles = JsonSerializer.Deserialize<List<VAdminRole>>( rolesJson, new JsonSerializerOptions()
			{
				IncludeFields = true,
			} );

			Log.Info( $"Received {roles.Count} roles" );

			ROLES.Clear();

			foreach ( var role in roles )
			{
				LoadRole( role );
			}

			OnNetworkRolesReceived?.Invoke();
		}

		#endregion

		#region Commands

		[ServerCmd( "vadmin_roles_sv" )]
		public static void CmdInfoSv()
		{
			PrintRoles();
		}

		[ClientCmd( "vadmin_roles" )]
		public static void CmdInfo()
		{
			PrintRoles();
		}

		private static void PrintRoles()
		{
			Log.Info( "" );
			Log.Info( $"== Begin VAdmin Roles Info == {(Host.IsClient ? "CL" : "SV")}" );
			Log.Info( "" );

			var roles = GetRoles();

			foreach ( var role in roles )
			{
				Log.Info( $"	= Role [{role.Id}] =" );
				Log.Info( $"	Name: {role.Name}" );
				Log.Info( $"	Level: {role.Level}" );
				Log.Info( $"	Color: {role.Color}" );

				Log.Info( "" );
			}

			Log.Info( "== End VAdmin Roles Info ==" );
			Log.Info( "" );
		}

		#endregion
	}
}
