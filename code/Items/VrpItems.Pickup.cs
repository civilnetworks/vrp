using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VRP.Player;

namespace VRP.Items
{
	public static partial class VrpItems
	{
		public const float PICKUP_TRACE_RADIUS = 12f;
		public const float PICKUP_TRACE_GAP = 16f;
		public const float PICKUP_TRACE_DISTANCE = 80f;

		private static Dictionary<Client, (int, VrpItemActivePickup)> sClientActivePickups = new Dictionary<Client, (int, VrpItemActivePickup)>();

		public static VrpItemEntity FindPickupItem( Client client, Vector3? lookNormal = null )
		{
			if ( !(client.Pawn is VrpPlayer player) )
			{
				return null;
			}

			var origin = player.EyePos;
			var fwd = lookNormal ?? player.EyeRot.Forward;
			var trace = Trace.Ray( origin, origin + fwd * (PICKUP_TRACE_DISTANCE + PICKUP_TRACE_RADIUS) )
				.WorldAndEntities()
				.UseHitboxes()
				.Ignore( player );
			var tr = trace.Run();

			if ( tr.Entity is VrpItemEntity itemEnt )
			{
				return itemEnt;
			}

			return null;
		}

		public static VrpItemPickupPoint[] FindPickupPoints( Client client, VrpItemEntity itemEnt, Vector3? lookNormal = null )
		{
			if ( itemEnt == null )
			{
				return null;
			}

			if ( !(client.Pawn is VrpPlayer player) )
			{
				return null;
			}

			var contacts = new List<VrpItemPickupPoint>();

			var origin = player.EyePos;
			var right = player.EyeRot.Right;
			if ( DoPickupTrace( client, itemEnt, origin + right * PICKUP_TRACE_GAP * 0.5f, lookNormal, out var rightHand ) )
			{
				rightHand.RightHand = true;
				contacts.Add( rightHand );
			}
			if ( DoPickupTrace( client, itemEnt, origin - right * PICKUP_TRACE_GAP * 0.5f, lookNormal, out var leftHand ) )
			{
				leftHand.RightHand = false;
				contacts.Add( leftHand );
			}

			if ( contacts.Count == 0 && DoPickupTrace( client, itemEnt, origin, lookNormal, out rightHand ) )
			{
				// Probably small object
				rightHand.RightHand = true;
				contacts.Add( rightHand );
			}

			return contacts.ToArray();
		}

		private static bool DoPickupTrace( Client client, VrpItemEntity itemEnt, Vector3 origin, Vector3? lookNormal, out VrpItemPickupPoint pickupPoint )
		{
			pickupPoint = null;

			if ( itemEnt == null )
			{
				return false;
			}

			if ( !(client.Pawn is VrpPlayer player) )
			{
				return false;
			}

			var tag = "vrp_pickup_test";
			itemEnt.Tags.Add( tag );

			var fwd = lookNormal ?? player.EyeRot.Forward;
			var trace = Trace.Ray( origin, origin + fwd * PICKUP_TRACE_DISTANCE )
				.EntitiesOnly()
				.UseHitboxes()
				.Ignore( player );
			//.WithTag( tag );
			var tr = trace.Run();

			itemEnt.Tags.Remove( tag );

			if ( !tr.Hit || tr.Entity != itemEnt )
			{
				return false;
			}

			if ( tr.Distance * tr.Fraction > PICKUP_TRACE_DISTANCE )
			{
				return false;
			}

			var localPos = itemEnt.Transform.PointToLocal( tr.EndPos );
			var localAng = itemEnt.Transform.NormalToLocal( tr.Normal ).EulerAngles.ToRotation();

			var pyr = localAng.Angles();
			pyr.pitch = pyr.pitch == pyr.pitch ? pyr.pitch : 0f;
			pyr.yaw = pyr.yaw == pyr.yaw ? pyr.yaw : 0f;
			pyr.roll = pyr.roll == pyr.roll ? pyr.roll : 0f;

			pickupPoint = new VrpItemPickupPoint( itemEnt )
			{
				Location = new Location( localPos, pyr ),
			};

			return true;
		}

		[ServerCmd]
		public static void CmdStartPickup( int networkIdent, string pickupJson )
		{
			var client = ConsoleSystem.Caller;

			var ent = Entity.FindByIndex( networkIdent );
			if ( !(ent is VrpItemEntity itemEnt) )
			{
				return;
			}

			var pickups = JsonSerializer.Deserialize<VrpItemPickupPoint[]>( pickupJson );
			if ( !DoPickup( client, itemEnt, pickups, out var error ) )
			{
				VrpSystem.SendChatMessage( To.Single( client ), error );
				return;
			}
		}

		[ServerCmd]
		public static void CmdEndPickup()
		{
			Host.AssertServer();

			var client = ConsoleSystem.Caller;

			EndPickup( client );
		}

		public static void RpcNetworkPickup( Client client, int networkIdent, VrpItemPickupPoint[] pickups )
		{
			Host.AssertServer();

			try
			{
				var json = JsonSerializer.Serialize( pickups );
				RpcNetworkPickup( client, networkIdent, json );
			}
			catch ( Exception e )
			{
				VrpSystem.SendChatMessage( To.Single( client ), "An error occured when networking your pickup data" );
			}
		}

		[ClientRpc]
		public static void RpcNetworkPickup( Client client, int networkIdent, string pickupJson )
		{
			Host.AssertClient();

			if ( sClientActivePickups.ContainsKey( client ) )
			{
				sClientActivePickups.Remove( client );
			}

			var pickups = JsonSerializer.Deserialize<VrpItemPickupPoint[]>( pickupJson );
			var pickup = new VrpItemActivePickup( client, null, pickups );

			sClientActivePickups.Add( client, (networkIdent, pickup) );
		}

		[ClientRpc]
		public static void RpcClearPickup( Client client )
		{
			Host.AssertClient();

			if ( sClientActivePickups.ContainsKey( client ) )
			{
				sClientActivePickups.Remove( client );
			}
		}

		public static bool TryGetClientPickup( Client client, out VrpItemActivePickup pickup )
		{
			if ( sClientActivePickups.TryGetValue( client, out var value ) && value.Item2.Entity != null )
			{
				pickup = value.Item2;
				return true;
			}

			pickup = null;
			return false;
		}

		[Event.Tick]
		private static void ClientPickupsThink()
		{
			if ( Host.IsServer )
			{
				return;
			}

			foreach ( var pair in sClientActivePickups )
			{
				var client = pair.Key;
				var networkIdent = pair.Value.Item1;
				var pickup = pair.Value.Item2;

				if ( pickup.Entity == null )
				{
					if ( (Entity.FindByIndex( networkIdent ) is VrpItemEntity itemEnt) )
					{
						pickup.Entity = itemEnt;

						foreach ( var point in pickup.Points )
						{
							point.Entity = itemEnt;
						}
					}

					continue;
				}


			}
		}
	}
}
