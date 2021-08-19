using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VAdmin.Permissions;

namespace VAdmin
{
	public delegate void VAdminUserInfoEvent( VAdminUserInfo user );

	public static partial class VAdminSystem
	{
		public static VAdminUserInfoEvent OnUserLoaded;

		private static Dictionary<ulong, VAdminUserInfo> USERS = new Dictionary<ulong, VAdminUserInfo>();

		public static VAdminUserInfo GetUserInfo( Client client )
		{
			return GetUserInfo( client.SteamId, false );
		}

		public static VAdminUserInfo GetUserInfo( ulong steamid, bool defaultToNull = false )
		{
			if ( USERS.TryGetValue( steamid, out var info ) )
			{
				return info;
			}

			if ( defaultToNull )
			{
				return null;
			}

			return new VAdminUserInfo( steamid );
		}

		public static VAdminUserInfo[] GetOnlineUsers( bool sort = false )
		{
			var users = new List<VAdminUserInfo>();

			foreach ( var cl in Client.All )
			{
				users.Add( GetUserInfo( cl ) );
			}

			if ( !sort )
			{
				return users.ToArray();
			}

			return users.OrderByDescending( u => u.GetHighestLevel() ).ThenBy( u => u.GetClient()?.Name ?? u.SteamId.ToString() ).ToArray();
		}

		public static VAdminRole[] GetUserRoles( VAdminUserInfo user, bool includeDefault = true )
		{
			var userRoles = user.GetRoles();

			if ( includeDefault )
			{
				return GetRoles().Where( r => userRoles.Contains( r.Id ) ).OrderByDescending( r => r.Level ).ToArray();
			}
			else
			{
				return GetRoles().Where( r => userRoles.Contains( r.Id ) && !r.Equals( DEFAULT_USER_ROLE.Id ) ).OrderByDescending( r => r.Level ).ToArray();
			}
		}

		public static VAdminRole GetUserDisplayRole( VAdminUserInfo user, bool includeDefault = false )
		{
			var roles = GetUserRoles( user, includeDefault );

			if ( roles.Length == 0 )
			{
				return null;
			}

			return roles[0];
		}

		public static VAdminUserInfo LoadUser( VAdminUserInfo user )
		{
			if ( USERS.ContainsKey( user.SteamId ) )
			{
				USERS.Remove( user.SteamId );
			}

			USERS[user.SteamId] = user;

			OnUserLoaded?.Invoke( user );

			return user;
		}

		#region Networking

		[ClientRpc]
		public static void RpcNetworkUserInfo( string infoJson )
		{
			var data = JsonSerializer.Deserialize<VAdminUserInfoData>( infoJson );
			var user = VAdminUserInfo.FromData( data );

			LoadUser( user );
		}

		#endregion
	}
}
