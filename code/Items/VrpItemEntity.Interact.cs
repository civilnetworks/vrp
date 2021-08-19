using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VRP.Items.Interacts;
using VRP.Items.UI;

namespace VRP.Items
{
	public partial class VrpItemEntity : ModelEntity, IUse
	{
		public const float INTERACT_DISTANCE = 100;

		public bool OnUse( Entity user )
		{
			if ( user is SandboxPlayer player )
			{
				//this.RpcInteract( To.Single( player ) );
				return true;
			}

			return false;
		}

		public bool IsUsable( Entity user )
		{
			return user is SandboxPlayer;
		}

		[ServerCmd]
		public static void CmdInteract( int entNetworkIdent, string key )
		{
			DoCmdInteract( entNetworkIdent, key, null );
		}

		[ServerCmd]
		public static void CmdInteractArgs( int entNetworkIdent, string key, string argsJson )
		{
			var args = JsonSerializer.Deserialize<List<string>>( argsJson );

			DoCmdInteract( entNetworkIdent, key, args?.ToArray() );
		}

		private static void DoCmdInteract( int entNetworkIdent, string key, string[] args )
		{
			var client = ConsoleSystem.Caller;

			var ent = Entity.FindByIndex( entNetworkIdent );
			if ( ent == null || !(ent is VrpItemEntity itemEntity) )
			{
				return;
			}

			var interaction = itemEntity.Item.GetInteraction( key );
			if ( interaction != null && interaction.CanInteractFunc != null )
			{
				var result = interaction.CanInteractFunc( client, itemEntity );
				if ( !result.Success )
				{
					VrpSystem.SendChatMessage( To.Single( client ), result.DisplayError );
					return;
				}
			}

			if ( interaction.Args != null && interaction.Args.Length > 0 && args == null )
			{
				// Client needs to enter the required args
				RpcInteractArgs( To.Single( client ), entNetworkIdent, key );
				return;
			}

			if ( itemEntity.TryInteract( client, key, args, out var error ) )
			{
				// TODO: Interaction success stuff, E.G client interact callback, sounds etc
			}
			else if ( error != null )
			{
				if ( itemEntity.Item.HasInteraction( key ) )
				{
					VrpSystem.SendChatMessage( To.Single( client ), $"Cannot {interaction.GetDisplayName( client, itemEntity )} - {error}" );
				}
				else
				{
					VrpSystem.SendChatMessage( To.Single( client ), error );
				}
			}
		}

		[ClientRpc]
		public static void RpcInteractArgs( int entNetworkIdent, string interactKey )
		{
			var ent = Entity.FindByIndex( entNetworkIdent );
			if (ent == null)
			{
				return;
			}

			if (ent is VrpItemEntity itemEnt)
			{
				VrpItemInteractArgsMenu.Open( itemEnt, interactKey );
			}
		}

		/*[ClientRpc]
		private void RpcInteract()
		{
			VrpItemInteractMenu.Open( this );
		}*/
	}
}
