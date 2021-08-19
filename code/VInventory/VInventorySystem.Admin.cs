using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin;
using VInventory.UI;
using VRP;

namespace VInventory
{
	public static partial class VInventorySystem
	{
		public static bool IsVInventoryAdmin( Client client )
		{
			return VAdminSystem.TestCommand( client, "vinventory_admin" );
		}

		[ServerCmd]
		public static void CmdAddItemConfig( string configTitle, string json )
		{
			var client = ConsoleSystem.Caller;

			if ( !IsVInventoryAdmin( client ) )
			{
				return;
			}

			if ( !TryDeserializeItemConfig( configTitle, json, out var config, out var error ) )
			{
				VrpSystem.SendChatMessage( To.Single( client ), $"Error: {error}" );
				return;
			}

			config.Id = GenerateItemConfigId();

			if ( !config.CanCreateConfig( out error ) )
			{
				VrpSystem.SendChatMessage( To.Single( client ), $"Error: {error}" );
				return;
			}

			if ( !TryAddItemConfig( config, out error ) )
			{
				VrpSystem.SendChatMessage( To.Single( client ), $"Error: {error}" );
				return;
			}

			VrpSystem.SendChatMessage( To.Single( client ), $"Item config created [{config.Id}]" );
		}

		[ClientRpc]
		public static void RpcOpenAdminMenu()
		{
			VInventoryAdminMenu.Create();
		}
	}
}
