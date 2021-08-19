using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP.Items.Interacts
{
	public class ClaimUnclaimInteraction : VrpItemInteraction
	{
		public const string DEFAULT_DATA_KEY = "claim";

		public ClaimUnclaimInteraction() : base( "claim_unclaim", "Claim / Unclaim" )
		{

		}

		public static ulong GetOwnerSteamId( VrpItemEntity ent )
		{
			return ent.Item.GetUnsignedLong( DEFAULT_DATA_KEY );
		}

		public static Client GetOwner( VrpItemEntity ent )
		{
			var steamId = GetOwnerSteamId( ent );
			return Client.All.FirstOrDefault( c => c.SteamId == steamId );
		}

		public static bool IsClaimed( VrpItemEntity ent )
		{
			return GetOwnerSteamId( ent ) != 0;
		}

		public static bool IsClaimable( VrpItemEntity ent )
		{
			return ent.Item.HasInteraction( typeof( ClaimUnclaimInteraction ) );
		}

		public override void ClientInteract( Client caller, VrpItemEntity ent, string[] args )
		{

		}

		public override string GetDisplayName( Client caller, VrpItemEntity ent )
		{
			var owner = GetOwnerSteamId( ent );
			if ( owner == caller.SteamId )
			{
				return "Un-claim (Claimed)";
			}
			else
			{
				return "Claim";
			}
		}

		public override string GetIconPath( Client caller, VrpItemEntity ent )
		{
			var owner = GetOwnerSteamId( ent );
			if ( owner == caller.SteamId )
			{
				return "materials/vrp/interactions/unclaim.png";
			}
			else
			{
				return "materials/vrp/interactions/claim.png";
			}
		}

		public override VrpItemInteractResult Interact( Client caller, VrpItemEntity ent, string[] args )
		{
			var owner = GetOwnerSteamId( ent );
			var owned = owner != 0;

			if ( owner == caller.SteamId )
			{
				// Unclaim
				ent.Item.SetData( DEFAULT_DATA_KEY, null );
				VrpSystem.SendChatMessage( To.Single( caller ), $"You un-claimed the {ent.Item.GetDisplayName()}" );

				return new VrpItemInteractResult( true );
			}
			else
			{
				// Claim
				if ( owned )
				{
					return new VrpItemInteractResult( false, "You can't claim what you don't own." );
				}

				ent.Item.SetData( DEFAULT_DATA_KEY, caller.SteamId );
				VrpSystem.SendChatMessage( To.Single( caller ), $"You claimed the {ent.Item.GetDisplayName()}" );

				return new VrpItemInteractResult( true );
			}
		}
	}
}
