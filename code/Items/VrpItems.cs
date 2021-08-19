using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdmin;
using VRP.Player;

namespace VRP.Items
{
	/// <summary>
	/// Static manager class for VRP Item entities and items.
	/// </summary>
	public static partial class VrpItems
	{
		/*public static VrpItemEntity SpawnItem()
		{

		}*/

		public static object ParseItemDataString(string val)
		{
			if ( bool.TryParse( val, out var bval ) )
			{
				return bval;
			}
			else if ( double.TryParse( val, out var dval ) )
			{
				return dval;
			}
			else
			{
				return val;
			}
		}

		[ServerCmd]
		public static void CmdCreateItem( string itemId )
		{
			var client = ConsoleSystem.Caller;
			if ( !VAdminSystem.TestPermission( client, "vrp_spawn_items" ) )
			{
				return;
			}

			var pos = client.Pawn.EyePos + client.Pawn.EyeRot.Forward * 100f;
			if ( !TryCreateItemEntity( itemId, pos, out var itemEntity, out var error ) )
			{
				VrpSystem.SendChatMessage( error );
			}
		}

		// TODO: Total hack since it doesn't work with string lists currently.
		public static void CmdCreateItem( string itemId, List<string> data )
		{
			var str = data[0];

			for ( var i = 1; i < data.Count; i++ )
			{
				str += $",{data[i]}";
			}

			CmdCreateItem( itemId, str );
		}

		[ServerCmd]
		public static void CmdCreateItem( string itemId, string str )
		{
			var isSplit = str.Contains( "," );
			var data = isSplit ? str.Split( "," ) : new string[] { str };

			var client = ConsoleSystem.Caller;
			if ( !VAdminSystem.TestPermission( client, "vrp_spawn_items" ) )
			{
				return;
			}

			var parsedData = new object[data.Length];

			for ( var i = 0; i < data.Length; i++ )
			{
				var val = data[i];
				parsedData[i] = ParseItemDataString( val );
			}

			if ( !TryCreateItem( itemId, out var item, out var error ) )
			{
				VrpSystem.SendChatMessage( error );
				return;
			}

			// Set the required data we received
			var x = 0;
			foreach ( var val in item.DataTemplates )
			{
				item.Data[val.Key] = parsedData[x++];
			}

			if ( client.Pawn is VrpPlayer player )
			{
				var pos = player.GetEyeTrace( 130f ).EndPos;
				if ( !TryCreateItemEntity( item, pos, out var itemEntity, out error ) )
				{
					VrpSystem.SendChatMessage( error );
				}
			}
		}
	}
}
