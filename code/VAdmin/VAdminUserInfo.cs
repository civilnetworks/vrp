using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin.Permissions;

namespace VAdmin
{
	public class VAdminUserInfo
	{
		private List<string> _roles = new List<string>();

		public VAdminUserInfo( ulong steamid )
		{
			this.SteamId = steamid;
		}

		public ulong SteamId { get; }

		/// <summary>
		/// Returns the highest level granted by any roles this user has.
		/// </summary>
		/// <returns></returns>
		public int GetHighestLevel()
		{
			var level = 0;
			foreach ( var roleId in this.GetRoles() )
			{
				if ( !VAdminSystem.TryGetRole( roleId, out var role ) )
				{
					continue;
				}

				level = Math.Max( level, role.Level );
			}
			return level;
		}

		public bool IsStaff()
		{
			foreach ( var roleId in GetRoles() )
			{
				if ( !VAdminSystem.TryGetRole( roleId, out var role ) )
				{
					continue;
				}

				if ( role.Staff )
				{
					return true;
				}
			}

			return false;
		}

		public bool IsOnline()
		{
			return this.GetClient() != null;
		}

		public Client GetClient()
		{
			return Client.All.FirstOrDefault( c => c.SteamId == this.SteamId );
		}

		public string[] GetRoles( bool includeDefault = true )
		{
			if ( !includeDefault )
			{
				return _roles.ToArray();
			}

			return _roles.Append( VAdminSystem.DEFAULT_USER_ROLE.Id ).ToArray();
		}

		public bool HasRole( string id )
		{
			return GetRoles().FirstOrDefault( r => r.Equals( id ) ) != null;
		}

		public bool AddRole( string id )
		{
			if ( _roles.Contains( id ) )
			{
				return false;
			}

			_roles.Add( id );
			return true;
		}

		public bool RemoveRole( string id )
		{
			return _roles.Remove( id );
		}

		public bool CanTarget( string permissionName, int requiredLevel, out VAdminPermissionSummary summary )
		{
			summary = this.GetPermissionSummary( permissionName );

			if ( !summary.Granted )
			{
				return false;
			}

			if ( summary.OverrideLevel )
			{
				// Overridden permission levels always behave like TargetDefault.Relative
				return summary.Level >= requiredLevel;
			}
			else if ( VAdminSystem.TryGetPermission( permissionName, out var permission ) )
			{
				// Non-overridden permission level (Set from role level)
				switch ( permission.TargetDefault )
				{
					case TargetDefault.All:
						return true;
					case TargetDefault.Relative:
						return summary.Level >= requiredLevel;
					case TargetDefault.RelativeBelow:
						return summary.Level > requiredLevel;
					default:
						return false;
				}
			}
			else
			{
				return false;
			}
		}

		public VAdminPermissionSummary GetPermissionSummary( string permissionName )
		{
			var granted = false;
			var grantedBy = new List<string>();
			var overrideLevel = false;
			var overrideLevelBy = new List<string>();
			var maxLevel = -int.MaxValue;
			var maxLevelBy = String.Empty;

			var hasPriorityRole = false;
			var grantedPriority = false;
			var grantedByPriority = new List<string>();
			var overrideLevelPriority = false;
			var overrideLevelByPriority = new List<string>();
			var maxLevelPriority = -int.MaxValue;
			var maxLevelByPriority = String.Empty;

			foreach ( var roleId in this.GetRoles() )
			{
				if ( !VAdminSystem.TryGetRole( roleId, out var role ) )
				{
					continue;
				}

				var permission = role.GetPermission( permissionName );
				if ( permission == null )
				{
					continue;
				}

				if ( permission.Priority )
				{
					hasPriorityRole = true;

					if ( permission.Granted )
					{
						grantedPriority = true;
						grantedByPriority.Add( roleId );
					}
					else if ( !grantedPriority )
					{
						grantedByPriority.Add( roleId );
					}
					if ( permission.OverrideLevel )
					{
						overrideLevelPriority = true;
						overrideLevelByPriority.Add( roleId );

						if ( maxLevelPriority < permission.Level )
						{
							maxLevelPriority = permission.Level;
							maxLevelByPriority = roleId;
						}
					}
					else if ( maxLevel < role.Level )
					{
						maxLevelPriority = role.Level;
						maxLevelByPriority = roleId;
					}
				}
				else
				{
					if ( permission.Granted )
					{
						granted = true;
						grantedBy.Add( roleId );
					}
					if ( permission.OverrideLevel )
					{
						overrideLevel = true;
						overrideLevelBy.Add( roleId );

						if ( maxLevel < permission.Level )
						{
							maxLevel = permission.Level;
							maxLevelBy = roleId;
						}
					}
					else if ( maxLevel < role.Level )
					{
						maxLevel = role.Level;
						maxLevelBy = roleId;
					}
				}
			}

			VAdminPermissionSummary summary;

			if ( hasPriorityRole )
			{
				summary = new VAdminPermissionSummary( permissionName )
				{
					Granted = grantedPriority,
					GrantedBy = grantedByPriority.ToArray(),
					OverrideLevel = overrideLevelPriority,
					OverrideLevelBy = overrideLevelByPriority.ToArray(),
					Level = maxLevelPriority,
					LevelBy = maxLevelByPriority,
				};
			}
			else
			{
				summary = new VAdminPermissionSummary( permissionName )
				{
					Granted = granted,
					GrantedBy = grantedBy.ToArray(),
					OverrideLevel = overrideLevel,
					OverrideLevelBy = overrideLevelBy.ToArray(),
					Level = maxLevel,
					LevelBy = maxLevelBy,
				};
			}

			return summary;
		}

		public static VAdminUserInfo FromData( VAdminUserInfoData data )
		{
			return new VAdminUserInfo( data.SteamId )
			{
				_roles = data.Roles.ToList(),
			};
		}
	}
}
